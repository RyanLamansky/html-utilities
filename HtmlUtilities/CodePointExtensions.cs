using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enables additional scenarios for use of <see cref="CodePoint"/> in conjunction with other types.
/// </summary>
public static class CodePointExtensions
{
    /// <summary>
    /// Converts the provided <see cref="CodePoint"/> sequence into UTF-16 <see cref="char"/>s and appends them to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="destination">Receives the characters.</param>
    /// <param name="source">The code points to apppend.</param>
    /// <returns><paramref name="destination"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">All parameters are required.</exception>
    public static StringBuilder Append(this StringBuilder destination, IEnumerable<CodePoint> source)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(source);

        foreach (var c in CodePoint.EncodeUtf16(source))
            destination.Append(c);

        return destination;
    }

    /// <summary>
    /// Converts the provided <see cref="CodePoint"/> sequence into UTF-16 <see cref="char"/>s and appends them to a new <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="source">The code points to include in the result.</param>
    /// <returns>A <see cref="StringBuilder"/> containing the characters.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> cannot be null.</exception>
    public static StringBuilder ToStringBuilder(this IEnumerable<CodePoint> source) => new StringBuilder().Append(source);

    /// <summary>
    /// Converts the provided <see cref="CodePoint"/> sequence into a <see cref="string"/>.
    /// </summary>
    /// <param name="source">The code points that will form the result.</param>
    /// <returns>A <see cref="string"/> representation of <paramref name="source"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> cannot be null.</exception>
    public static string ToString(this IEnumerable<CodePoint> source) => ToStringBuilder(source).ToString();
}
