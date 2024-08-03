using System.Buffers;
using System.Runtime.CompilerServices;

namespace HtmlUtilities;

/// <summary>
/// Modeled after <see cref="ArrayBufferWriter{T}"/> and its extension methods, but faster, more efficient, and potentially allocation-free.
/// </summary>
/// <typeparam name="T">The type of array desired.</typeparam>
/// <remarks>This is not part of the public API because it's easy to misuse and I don't want to help everyone fix their bugs.</remarks>
internal ref struct ArrayBuilder<T>(int initialCapacity)
{
    // Accessing ArrayPool<T>.Shared is inlined by the JIT, which then sees a sealed implementation, allowing removal of the v-table lookups.
    // The object returned by ArrayPool<T>.Create cannot get this benefit.
    // Reference: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Buffers/ArrayPool.cs
    // Using the shared pool carries risk of malfunction if another user returns a rented array but continues to modify it.
    // Microsoft uses ArrayPool<T>.Shared extensively within the .NET runtime, so if they're okay with this risk, I suppose I am, too.
    private T[] buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
    private int written = 0;

    private void Grow(int amount)
    {
        var old = buffer;
        buffer = ArrayPool<T>.Shared.Rent(written + amount);
        Array.Copy(old, buffer, written);
        ArrayPool<T>.Shared.Return(old);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan(int sizeHint = 0)
    {
        if (written + sizeHint > buffer.Length)
            Grow(sizeHint);

        return buffer.AsSpan(written, buffer.Length - written);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlyMemory<T> values) => Write(values.Span);

    internal readonly T[] Buffer => buffer;

    internal readonly ReadOnlyMemory<T> WrittenMemory => buffer.AsMemory(0, written);

    public readonly ReadOnlySpan<T> WrittenSpan => buffer.AsSpan(0, written);

    public readonly void Release() => ArrayPool<T>.Shared.Return(buffer);

    public static implicit operator ReadOnlyMemory<T>(ArrayBuilder<T> builder) => builder.WrittenSpan.ToArray();
}

internal static class ArrayBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(ref this ArrayBuilder<byte> arrayBuilder, char c)
    {
        System.Diagnostics.Debug.Assert(c <= byte.MaxValue);

        arrayBuilder.Write((byte)c);
    }
}
