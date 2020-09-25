using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluffyBunny.OAuth2TokenManagment
{
    internal class TimedLock
    {
        private readonly SemaphoreSlim toLock;
        private object acquireLock;

        public TimedLock()
        {
            toLock = new SemaphoreSlim(1, 1);
        }

        public async Task<LockReleaser> Lock(TimeSpan timeout)
        {
            if (await toLock.WaitAsync(timeout))
            {
                return new LockReleaser(toLock);
            }
            throw new TimeoutException();
        }

        public struct LockReleaser : IDisposable
        {
            private readonly SemaphoreSlim toRelease;

            public LockReleaser(SemaphoreSlim toRelease)
            {
                this.toRelease = toRelease;
            }
            public void Dispose()
            {
                toRelease.Release();
            }
        }
    }
}
