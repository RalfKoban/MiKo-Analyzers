using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="FirstWordAdjustment"/>.
    /// </summary>
    internal static class FirstWordAdjustmentExtensions
    {
        /// <summary>
        /// Determines whether the specified bit is set in the given value.
        /// </summary>
        /// <param name="value">
        /// A bitwise combination of the enumeration members that specifies the value to check.
        /// </param>
        /// <param name="flag">
        /// A bitwise combination of the enumeration members that specifies the value to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified bit is set in the value; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSet(this FirstWordAdjustment value, in FirstWordAdjustment flag) => (value & flag) == flag;
    }
}