using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Beskar.Memory.Extensions;

/// <summary>
/// Provides high-performance extension methods for <see cref="Task"/> and asynchronous operations.
/// </summary>
public static class TaskExtensions
{
   /// <summary>
   /// Awaits the task and, if faulted, throws all inner exceptions wrapped in an <see cref="AggregateException"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static async Task WithAggregateException(this Task task)
   {
      try
      {
         await task;
      }
      catch
      {
         if (task.Exception is not null)
         {
            throw task.Exception;
         }
         throw;
      }
   }
}
