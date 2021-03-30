using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace ParallelCompletion.Tests
{
    public class BasicTests
    {
        private async Task<TimeSpan> TimedTask(Task task)
        {
            var sw = new Stopwatch();
            sw.Start();
            await task;
            return sw.Elapsed;
        }
        
        private Task<TimeSpan> TimedTask(Func<Task> task)
            => TimedTask(task());

        [Fact]
        public async Task EmptyParallelTokenSource()
        {
            var pts = new ParallelTokenSource();
            var time = await TimedTask(pts.WaitForCompletion);
            
            Assert.InRange(time.TotalMilliseconds, 0, 5);
        }

        [Theory]
        [InlineData(100, 25)]
        public async Task SingleTask(int duration, int tolerance)
        {
            var time = await TimedTask(async () =>
            {
                var pts = new ParallelTokenSource();
                var token = pts.CreateToken();
                _ = Task.Run(async () =>
                {
                    await Task.Delay(duration);
                    token.Complete();
                });

                await pts.WaitForCompletion();
            });
            
            Assert.InRange(time.TotalMilliseconds, duration - tolerance, duration + tolerance);
        }
        
        [Theory]
        [InlineData(500, 100, 10)]
        [InlineData(100, 10, 10)]
        public async Task MultiExecution(int waitTime, int interval, int tolerance)
        {
            var time = await TimedTask(async () =>
            {
                var pts = new ParallelTokenSource();
                for (int i = waitTime; i > 0; i -= interval)
                {
                    var current = i;
                    var token = pts.CreateToken();
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(current);
                        token.Complete();
                    });
                }

                await pts.WaitForCompletion();
            });
            
            Assert.InRange(time.TotalMilliseconds, waitTime - tolerance, waitTime + tolerance);
        }
    }
}
