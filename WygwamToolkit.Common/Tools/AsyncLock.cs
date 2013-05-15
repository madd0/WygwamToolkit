//-----------------------------------------------------------------------
// <copyright file="AsyncSemaphore.cs" company="Stephen Toub">
//     Code by Stephen Toub http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
// </copyright>
//-----------------------------------------------------------------------

namespace Wygwam.Windows
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class AsyncLock
    {
        private readonly AsyncSemaphore m_semaphore;
        private readonly Task<Releaser> m_releaser; 

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
            m_releaser = Task.FromResult(new Releaser(this));
        } 

        public Task<Releaser> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                m_releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }

            public void Dispose()
            {
                if (m_toRelease != null)
                    m_toRelease.m_semaphore.Release();
            }
        } 
    }
}
