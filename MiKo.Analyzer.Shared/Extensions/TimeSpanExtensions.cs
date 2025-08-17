using System;
using System.Runtime.CompilerServices;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class TimeSpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Milliseconds(this in int value) => TimeSpan.FromMilliseconds(value);
    }
}