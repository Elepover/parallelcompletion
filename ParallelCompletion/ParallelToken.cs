using System;

namespace ParallelCompletion
{
    /// <summary>
    /// Represents a token associated with a task.
    /// </summary>
    public struct ParallelToken
    {
        internal ParallelToken(Action completionCallback)
        {
            _completionCallback = completionCallback;
        }

        private readonly Action _completionCallback;

        public void Complete()
            => _completionCallback();
    }
}
