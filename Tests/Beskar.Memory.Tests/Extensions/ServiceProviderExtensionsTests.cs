using System;
using System.Collections.Generic;
using Xunit;
using Beskar.Memory.Extensions;

namespace Beskar.Memory.Tests.Extensions;

public class ServiceProviderExtensionsTests
{
   [Fact]
   public void TryGetServiceArity1To8()
   {
      var provider = new MockServiceProvider();
      
      var s1 = new Service1();
      var s2 = new Service2();
      var s3 = new Service3();
      var s4 = new Service4();
      var s5 = new Service5();
      var s6 = new Service6();
      var s7 = new Service7();
      var s8 = new Service8();

      provider.AddService(s1);
      provider.AddService(s2);
      provider.AddService(s3);
      provider.AddService(s4);
      provider.AddService(s5);
      provider.AddService(s6);
      provider.AddService(s7);
      provider.AddService(s8);

      Assert.True(provider.TryGetService<Service1>(out var res1));
      Assert.Same(s1, res1);

      Assert.True(provider.TryGetServices<Service1, Service2>(out var r1, out var r2));
      Assert.Same(s1, r1);
      Assert.Same(s2, r2);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3>(out r1, out r2, out var r3));
      Assert.Same(s3, r3);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3, Service4>(out r1, out r2, out r3, out var r4));
      Assert.Same(s4, r4);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3, Service4, Service5>(out r1, out r2, out r3, out r4, out var r5));
      Assert.Same(s5, r5);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3, Service4, Service5, Service6>(out r1, out r2, out r3, out r4, out r5, out var r6));
      Assert.Same(s6, r6);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3, Service4, Service5, Service6, Service7>(out r1, out r2, out r3, out r4, out r5, out r6, out var r7));
      Assert.Same(s7, r7);

      Assert.True(provider.TryGetServices<Service1, Service2, Service3, Service4, Service5, Service6, Service7, Service8>(out r1, out r2, out r3, out r4, out r5, out r6, out r7, out var r8));
      Assert.Same(s8, r8);
   }

   [Fact]
   public void TryGetServiceMissingReturnsFalse()
   {
      var provider = new MockServiceProvider();
      
      Assert.False(provider.TryGetService<Service1>(out var res1));
      Assert.Null(res1);

      provider.AddService(new Service1());

      Assert.False(provider.TryGetServices<Service1, Service2>(out _, out var res2));
      Assert.Null(res2);
   }

   private sealed class MockServiceProvider : IServiceProvider
   {
      private readonly Dictionary<Type, object> _services = new();

      public void AddService<T>(T service) where T : class
      {
         _services[typeof(T)] = service;
      }

      public object? GetService(Type serviceType)
      {
         return _services.TryGetValue(serviceType, out var service) ? service : null;
      }
   }

   private sealed class Service1;
   private sealed class Service2;
   private sealed class Service3;
   private sealed class Service4;
   private sealed class Service5;
   private sealed class Service6;
   private sealed class Service7;
   private sealed class Service8;
}
