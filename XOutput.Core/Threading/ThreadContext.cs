﻿using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Threading
{
    public class ThreadContext
    {
        public bool Running => !Stopped;
        private bool Stopped => thread == null || thread.ThreadState.HasFlag(ThreadState.Stopped) || thread.ThreadState.HasFlag(ThreadState.Aborted) || thread.ThreadState.HasFlag(ThreadState.Unstarted);

        private readonly Thread thread;
        private readonly CancellationTokenSource tokenSource;
        private readonly ThreadResult result;

        internal ThreadContext(Thread thread, CancellationTokenSource tokenSource, ThreadResult result)
        {
            this.thread = thread;
            this.tokenSource = tokenSource;
            this.result = result;
        }

        [SupportedOSPlatform("windows")]
        public ThreadContext SetApartmentState(ApartmentState state)
        {
            thread.SetApartmentState(state);
            return this;
        }

        public ThreadContext Start()
        {
            if (Stopped)
            {
                thread.Start();
                return this;
            }
            throw new InvalidOperationException("Thread is already running!");
        }

        public ThreadContext Cancel()
        {
            if (!Stopped)
            {
                tokenSource.Cancel();
            }
            return this;
        }

        public ThreadResult Wait()
        {
            if (!Stopped)
            {
                thread.Join();
            }
            return result;
        }

        public Task<ThreadResult> WaitAsync()
        {
            var task = new Task<ThreadResult>(() => Wait());
            task.Start();
            return task;
        }
    }
}
