using System;
using System.Diagnostics.CodeAnalysis;

namespace Beskar.Memory.Extensions;

/// <summary>
/// Provides high-performance extension methods for <see cref="IServiceProvider"/> to resolve multiple services safely.
/// </summary>
public static class ServiceProviderExtensions
{
   extension(IServiceProvider provider)
   {
      /// <summary>
      /// Tries to resolve a service of type <typeparamref name="T"/> from the provider.
      /// </summary>
      public bool TryGetService<T>([NotNullWhen(true)] out T? service)
         where T : class
      {
         service = provider.GetService(typeof(T)) as T;
         return service is not null;
      }
      
      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/> and <typeparamref name="T2"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2)
         where T1 : class 
         where T2 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         
         return service1 is not null 
            && service2 is not null;
      }
      
      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3)
         where T1 : class
         where T2 : class
         where T3 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null;
      }

      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, and <typeparamref name="T4"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3, T4>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3,
         [NotNullWhen(true)] out T4? service4)
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;
         service4 = provider.GetService(typeof(T4)) as T4;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null
             && service4 is not null;
      }

      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, and <typeparamref name="T5"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3, T4, T5>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3,
         [NotNullWhen(true)] out T4? service4,
         [NotNullWhen(true)] out T5? service5)
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
         where T5 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;
         service4 = provider.GetService(typeof(T4)) as T4;
         service5 = provider.GetService(typeof(T5)) as T5;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null 
             && service4 is not null 
             && service5 is not null;
      }

      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, and <typeparamref name="T5"/>, and <typeparamref name="T6"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3, T4, T5, T6>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3,
         [NotNullWhen(true)] out T4? service4,
         [NotNullWhen(true)] out T5? service5,
         [NotNullWhen(true)] out T6? service6)
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
         where T5 : class
         where T6 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;
         service4 = provider.GetService(typeof(T4)) as T4;
         service5 = provider.GetService(typeof(T5)) as T5;
         service6 = provider.GetService(typeof(T6)) as T6;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null 
             && service4 is not null 
             && service5 is not null 
             && service6 is not null;
      }

      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, and <typeparamref name="T7"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3, T4, T5, T6, T7>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3,
         [NotNullWhen(true)] out T4? service4,
         [NotNullWhen(true)] out T5? service5,
         [NotNullWhen(true)] out T6? service6,
         [NotNullWhen(true)] out T7? service7)
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
         where T5 : class
         where T6 : class
         where T7 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;
         service4 = provider.GetService(typeof(T4)) as T4;
         service5 = provider.GetService(typeof(T5)) as T5;
         service6 = provider.GetService(typeof(T6)) as T6;
         service7 = provider.GetService(typeof(T7)) as T7;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null 
             && service4 is not null 
             && service5 is not null 
             && service6 is not null
             && service7 is not null;
      }

      /// <summary>
      /// Tries to resolve services of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, and <typeparamref name="T8"/> from the provider.
      /// </summary>
      public bool TryGetServices<T1, T2, T3, T4, T5, T6, T7, T8>(
         [NotNullWhen(true)] out T1? service1,
         [NotNullWhen(true)] out T2? service2,
         [NotNullWhen(true)] out T3? service3,
         [NotNullWhen(true)] out T4? service4,
         [NotNullWhen(true)] out T5? service5,
         [NotNullWhen(true)] out T6? service6,
         [NotNullWhen(true)] out T7? service7,
         [NotNullWhen(true)] out T8? service8)
         where T1 : class
         where T2 : class
         where T3 : class
         where T4 : class
         where T5 : class
         where T6 : class
         where T7 : class
         where T8 : class
      {
         service1 = provider.GetService(typeof(T1)) as T1;
         service2 = provider.GetService(typeof(T2)) as T2;
         service3 = provider.GetService(typeof(T3)) as T3;
         service4 = provider.GetService(typeof(T4)) as T4;
         service5 = provider.GetService(typeof(T5)) as T5;
         service6 = provider.GetService(typeof(T6)) as T6;
         service7 = provider.GetService(typeof(T7)) as T7;
         service8 = provider.GetService(typeof(T8)) as T8;

         return service1 is not null 
             && service2 is not null 
             && service3 is not null 
             && service4 is not null 
             && service5 is not null 
             && service6 is not null
             && service7 is not null 
             && service8 is not null;
      }
   }
}
