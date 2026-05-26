using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Beskar.Memory.Timing;

namespace Beskar.Memory.Tests.Timing;

public class TimerTests
{
   [Fact]
   public void StackTimerSpan()
   {
      var result = TimeSpan.Zero;
      
      using (new StackTimer(ref result))
      {
         Thread.Sleep(15);
      }
      
      Assert.True(result > TimeSpan.Zero);
      Assert.True(result.TotalMilliseconds >= 10);
   }

   [Fact]
   public void StackTimerTicks()
   {
      var result = 0L;
      
      using (new StackTimer(ref result))
      {
         Thread.Sleep(15);
      }
      
      Assert.True(result > 0);
   }

   [Fact]
   public async Task AsyncTimerMeasurement()
   {
      var result = new AsyncTimerResult();
      
      using (new AsyncTimer(result))
      {
         await Task.Delay(15);
      }
      
      Assert.True(result.Elapsed > TimeSpan.Zero);
      Assert.True(result.ElapsedMilliseconds >= 10);
   }

   [Fact]
   public void AsyncTimerResultFeatures()
   {
      var result = new AsyncTimerResult
      {
         Elapsed = TimeSpan.FromMilliseconds(42.5)
      };
      
      Assert.Equal(42.5, result.ElapsedMilliseconds);
      Assert.Equal(TimeSpan.FromMilliseconds(42.5).Ticks, result.ElapsedTicks);
      Assert.Equal($"{42.5:F2} ms", result.ToString());
      
      result.Reset();
      Assert.Equal(TimeSpan.Zero, result.Elapsed);
   }

   [Fact]
   public void AsyncTimerNullSafety()
   {
      Assert.Throws<ArgumentNullException>(() => new AsyncTimer(null!));
   }
}
