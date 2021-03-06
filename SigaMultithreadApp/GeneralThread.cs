using System;
using System.Threading;
using System.Threading.Tasks;

namespace SigaMultithreadApp
{
    public abstract class GeneralThread
    {
        protected CancellationTokenSource CurrentCts;
        protected readonly int MinRandomMs;
        protected readonly int MaxRandomMs;

        protected GeneralThread(int minRandomMs, int maxRandomMs)
        {
            if (minRandomMs > maxRandomMs)
                throw new ArgumentOutOfRangeException();

            MinRandomMs = minRandomMs;
            MaxRandomMs = maxRandomMs;
        }

        public virtual void Start()
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            CurrentCts = cts;
            Task.Run(() => Run(ct), ct);
        }

        public virtual void Stop()
        {
            CurrentCts?.Cancel();
        }

        protected int GetNextRandomMs()
        {
            var random = new Random();
            return random.Next(MinRandomMs, MaxRandomMs);
        }
        protected abstract void Run(CancellationToken cancellationToken);

    }
}
