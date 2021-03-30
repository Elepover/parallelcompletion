using System.Threading.Tasks;

namespace ParallelCompletion.Extensions
{
    public static class TaskExtensions
    {
        public static void RegisterTo(this Task task, ParallelTokenSource pts)
            => pts.RunTask(task);
    }
}
