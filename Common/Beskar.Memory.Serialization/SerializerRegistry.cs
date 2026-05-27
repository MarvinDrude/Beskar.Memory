using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Beskar.Memory.Buffers;
using Beskar.Memory.Serialization.Interfaces;
using Beskar.Memory.Serialization.Resolvers;
using Beskar.Memory.Writers;
using Beskar.Memory.Serialization.Serializers.Collections;
using Beskar.Memory.Serialization.Serializers.Collections.Concurrent;
using Beskar.Memory.Serialization.Serializers.Collections.Immutable;
using Beskar.Memory.Serialization.Serializers.System;

namespace Beskar.Memory.Serialization;

public delegate int WriteDelegate<T>(ref BufferWriter<byte> writer, scoped in T value)
   where T : allows ref struct;

public delegate bool TryReadDelegate<T>(ref SequenceReader<byte> reader, [MaybeNullWhen(false)] out T value)
   where T : allows ref struct;

public delegate int CalculateByteLengthDelegate<T>(scoped in T value)
   where T : allows ref struct;

/// <summary>
/// Central registry for managing serializer resolvers, explicit type mappings, and open generic serialization mappings.
/// </summary>
public static class SerializerRegistry
{
   private static volatile ISerializerResolver[] _resolvers;
   private static readonly ConcurrentDictionary<Type, Type> _explicitMappings = new();
   private static readonly ConcurrentDictionary<Type, Type> _openGenericMappings = new();

   private static readonly Lock _resolverLock = new();

   static SerializerRegistry()
   {
      // Register default resolvers
      _resolvers =
      [
         new RegisteredSerializerResolver(),
         new OpenGenericSerializerResolver(),
         new ArraySerializerResolver(),
         new EnumSerializerResolver()
      ];

      // Register default open generic mappings
      RegisterOpenGeneric(typeof(Lazy<>), typeof(LazySerializer<>));
      RegisterOpenGeneric(typeof(Nullable<>), typeof(NullableSerializer<>));
      RegisterOpenGeneric(typeof(List<>), typeof(ListSerializer<>));
      RegisterOpenGeneric(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
      RegisterOpenGeneric(typeof(HashSet<>), typeof(HashSetSerializer<>));
      RegisterOpenGeneric(typeof(Queue<>), typeof(QueueSerializer<>));
      RegisterOpenGeneric(typeof(Stack<>), typeof(StackSerializer<>));
      RegisterOpenGeneric(typeof(LinkedList<>), typeof(LinkedListSerializer<>));
      RegisterOpenGeneric(typeof(SortedList<,>), typeof(SortedListSerializer<,>));
      RegisterOpenGeneric(typeof(SortedDictionary<,>), typeof(SortedDictionarySerializer<,>));
      RegisterOpenGeneric(typeof(ReadOnlyCollection<>), typeof(ReadOnlyCollectionSerializer<>));
      RegisterOpenGeneric(typeof(ReadOnlyDictionary<,>), typeof(ReadOnlyDictionarySerializer<,>));
      RegisterOpenGeneric(typeof(ArraySegment<>), typeof(ArraySegmentSerializer<>));

      // Concurrent
      RegisterOpenGeneric(typeof(ConcurrentBag<>), typeof(ConcurrentBagSerializer<>));
      RegisterOpenGeneric(typeof(ConcurrentQueue<>), typeof(ConcurrentQueueSerializer<>));
      RegisterOpenGeneric(typeof(ConcurrentStack<>), typeof(ConcurrentStackSerializer<>));
      RegisterOpenGeneric(typeof(ConcurrentDictionary<,>), typeof(ConcurrentDictionarySerializer<,>));

      // Immutable
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableArray<>), typeof(ImmutableArraySerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableList<>), typeof(ImmutableListSerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableHashSet<>), typeof(ImmutableHashSetSerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableDictionary<,>), typeof(ImmutableDictionarySerializer<,>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableQueue<>), typeof(ImmutableQueueSerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableStack<>), typeof(ImmutableStackSerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableSortedSet<>), typeof(ImmutableSortedSetSerializer<>));
      RegisterOpenGeneric(typeof(System.Collections.Immutable.ImmutableSortedDictionary<,>), typeof(ImmutableSortedDictionarySerializer<,>));
   }

   /// <summary>
   /// Registers an explicit serializer type for a value type.
   /// </summary>
   public static void Register(Type valueType, Type serializerType)
   {
      // Verify constraint: serializerType must implement ISerializer<valueType>
      var targetInterface = typeof(ISerializer<>).MakeGenericType(valueType);
      if (!targetInterface.IsAssignableFrom(serializerType))
      {
         throw new ArgumentException($"Serializer type {serializerType.FullName} must implement ISerializer<{valueType.FullName}>.");
      }

      var registryType = typeof(SerializerRegistry<>).MakeGenericType(valueType);
      System.Reflection.MethodInfo? registerMethod = null;

      foreach (var method in registryType.GetMethods(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
      {
         if (method is { Name: "Register", IsGenericMethod: true }
             && method.GetGenericArguments().Length == 1
             && method.GetParameters().Length == 0)
         {
            registerMethod = method;
            break;
         }
      }

      if (registerMethod == null)
      {
         throw new InvalidOperationException($"Could not find Register method on SerializerRegistry<{valueType.Name}>.");
      }

      var genericRegisterMethod = registerMethod.MakeGenericMethod(serializerType);
      genericRegisterMethod.Invoke(null, null);

      _explicitMappings[valueType] = serializerType;
   }

   /// <summary>
   /// Registers an open generic value type mapping to an open generic serializer type.
   /// </summary>
   public static void RegisterOpenGeneric(Type openGenericValueType, Type openGenericSerializerType)
   {
      if (!openGenericValueType.IsGenericTypeDefinition)
      {
         throw new ArgumentException("Value type must be an open generic definition.", nameof(openGenericValueType));
      }

      if (!openGenericSerializerType.IsGenericTypeDefinition)
      {
         throw new ArgumentException("Serializer type must be an open generic definition.", nameof(openGenericSerializerType));
      }

      _openGenericMappings[openGenericValueType] = openGenericSerializerType;
   }

   /// <summary>
   /// Registers a custom serializer resolver.
   /// </summary>
   public static void RegisterResolver(ISerializerResolver resolver)
   {
      lock (_resolverLock)
      {
         var current = _resolvers;
         var newResolvers = new ISerializerResolver[current.Length + 1];

         Array.Copy(current, newResolvers, current.Length);
         newResolvers[current.Length] = resolver;

         _resolvers = newResolvers;
      }
   }

   /// <summary>
   /// Inserts a custom serializer resolver at the specified index.
   /// </summary>
   public static void InsertResolver(int index, ISerializerResolver resolver)
   {
      lock (_resolverLock)
      {
         var current = _resolvers;
         if (index < 0 || index > current.Length)
         {
            throw new ArgumentOutOfRangeException(nameof(index));
         }

         var newResolvers = new ISerializerResolver[current.Length + 1];
         if (index > 0)
         {
            Array.Copy(current, 0, newResolvers, 0, index);
         }

         newResolvers[index] = resolver;

         if (index < current.Length)
         {
            Array.Copy(current, index, newResolvers, index + 1, current.Length - index);
         }

         _resolvers = newResolvers;
      }
   }

   /// <summary>
   /// Tries to get an explicit serializer type for a given value type.
   /// </summary>
   public static bool TryGetExplicitSerializer(Type valueType, [NotNullWhen(true)] out Type? serializerType)
   {
      return _explicitMappings.TryGetValue(valueType, out serializerType);
   }

   /// <summary>
   /// Tries to get an open generic serializer mapping.
   /// </summary>
   public static bool TryGetOpenGenericMapping(Type openGenericValueType, [NotNullWhen(true)] out Type? openGenericSerializerType)
   {
      return _openGenericMappings.TryGetValue(openGenericValueType, out openGenericSerializerType);
   }

   /// <summary>
   /// Resolves a serializer for type <typeparamref name="T"/> using the registered resolvers.
   /// </summary>
   public static bool Resolve<T>()
      where T : allows ref struct
   {
      // ReSharper disable once InconsistentlySynchronizedField
      var resolvers = _resolvers;
      for (var i = 0; i < resolvers.Length; i++)
      {
         try
         {
            if (resolvers[i].TryResolve<T>())
            {
               return true;
            }
         }
         catch
         {
            // Suppress resolution exception from one resolver and continue to the next
         }
      }

      return false;
   }
}

/// <summary>
/// A zero-overhead, reflection-free static registry for serializers using delegate caching with dynamic on-demand resolution.
/// </summary>
public static class SerializerRegistry<T>
   where T : allows ref struct
{
   private static readonly Lock _registerLock = new();
   private static bool _isResolved;

   private static WriteDelegate<T>? _write;
   private static TryReadDelegate<T>? _tryRead;
   private static CalculateByteLengthDelegate<T>? _calculateByteLength;

   private static void EnsureInitialized()
   {
      if (_isResolved)
         return;

      lock (_registerLock)
      {
         if (_isResolved)
            return;

         if (_write == null
             || _tryRead == null
             || _calculateByteLength == null)
         {
            SerializerRegistry.Resolve<T>();
         }

         _isResolved = true;
      }
   }

   /// <summary>
   /// Gets a value indicating whether a serializer is already registered for type <typeparamref name="T"/> without triggering dynamic resolution.
   /// </summary>
   public static bool IsAlreadyRegistered => _write != null;

   /// <summary>
   /// Registers the serializer type <typeparamref name="TSerializer"/> for type <typeparamref name="T"/>.
   /// </summary>
   public static void Register<TSerializer>()
      where TSerializer : ISerializer<T>
   {
      lock (_registerLock)
      {
         _write = TSerializer.Write;
         _tryRead = TSerializer.TryRead;
         _calculateByteLength = TSerializer.CalculateByteLength;

         _isResolved = true;
      }
   }

   /// <summary>
   /// Registers custom serialization delegates for type <typeparamref name="T"/>.
   /// </summary>
   public static void Register(
      WriteDelegate<T> write,
      TryReadDelegate<T> tryRead,
      CalculateByteLengthDelegate<T> calculateByteLength)
   {
      lock (_registerLock)
      {
         _write = write;
         _tryRead = tryRead;
         _calculateByteLength = calculateByteLength;

         _isResolved = true;
      }
   }

   /// <summary>
   /// The cached static write delegate for type <typeparamref name="T"/>.
   /// </summary>
   public static WriteDelegate<T>? Write
   {
      get
      {
         EnsureInitialized();
         return _write;
      }
      set
      {
         lock (_registerLock)
         {
            _write = value;
            _isResolved = true;
         }
      }
   }

   /// <summary>
   /// The cached static read delegate for type <typeparamref name="T"/>.
   /// </summary>
   public static TryReadDelegate<T>? TryRead
   {
      get
      {
         EnsureInitialized();
         return _tryRead;
      }
      set
      {
         lock (_registerLock)
         {
            _tryRead = value;
            _isResolved = true;
         }
      }
   }

   /// <summary>
   /// The cached static length calculation delegate for type <typeparamref name="T"/>.
   /// </summary>
   public static CalculateByteLengthDelegate<T>? CalculateByteLength
   {
      get
      {
         EnsureInitialized();
         return _calculateByteLength;
      }
      set
      {
         lock (_registerLock)
         {
            _calculateByteLength = value;
            _isResolved = true;
         }
      }
   }

   /// <summary>
   /// Gets the cached static write delegate for type <typeparamref name="T"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static WriteDelegate<T> GetWrite()
   {
      EnsureInitialized();
      return _write ?? throw new InvalidOperationException($"Write delegate not set for type {typeof(T)}.");
   }

   /// <summary>
   /// Gets the cached static read delegate for type <typeparamref name="T"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TryReadDelegate<T> GetTryRead()
   {
      EnsureInitialized();
      return _tryRead ?? throw new InvalidOperationException($"TryRead delegate not set for type {typeof(T)}.");
   }

   /// <summary>
   /// Gets the cached static length calculation delegate for type <typeparamref name="T"/>.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static CalculateByteLengthDelegate<T> GetCalculateByteLength()
   {
      EnsureInitialized();
      return _calculateByteLength ?? throw new InvalidOperationException($"CalculateByteLength delegate not set for type {typeof(T)}.");
   }
}
