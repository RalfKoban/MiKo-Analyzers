// ncrunch: rdi off
// ReSharper disable once CheckNamespace

using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace System
{
    internal static class TimeSpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Milliseconds(this in int value) => TimeSpan.FromMilliseconds(value);
    }
}