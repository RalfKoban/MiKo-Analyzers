// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Encapsulates a method that transforms an enumerator entry into a <see cref="string"/>.
    /// </summary>
    /// <param name="entry">
    /// The entry to transform.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> that contains the result of the transformation.
    /// </returns>
    internal delegate string WordsReadOnlySpanEnumeratorSelectDelegate(ReadOnlySpanEnumeratorEntry entry);
}