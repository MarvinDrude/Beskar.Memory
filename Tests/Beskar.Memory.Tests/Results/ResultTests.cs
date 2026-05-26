using System;
using Xunit;
using Beskar.Memory.Results;
using Beskar.Memory.Results.Errors;

namespace Beskar.Memory.Tests.Results;

public class ResultTests
{
   [Fact]
   public void ResultSuccessState()
   {
      Result<string, int> r = "SuccessValue";
      
      Assert.True(r.IsSuccess);
      Assert.True(r.HasValue);
      Assert.False(r.Failed);
      Assert.Equal("SuccessValue", r.Success);
      Assert.Equal(0, r.Error);

      var (success, val, err) = r;
      Assert.True(success);
      Assert.Equal("SuccessValue", val);
      Assert.Equal(0, err);

      Assert.Equal("SUCCESS: SuccessValue", r.ToString());
   }

   [Fact]
   public void ResultErrorState()
   {
      Result<string, int> r = 500;
      
      Assert.False(r.IsSuccess);
      Assert.False(r.HasValue);
      Assert.True(r.Failed);
      Assert.Null(r.Success);
      Assert.Equal(500, r.Error);

      var (success, val, err) = r;
      Assert.False(success);
      Assert.Null(val);
      Assert.Equal(500, err);

      Assert.Equal("ERROR: 500", r.ToString());
   }

   [Fact]
   public void VoidResultSuccessState()
   {
      VoidResult<string> r = true;
      
      Assert.True(r.IsSuccess);
      Assert.True(r.HasValue);
      Assert.False(r.Failed);
      Assert.Null(r.Error);

      var (success, err) = r;
      Assert.True(success);
      Assert.Null(err);

      Assert.Equal("SUCCESS", r.ToString());
   }

   [Fact]
   public void VoidResultErrorState()
   {
      VoidResult<string> r = "ErrorMessage";
      
      Assert.False(r.IsSuccess);
      Assert.False(r.HasValue);
      Assert.True(r.Failed);
      Assert.Equal("ErrorMessage", r.Error);

      var (success, err) = r;
      Assert.False(success);
      Assert.Equal("ErrorMessage", err);

      Assert.Equal("ERROR: ErrorMessage", r.ToString());
   }

   [Fact]
   public void StringErrorAndGenericError()
   {
      StringError err1 = "DetailMessage";
      StringError err2 = "DetailMessage";
      StringError err3 = "OtherDetail";

      Assert.Equal("DetailMessage", err1.Detail);
      Assert.Equal("DetailMessage", err1.ToString());
      Assert.True(err1 == err2);
      Assert.True(err1 != err3);
      Assert.True(err1.Equals(err2));

      Error<int> gErr1 = 404;
      Error<int> gErr2 = 404;
      Error<int> gErr3 = 500;

      Assert.Equal(404, gErr1.Detail);
      Assert.Equal("404", gErr1.ToString());
      Assert.True(gErr1 == gErr2);
      Assert.True(gErr1 != gErr3);
   }

   [Fact]
   public void ResultEquality()
   {
      Result<string, int> r1 = "Val";
      Result<string, int> r2 = "Val";
      Result<string, int> r3 = "Other";
      Result<string, int> r4 = 500;

      Assert.True(r1 == r2);
      Assert.False(r1 == r3);
      Assert.False(r1 == r4);
      Assert.True(r1.Equals(r2));
   }
}
