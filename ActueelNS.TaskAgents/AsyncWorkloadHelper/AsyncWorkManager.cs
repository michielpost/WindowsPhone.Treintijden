using System;
using System.Threading;

namespace ActueelNS.TaskAgents.AsyncWorkloadHelper
{
    public class AsyncWorkManager<TParam, TResult>
    {
        /// <summary>
        /// Number of outstanding operations before everything is complete
        /// </summary>
        int outstandingItems = 0;

        /// <summary>
        /// Event to trigger starting all the operations
        /// </summary>
        ManualResetEvent startEvent = new ManualResetEvent(false);

        /// <summary>
        /// Event to release any clients waiting on the completion of the work
        /// </summary>
        ManualResetEvent completionEvent = new ManualResetEvent(false);

        /// <summary>
        /// Adds a new work item
        /// </summary>
        /// <param name="method">The method that represents the work to perform</param>
        /// <param name="parameter">Parameter to pass to the work method</param>
        /// <returns>An object that can be used to retrieve the result after completion</returns>
        public WorkloadInfo<TParam, TResult> AddWorkItem(Action<TParam, WorkloadInfo<TParam, TResult>> method, TParam parameter)
        {
            var work = new WorkloadInfo<TParam, TResult>(this, parameter, method);
            BeginWorkItem(work);
            return work;
        }

        /// <summary>
        /// Waits indefinitely for all the work items to complete
        /// </summary>
        public void WaitAll()
        {
            Start();
            completionEvent.WaitOne();
        }

        /// <summary>
        /// Waits for all the work items to complete, or for a specific timeout 
        /// </summary>
        /// <param name="timeout">The time to wait before giving up</param>
        /// <returns>true if the wait succeeded; false if the timeout expired</returns>
        public bool WaitAll(TimeSpan timeout)
        {
            Start();
            return completionEvent.WaitOne(timeout);
        }

        /// <summary>
        /// Starts performing work, if not already happening.
        /// </summary>
        /// <remarks>This method is implicitly called by the WaitAll methods</remarks>
        public void Start()
        {
            lock (this)
            {
                // Work has already been completed
                if (completionEvent.WaitOne(0) == true)
                    return;

                // Reset the completion event to be waited on
                completionEvent.Reset();
            }

            // Release the threads waiting to do work
            startEvent.Set();
        }

        /// <summary>
        /// Completes a work item, decrementing the amount of work left to do
        /// </summary>
        internal void CompleteWorkItem()
        {
            DecrementOutstandingItemsAndFinishIfNecessary();
        }

        /// <summary>
        /// Schedules a work item to begin
        /// </summary>
        /// <param name="work">The work to perform</param>
        /// <remarks>The worker thread is blocked on the startEvent so that the work
        /// doesn't complete before the client is ready to handle the completion event
        /// </remarks>
        void BeginWorkItem(WorkloadInfo<TParam, TResult> work)
        {
            lock (this)
            {
                outstandingItems++;
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                // Wait until it's OK to start
                startEvent.WaitOne();
                try
                {
                    // Method is responsible for completing itself if it didn't fail
                    work.Method(work.Parameter, work);
                }
                catch (Exception ex)
                {
                    // Complete with a failure case
                    work.NotifyFailure(ex);
                }
            });
        }

        /// <summary>
        /// Handles book-keeping for when items complete, including notifying waiting clients
        /// when all the work is done
        /// </summary>
        void DecrementOutstandingItemsAndFinishIfNecessary()
        {
            lock (this)
            {
                outstandingItems--;
                if (outstandingItems < 0)
                    throw new Exception("Internal error: too many items completed!");

                if (outstandingItems == 0)
                    CompleteWorkload();
            }
        }

        /// <summary>
        /// Releases any clients waiting on the completion, and raises the WorkComplete event
        /// </summary>
        /// <remarks>This is called inside a lock from DecrementOutstandingItemsAndFinishIfNecessary</remarks>
        void CompleteWorkload()
        {
            completionEvent.Set();
            startEvent.Reset();
            RaiseWorkComplete();
        }

        /// <summary>
        /// Helper to raise the completion method
        /// </summary>
        void RaiseWorkComplete()
        {
            var handler = WorkComplete;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised when all the work is complete.
        /// </summary>
        public event EventHandler WorkComplete;
    }
}
