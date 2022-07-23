using System.Buffers;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace HtmlUtilities;

/// <summary>
/// Modeled after <see cref="ArrayBufferWriter{T}"/> and its extension methods, but faster, more efficient, and potentially allocation-free.
/// </summary>
/// <typeparam name="T">The type of array desired.</typeparam>
/// <remarks>This is not part of the public API because it's easy to misuse and I don't want to help everyone fix their bugs.</remarks>
internal ref struct ArrayBuilder<T>
{
    private static readonly ArrayPool<T> arrayPool = ArrayPool<T>.Create();

    private T[] buffer;
    private int written;

    public ArrayBuilder(int initialCapacity)
    {
        // Use power-of-two scaling to improve the chance of finding an existing array in the pool.
        buffer = arrayPool.Rent((int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(4, initialCapacity)));
        written = 0;
    }

    private void Grow(int amount)
    {
        var old = buffer;
        buffer = arrayPool.Rent((int)BitOperations.RoundUpToPowerOf2((uint)(written + amount)));
        Array.Copy(old, buffer, written);
        arrayPool.Return(old);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan(int length)
    {
        if (written + length > buffer.Length)
            Grow(length);

        return buffer.AsSpan(written, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(T value)
    {
        if (written + 1 > buffer.Length)
            Grow(1);

        buffer[written++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<T> values)
    {
        var length = values.Length;
        var destination = GetSpan(length);
        values.CopyTo(destination);
        written += length;
    }

    public readonly ReadOnlySpan<T> WrittenSpan => buffer.AsSpan(0, written);

    public readonly T[] ToArray() => WrittenSpan.ToArray();

    public readonly void Release()
    {
        arrayPool.Return(buffer);
    }
}
