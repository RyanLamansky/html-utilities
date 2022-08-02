﻿using System.Buffers;

namespace HtmlUtilities;

/// <summary>
/// Writes attributes for an HTML element.
/// </summary>
public readonly struct AttributeWriter
{
    private readonly IBufferWriter<byte> writer;

    internal AttributeWriter(IBufferWriter<byte> writer)
    {
        System.Diagnostics.Debug.Assert(writer is not null);

        this.writer = writer;
    }

    /// <summary>
    /// Writes a validated attribute name with no value.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name)
    {
        writer.Write(name.value ?? throw new ArgumentException("name was never initialized.", nameof(name)));
    }

    /// <summary>
    /// Writes a validated attribute.
    /// </summary>
    /// <param name="attribute">The validated attribute to write.</param>
    /// <exception cref="ArgumentException"><paramref name="attribute"/> was never initialized.</exception>
    public void Write(ValidatedAttribute attribute)
    {
        writer.Write(attribute.value ?? throw new ArgumentException("attribute was never initialized.", nameof(attribute)));
    }

    /// <summary>
    /// Writes an attribute that consists only of a name, no value.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name)
    {
        var w = new ArrayBuilder<byte>(name.Length);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);
            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes a validated attribute name and unvalidated value pair.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name, ReadOnlySpan<char> value)
    {
        Write(name);
        var writer = this.writer;

        var w = new ArrayBuilder<byte>(value.Length + 3);
        try
        {
            ValidatedAttributeValue.Validate(value, ref w);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes an attribute name and value pair.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
    {
        var w = new ArrayBuilder<byte>(name.Length + value.Length + 3);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);
            ValidatedAttributeValue.Validate(value, ref w);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes a validated attribute name and value pair.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name, long value)
    {
        Write(name);

        Span<byte> result = stackalloc byte[ValidatedAttributeValue.CountBytes(value)];
        ValidatedAttributeValue.ToUtf8(value, result);

        writer.Write(result);
    }

    /// <summary>
    /// Writes a validated attribute name and optional value.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">If provided, the value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name, long? value)
    {
        if (value is null)
            Write(name);
        else
            Write(name, value.GetValueOrDefault());
    }

    /// <summary>
    /// Writes an attribute name and value pair.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name, long value)
    {
        var byteCount = ValidatedAttributeValue.CountBytes(value);
        var w = new ArrayBuilder<byte>(name.Length + byteCount + 3);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);

            Span<byte> result = stackalloc byte[byteCount];
            ValidatedAttributeValue.ToUtf8(value, result);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes an attribute name and optional value.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">If provided, the value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name, long? value)
    {
        if (value is null)
            Write(name);
        else
            Write(name, value.GetValueOrDefault());
    }

    /// <summary>
    /// Writes an attribute name and value pair.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name, ulong value)
    {
        Write(name);

        Span<byte> result = stackalloc byte[ValidatedAttributeValue.CountBytes(value)];
        ValidatedAttributeValue.ToUtf8(value, result);

        writer.Write(result);
    }

    /// <summary>
    /// Writes an attribute name and value pair.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">The value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name, ulong value)
    {
        var byteCount = ValidatedAttributeValue.CountBytes(value);
        var w = new ArrayBuilder<byte>(name.Length + byteCount + 3);
        try
        {
            ValidatedAttributeName.Validate(name, ref w);

            Span<byte> result = stackalloc byte[byteCount];
            ValidatedAttributeValue.ToUtf8(value, result);

            writer.Write(w.WrittenSpan);
        }
        finally
        {
            w.Release();
        }
    }

    /// <summary>
    /// Writes a validated attribute name and optional value.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">If provided, the value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void Write(ValidatedAttributeName name, ulong? value)
    {
        if (value is null)
            Write(name, ReadOnlySpan<char>.Empty);
        else
            Write(name, value.GetValueOrDefault());
    }

    /// <summary>
    /// Writes an attribute name and optional value.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">If provided, the value to write.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void Write(ReadOnlySpan<char> name, ulong? value)
    {
        if (value is null)
            Write(name, ReadOnlySpan<char>.Empty);
        else
            Write(name, value.GetValueOrDefault());
    }

    /// <summary>
    /// Writes an attribute that consists only of a validated name, no value, if <paramref name="value"/> is true.
    /// </summary>
    /// <param name="name">The validated attribute name to write.</param>
    /// <param name="value">When true, <paramref name="name"/> is written, otherwise false.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> was never initialized.</exception>
    public void WriteIfTrue(ValidatedAttributeName name, bool value)
    {
        if (value)
            Write(name);
    }

    /// <summary>
    /// Writes an attribute that consists only of a name, no value, if <paramref name="value"/> is true.
    /// </summary>
    /// <param name="name">The attribute name to write.</param>
    /// <param name="value">When true, <paramref name="name"/> is written, otherwise false.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public void WriteIfTrue(ReadOnlySpan<char> name, bool value)
    {
        if (value)
            Write(name);
    }
}
