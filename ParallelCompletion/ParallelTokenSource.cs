using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelCompletion
{
    /// <summary>
    /// Provide methods to wait for multiple tasks to complete.
    /// </summary>
    public class ParallelTokenSource
    {
        private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        private readonly Dictionary<int, bool> _completionState = new Dictionary<int, bool>();
        private bool _isCompleted = false;
        private int _tokenCount = 0;

        private void CheckCompletionState()
        {
            if (_isCompleted)
                throw new InvalidOperationException($"This {nameof(ParallelTokenSource)} has been marked as completed and cannot be reused.");
        }

        /// <summary>
        /// Record the corresponding task's state as complete and checks if all tasks has completed.
        /// </summary>
        /// <param name="id">Task's identifier.</param>
        private void CompletionCallback(int id)
        {
            _completionState[id] = true;
            if (_completionState.Any(x => !x.Value)) return;
            _isCompleted = true;
            _tcs.SetResult(true);
        }

        /// <summary>
        /// Wait for all associated tasks to complete.
        /// </summary>
        public async Task WaitForCompletion()
        {
            CheckCompletionState();
            
            // wait for all tasks to complete, return immediately if no tasks were associated
            if (_tokenCount != 0)
                _ = await _tcs.Task;
            
            // clean up
            _completionState.Clear();
            
            // mark this instance as never to be used again
            _isCompleted = true;
        }

        /// <summary>
        /// Associate a task to this <see cref="ParallelTokenSource"/>, launch it and wait for completion.<br />
        /// This method returns immediately.
        /// </summary>
        /// <param name="task">Task to run.</param>
        public void RunTask(Task task)
        {
            var token = CreateToken();
            _ = Task.Run(async () =>
            {
                await task;
                token.Complete();
            });
        }

        /// <inheritdoc cref="RunTask(System.Threading.Tasks.Task)"/>
        public void RunTask(Func<Task> task)
            => RunTask(task());
        
        /// <summary>
        /// Create a <see cref="ParallelToken"/> associated with this <see cref="ParallelTokenSource"/>.
        /// </summary>
        /// <returns>Created <see cref="ParallelToken"/></returns>
        public ParallelToken CreateToken()
        {
            CheckCompletionState();
            
            int nextTokenId = Interlocked.Increment(ref _tokenCount);
            _completionState.Add(nextTokenId, false);

            return new ParallelToken(() => CompletionCallback(nextTokenId));
        }
    }
}
