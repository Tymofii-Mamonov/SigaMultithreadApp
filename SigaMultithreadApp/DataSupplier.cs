using System.Threading;

namespace SigaMultithreadApp
{
    public class DataSupplier : GeneralThread
    {
        public DataSupplier(int minRandomMs, int maxRandomMs, Manager manager) : base(minRandomMs, maxRandomMs)
        {
            _supplierManager = manager;
        }

        private readonly Manager _supplierManager;

        public override void Start()
        {
            new Thread(Stop).Start();
            base.Start();
        }

        public override void Stop()
        {
            _supplierManager.StopRequested.WaitOne();
            base.Stop();
        }

        protected override void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(GetNextRandomMs());
                if (cancellationToken.IsCancellationRequested)
                    break;
                var message = new Message();
                _supplierManager?.Enqueue(message);
                _supplierManager?.MessageEnqueued.Set();
            }
        }
    }
}
