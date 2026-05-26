using Beskar.Memory.Results;
using Beskar.Memory.Results.Errors;

namespace Beskar.Memory.Extensions;

public static class ResultExtensions
{
   extension<TResult>(Result<TResult, StringError> res)
   {
      public string ErrorMessage => res.Error.Detail;
   }
}
