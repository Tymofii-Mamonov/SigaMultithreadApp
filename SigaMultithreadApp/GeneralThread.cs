using System;
using System.Threading;
using System.Threading.Tasks;

namespace SigaMultithreadApp
{
    public abstract class GeneralThread : IDisposable
    {
        protected CancellationTokenSource CurrentCts;
        protected readonly int SleepingTimeMs;

        protected GeneralThread(int minRandomMs, int maxRandomMs)
        {
            var random = new Random();
            SleepingTimeMs = random.Next(minRandomMs, maxRandomMs);
        }

        public virtual async Task Start()
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            CurrentCts = cts;
            await Task.Run(() => Run(ct), ct);
        }

        public virtual void Stop()
        {
            CurrentCts?.Cancel();
            Dispose();
        }

        protected abstract void Run(CancellationToken cancellationToken);

        public virtual void Dispose()
        {
            CurrentCts?.Dispose();
        }
    }
}
