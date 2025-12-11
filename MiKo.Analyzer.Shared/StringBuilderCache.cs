using System;
using System.Text;

//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers
{
    internal static class StringBuilderCache
    {
        public const int DefaultCapacity = 128;
        private const int MaxBuilderSize = 1024;

        [ThreadStatic]
        private static StringBuilder s_cachedInstance1;

        [ThreadStatic]
        private static StringBuilder s_cachedInstance2;

        public static StringBuilder Acquire(in int capacity = DefaultCapacity)
        {
            if (capacity <= MaxBuilderSize)
            {
                var instance1 = s_cachedInstance1;

                // only return the cached instance if the requested capacity is less than the currently cached capacity (avoids fragmentation of chunks in case the requested capacity is larger)
                if (instance1 != null && capacity <= instance1.Capacity)
                {
                    s_cachedInstance1 = null;

                    instance1.Length = 0; // reset length and clear builder

                    return instance1;
                }

                var instance2 = s_cachedInstance2;

                if (instance2 != null && capacity <= instance2.Capacity)
                {
                    s_cachedInstance2 = null;

                    instance2.Length = 0; // reset length and clear builder

                    return instance2;
                }
            }

            return new StringBuilder(Math.Max(capacity, DefaultCapacity));
        }

        public static void Release(StringBuilder builder)
        {
            if (builder.Capacity > MaxBuilderSize)
            {
                return;
            }

            if (s_cachedInstance1 is null)
            {
                s_cachedInstance1 = builder;
            }
            else if (s_cachedInstance2 is null)
            {
                s_cachedInstance2 = builder;
            }
        }

        public static string GetStringAndRelease(StringBuilder builder)
        {
            var result = builder.ToString();

            Release(builder);

            return result;
        }
    }
}