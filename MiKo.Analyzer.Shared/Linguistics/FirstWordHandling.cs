using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Defines values that specify how to handle the first word of a comment.
    /// </summary>
    [Flags]
    public enum FirstWordHandling : ushort
    {
        /// <summary>
        /// Keep it, do NOT touch it.
        /// </summary>
        None = 0,

        /// <summary>
        /// Keep the leading space of the word if there is any.
        /// </summary>
        KeepLeadingSpace = 1 << 0,

        /// <summary>
        /// Attempt to make the word starting with an upper case.
        /// </summary>
        MakeUpperCase = 1 << 1,

        /// <summary>
        /// Attempt to make the word starting with a lower case.
        /// </summary>
        MakeLowerCase = 1 << 2,

        /// <summary>
        /// Attempt to make it an infinite verb.
        /// </summary>
        MakeInfinite = 1 << 3,

        /// <summary>
        /// Attempt to make it a plural verb.
        /// </summary>
        MakePlural = 1 << 4,
    }
}