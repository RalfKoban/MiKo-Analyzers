namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Defines values that specify the type of boundary used to identify word breaks when enumerating words.
    /// </summary>
    /// <seealso cref="WordsReadOnlySpanEnumerator"/>
    public enum WordBoundary
    {
        /// <summary>
        /// No word boundary is specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Word boundaries occur at uppercase characters, treating each uppercase letter as the start of a new word.
        /// </summary>
        UpperCaseCharacters = 1,

        /// <summary>
        /// Word boundaries occur at whitespace characters, splitting words wherever spaces, tabs, or other whitespace appears.
        /// </summary>
        WhiteSpaces = 2,
    }
}