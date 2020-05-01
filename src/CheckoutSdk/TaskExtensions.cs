using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Checkout
{
    /// <summary>
    /// Extensions on Task and Task<T>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// ConfigureAwait(false)
        /// </summary>
        public static ConfiguredTaskAwaitable NotOnCapturedContext(this Task task)
        {
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// ConfigureAwait(false)
        /// </summary>
        public static ConfiguredTaskAwaitable<T> NotOnCapturedContext<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
