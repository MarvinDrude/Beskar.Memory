namespace Beskar.Memory.Constants;

/// <summary>
/// Defines buffer-related constants used throughout the memory package.
/// </summary>
internal static class BufferConstants
{
   /// <summary>
   /// The maximum byte buffer size considered safe for stack allocations.
   /// </summary>
   public const int StackSafeByteBufferSize = 256;
}
