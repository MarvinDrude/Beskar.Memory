using System;
using System.Threading.Channels;

namespace Beskar.Memory.Threading;

/// <summary>
/// Options for configuring a <see cref="WorkPool"/>.
/// </summary>
public sealed class WorkPoolOptions
{
   /// <summary>
   /// Gets the maximum number of worker tasks running in parallel.
   /// Defaults to <see cref="Environment.ProcessorCount"/>.
   /// </summary>
   public int MaxDegreeOfParallelism { get; init; } = Environment.ProcessorCount;

   /// <summary>
   /// Gets the maximum capacity of the work queue.
   /// Defaults to 10,000.
   /// </summary>
   public int Capacity { get; init; } = 10_000;

   /// <summary>
   /// Gets the behavior of the channel when the capacity is reached.
   /// Defaults to <see cref="BoundedChannelFullMode.Wait"/>.
   /// </summary>
   public BoundedChannelFullMode FullMode { get; init; } = BoundedChannelFullMode.Wait;
   
   /// <summary>
   /// Gets a value indicating whether the channel has a single reader.
   /// Defaults to <see langword="false"/>.
   /// </summary>
   public bool SingleReader { get; init; } = false;
}
