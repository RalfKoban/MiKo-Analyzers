// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace System
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);
    }
}