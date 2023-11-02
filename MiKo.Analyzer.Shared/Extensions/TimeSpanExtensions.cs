// ReSharper disable once CheckNamespace
namespace System
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);
    }
}