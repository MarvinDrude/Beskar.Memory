using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Pools;

namespace Beskar.Memory.Tests.Pools;

public class IoQueueTests
{
   [Fact]
   public async Task ScheduleSequential()
   {
      var queue = new IoQueue();
      var tcs = new TaskCompletionSource<int>();
      var result = 0;

      queue.Schedule(state =>
      {
         result = (int)state!;
         tcs.TrySetResult(result);
      }, 42);

      var executed = await tcs.Task;
      Assert.Equal(42, executed);
      Assert.Equal(42, result);
   }

   [Fact]
   public async Task ScheduleConcurrent()
   {
      var queue = new IoQueue();
      var counter = 0;
      var totalTasks = 100;
      var tcs = new TaskCompletionSource();

      for (var i = 0; i < totalTasks; i++)
      {
         _ = Task.Run(() =>
         {
            queue.Schedule(state =>
            {
               var val = Interlocked.Increment(ref counter);
               if (val == totalTasks)
               {
                  tcs.TrySetResult();
               }
            }, null);
         });
      }

      await tcs.Task;
      Assert.Equal(totalTasks, counter);
   }

   [Fact]
   public async Task ScheduleLargeVolume()
   {
      var queue = new IoQueue();
      var counter = 0;
      var total = 1000;
      var tcs = new TaskCompletionSource();

      for (var i = 0; i < total; i++)
      {
         queue.Schedule(state =>
         {
            var val = Interlocked.Increment(ref counter);
            if (val == total)
            {
               tcs.TrySetResult();
            }
         }, null);
      }

      await tcs.Task;
      Assert.Equal(total, counter);
   }
}
