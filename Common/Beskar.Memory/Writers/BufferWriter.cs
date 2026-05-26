using System.Runtime.InteropServices;

namespace Beskar.Memory.Writers;

[StructLayout(LayoutKind.Auto)]
public ref partial struct BufferWriter<T> : IDisposable
{

}
