using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Defines values that specify how to handle the first word of a comment.
    /// </summary>
    [Flags]
    public enum FirstWordHandling
    {
        /// <summary>
        /// Keep it, do NOT touch it.
        /// </summary>
        None = 0,

        /// <summary>
        /// Keep a single leading space of the word if there is any.
        /// Trims multiple leading spaces to one leading space.
        /// Does not add a leading space in case there is none.
        /// </summary>
        KeepSingleLeadingSpace = 1 << 0,

        /// <summary>
        /// Attempt to make the word starting with an upper case.
        /// </summary>
        StartUpperCase = 1 << 1,

        /// <summary>
        /// Attempt to make the word starting with a lower case.
        /// </summary>
        StartLowerCase = 1 << 2,

        /// <summary>
        /// Attempt to make it an infinite verb.
        /// </summary>
        MakeInfinite = 1 << 3,

        /// <summary>
        /// Attempt to make it a plural verb.
        /// </summary>
        MakePlural = 1 << 4,

        /// <summary>
        /// Attempt to make it a 3rd person singular verb.
        /// </summary>
        MakeThirdPersonSingular = 1 << 5,
    }
}