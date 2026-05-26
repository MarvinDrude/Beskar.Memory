using Beskar.Memory.Code.ObserveGenerator.Attributes;
using Beskar.Memory.Code.ObserveGenerator.Enums;

namespace Beskar.Memory.Code.ObserveGenerator.Tests.Scenarios.Simple;

[Observe]
[ObserveActivity]
[ObserveMeter(name: "Meter.Name", version: "2.2.2")]
[ObserveInstrument("IntegerCounter", InstrumentKind.Counter, typeof(int))]
public sealed partial class SimpleObserveClass
{
   
}

[Observe]
[ObserveActivity]
[ObserveMeter(name: "Meter.Name2", version: "2.2.2")]
[ObserveInstrument("Histo", InstrumentKind.Histogram, typeof(double), unit: "m/s", description: "This is a description")]
public static partial class SecondClass<T, T2>
   where T : class
{
   
}
