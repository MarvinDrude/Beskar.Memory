using System;
using Beskar.Memory.Code.TypeIdGenerator.Attributes;

namespace Beskar.Memory.Code.TypeIdGenerator.Tests.Scenarios.Simple;

[TypeSafeId]
public readonly partial record struct ExampleIntId(int Value);

[TypeSafeId]
public readonly partial record struct ExampleULongId(ulong Value);

[TypeSafeId]
public readonly partial record struct ExampleGuidId(Guid Value);