using System;
using System.Runtime.CompilerServices;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="TimeSpan"/>s.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Gets a <see cref="TimeSpan"/> that represents the specified number of milliseconds.
        /// </summary>
        /// <param name="value">
        /// The milliseconds value to be converted to a <see cref="TimeSpan"/>.
        /// </param>
        /// <returns>
        /// A <see cref="TimeSpan"/> that represents the specified number of milliseconds.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Milliseconds(this in int value) => TimeSpan.FromMilliseconds(value);
    }
}