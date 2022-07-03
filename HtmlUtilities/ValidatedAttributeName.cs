using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enables pre-validation of HTML attribute names by storing only valid names.
/// </summary>
public readonly struct ValidatedAttributeName
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="Exception"></exception>
    public ValidatedAttributeName(ReadOnlySpan<byte> name)
    {
        if (!IsValid(name))
            throw new Exception();

        this.value = name.ToArray(); // Copies the source so that it can't be changed after validation.
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="Exception"></exception>
    public ValidatedAttributeName(string name) : this(Encoding.UTF8.GetBytes(name))
    {
    }

    /// <summary>
    /// Determines whether the provided attribute name is valid acccording to HTML5 rules.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>True if <paramref name="name"/> is a valid attribute name, otherwise false.</returns>
    public static bool IsValid(ReadOnlySpan<byte> name)
    {
        // https://html.spec.whatwg.org/#attributes-2

        var enumerator = name.GetEnumerator();
        if (!enumerator.MoveNext())
            return false;

        do
        {
            var c = (char)enumerator.Current;
            // Control codes and specific characters
            if (c is >= '\u0000' and <= '\u001F' or >= '\u007F' and <= '\u009F' or ' ' or '"' or '\'' or '>' or '/' or '=')
                return false;

            // https://infra.spec.whatwg.org/#noncharacter
            // Double-byte non-characters.
            if (c is >= '\uFDD0' and <= '\uFDEF' or '\uFFFE' or '\uFFFF')
                return false;

            // TODO: Block 3-or-more byte non-characters
        } while (enumerator.MoveNext());

        return true;
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="Exception"></exception>
    public static implicit operator ValidatedAttributeName(ReadOnlySpan<byte> name) => new(name);
}
