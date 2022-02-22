namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Specifies how to handle the first word of a comment.
    /// </summary>
    public enum FirstWordHandling
    {
        /// <summary>
        /// Keep it, do NOT touch it.
        /// </summary>
        None = 0,

        /// <summary>
        /// Attempt to make it an infinite verb.
        /// </summary>
        MakeInfinite = 1,
    }
}