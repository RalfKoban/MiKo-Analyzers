// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);
    }
}