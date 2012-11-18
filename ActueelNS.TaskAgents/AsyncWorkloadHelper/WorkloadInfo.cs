using System;

namespace ActueelNS.TaskAgents.AsyncWorkloadHelper
{
    public class WorkloadInfo<TParam, TResult>
    {
        Exception error;
        TResult result;
        AsyncWorkManager<TParam, TResult> parent;

        /// <summary>
        /// Create a new workload item
        /// </summary>
        /// <param name="parent">The parent WorkLoadManager that handles completion of this item</param>
        /// <param name="param">The parameter to pass to the Method when it is executed</param>
        /// <param name="method">The method to execute</param>
        internal WorkloadInfo(AsyncWorkManager<TParam, TResult> parent, TParam param, Action<TParam, WorkloadInfo<TParam, TResult>> method)
        {
            this.parent = parent;
            this.Error = null;
            this.Result = default(TResult);
            this.IsComplete = false;
            this.Parameter = param;
            this.Method = method;
        }

        /// <summary>
        /// Whether or not the item is complete
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// The exception, if any, that was raised by the Method
        /// </summary>
        public Exception Error
        {
            get { CheckComplete(); return error; }
            private set { error = value; }
        }

        /// <summary>
        /// The parameter to be used by the method
        /// </summary>
        public TParam Parameter { get; private set; }

        /// <summary>
        /// The method to be called for this work item
        /// </summary>
        public Action<TParam, WorkloadInfo<TParam, TResult>> Method { get; private set; }

        /// <summary>
        /// The result, if any, of this work item
        /// </summary>
        public TResult Result
        {
            get { CheckComplete(); return result; }
            private set { result = value; }
        }

        /// <summary>
        /// Completes the work item with a successful result
        /// </summary>
        /// <param name="result">The result of the operation</param>
        /// <remarks>This method is called by the worker Method once it has completed its task</remarks>
        public void NotifySuccess(TResult result)
        {
            MarkAsComplete();
            Result = result;
            parent.CompleteWorkItem();
        }

        /// <summary>
        /// Completes the work item with an error
        /// </summary>
        /// <param name="error">The error to report</param>
        /// <remarks>This method is called by the worker Method if it fails its task</remarks>
        public void NotifyFailure(Exception error)
        {
            MarkAsComplete();
            Error = error;
            parent.CompleteWorkItem();
        }

        /// <summary>
        /// Marks the item complete
        /// </summary>
        void MarkAsComplete()
        {
            lock (this)
            {
                if (IsComplete)
                    throw new InvalidOperationException("Can't complete more than once");

                IsComplete = true;
            }
        }

        /// <summary>
        /// Checks if the item is complete, and throws if not
        /// </summary>
        void CheckComplete()
        {
            if (!IsComplete)
                throw new InvalidOperationException("This work item is not yet complete");
        }
    }
}
