using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SigaMultithreadApp
{
    public class Manager : GeneralThread
    {
        public delegate void DataProcessed(string message);
        public event DataProcessed NewDataProcessed;
        public EventWaitHandle MessageEnqueued = new EventWaitHandle(false, EventResetMode.AutoReset);
        public EventWaitHandle StopRequested = new EventWaitHandle(false, EventResetMode.ManualReset);

        public Manager(int minRandomMs, int maxRandomMs, int threadsNumber) : base(minRandomMs, maxRandomMs)
        {
            _numberOfThreads = threadsNumber;
        }

        private readonly Queue<Message> _messagesQueue = new Queue<Message>();
        private readonly int _numberOfThreads;

        public void Enqueue(Message message)
        {
            lock (_messagesQueue)
            {
                _messagesQueue.Enqueue(message);
            }
        }

        public override async Task Start()
        {
            for (var i = 1; i <= _numberOfThreads; i++)
            {
                var currentDataSupplier = new DataSupplier(2000, 4000, this);
                currentDataSupplier.Start();
            }
            await base.Start();
        }

        public override void Stop()
        {
            StopRequested.Set();
            MessageEnqueued.Set();
            base.Stop();
        }

        protected override void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                MessageEnqueued.WaitOne();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Thread.Sleep(SleepingTimeMs);
                Message currentMessage = null;
                lock (_messagesQueue)
                {
                    if (_messagesQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
                        currentMessage = _messagesQueue.Dequeue();

                }
                var timeSpan = stopwatch.Elapsed;
                stopwatch.Stop();
                if (currentMessage != null)
                    OnNewDataProcessed(ProcessMessage(currentMessage, timeSpan));
            }
        }

        public string ProcessMessage(Message message, TimeSpan timeSpan)
        {
            return "=====================================\r\n" +
                   $"Message Description: {message.MessageText}\r\n" +
                   $"Message ID: {message.Id}\r\n" +
                   $"Message SentTime: {message.SentTime}\r\n" +
                   $"Processing Time: {timeSpan.Minutes}s {timeSpan.Milliseconds}ms\r\n" +
                   "=====================================\r\n";
        }

        protected virtual void OnNewDataProcessed(string message)
        {
            NewDataProcessed?.Invoke(message);
        }

        public override void Dispose()
        {
            MessageEnqueued?.Dispose();
            StopRequested?.Dispose();
            base.Dispose();
        }
    }
}
