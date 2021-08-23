using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SigaMultithreadApp
{
    public class Manager : GeneralThread
    {
        public delegate void DataProcessed(ProcessedMessage processedMessage);
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

        public override void Start()
        {
            for (var i = 1; i <= _numberOfThreads; i++)
            {
                var currentDataSupplier = new DataSupplier(_numberOfThreads * 1350, _numberOfThreads * 3000, this);
                currentDataSupplier.Start();
            }
            base.Start();
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

                int queueCount;
                lock (_messagesQueue)
                    queueCount = _messagesQueue.Count;

                if (queueCount <= 0) continue;
                while (queueCount > 0 && !cancellationToken.IsCancellationRequested)
                {
                    Message currentMessage;
                    lock (_messagesQueue)
                        currentMessage = _messagesQueue.Dequeue();

                    var processedMessage = ProcessMessage(currentMessage);

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    OnNewDataProcessed(processedMessage);
                    lock (_messagesQueue)
                        queueCount = _messagesQueue.Count;
                }
            }
        }

        protected virtual void OnNewDataProcessed(ProcessedMessage processedMessage)
        {
            NewDataProcessed?.Invoke(processedMessage);
        }

        private ProcessedMessage ProcessMessage(Message message)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(GetNextRandomMs());
            var timeSpan = stopwatch.Elapsed;
            stopwatch.Stop();
            return new ProcessedMessage
            {
                Message = message,
                ProcessingTime = timeSpan,
                EndProcessingTime = DateTime.UtcNow
            };
        }
    }
}
