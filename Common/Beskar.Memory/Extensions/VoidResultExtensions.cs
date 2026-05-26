using Beskar.Memory.Results;
using Beskar.Memory.Results.Errors;

namespace Beskar.Memory.Extensions;

public static class VoidResultExtensions
{
   extension(VoidResult<StringError> res)
   {
      public string ErrorMessage => res.Error.Detail;
   }
}
