using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Beskar.Memory.Utilities;

/// <summary>
/// Helper class for throwing <see cref="ObjectDisposedException"/>s.
/// </summary>
public static class DisposedExceptionHelper
{
   /// <summary>
   /// Throws <see cref="ObjectDisposedException"/> if the object is disposed.
   /// </summary>
   /// <param name="obj">The object in question</param>
   /// <typeparam name="T">Type of the object</typeparam>
   /// <exception cref="ObjectDisposedException">Will always throw this.</exception>
   [DoesNotReturn]
   [MethodImpl(MethodImplOptions.NoInlining)]
   public static void ThrowIfDisposed<T>()
      where T : IDisposable, allows ref struct
   {
      throw new ObjectDisposedException(TypeCache<T>.Name);
   }

   private static class TypeCache<T>
      where T : IDisposable, allows ref struct
   {
      public static readonly string? Name = typeof(T).ToString();
   }
}
