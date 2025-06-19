using System.Runtime.InteropServices;

namespace Mosico;

/// <summary>
/// Convenient byte-level manipulation of 32b integers and floats
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal struct FourBytes
{
    [FieldOffset(0)]
    public byte Byte0;
    [FieldOffset(1)]
    public byte Byte1;
    [FieldOffset(2)]
    public byte Byte2;
    [FieldOffset(3)]
    public byte Byte3;
    [FieldOffset(0)]
    public ushort Short0;
    [FieldOffset(2)]
    public ushort Short1;
    [FieldOffset(0)]
    public uint Int;
    [FieldOffset(0)]
    public float Float;

    public FourBytes(double value) : this((float)value) { }

    public FourBytes(float value)
    {
        Byte0 = 0;
        Byte1 = 0;
        Byte2 = 0;
        Byte3 = 0;
        Short0 = 0;
        Short1 = 0;
        Int = 0;
        Float = value;
    }

    public FourBytes(int value) : this((uint)value) { }

    public FourBytes(uint value)
    {
        Byte0 = 0;
        Byte1 = 0;
        Byte2 = 0;
        Byte3 = 0;
        Short0 = 0;
        Short1 = 0;
        Float = 0;
        Int = value;
    }

    public FourBytes(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Byte array must be exactly 4 bytes long.", nameof(bytes));

        Short0 = 0;
        Short1 = 0;
        Int = 0;
        Float = 0;
        Byte0 = bytes[0];
        Byte1 = bytes[1];
        Byte2 = bytes[2];
        Byte3 = bytes[3];
    }

    public FourBytes(byte[] bytes) : this((ReadOnlySpan<byte>)bytes) { }
    public FourBytes(Span<byte> bytes) : this((ReadOnlySpan<byte>)bytes) { }

    public readonly byte[] ToArray() => [Byte0, Byte1, Byte2, Byte3];
    public static byte[] ToArray(double value) => ToArray((float)value);
    public static byte[] ToArray(float value) => new FourBytes(value).ToArray();
    public static byte[] ToArray(int value) => new FourBytes((uint)value).ToArray();
    public static byte[] ToArray(uint value) => new FourBytes(value).ToArray();
    public static float ToFloat(byte[] array) => new FourBytes(array).Float;
    public static int ToInt(byte[] array) => (int)new FourBytes(array).Int;
    public static uint ToUInt(byte[] array) => new FourBytes(array).Int;
}