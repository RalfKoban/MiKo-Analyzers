﻿using System;
using System.Text;

//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers
{
    internal static class StringBuilderCache
    {
        private const int MaxBuilderSize = 1000;
        private const int DefaultCapacity = 16;

        [ThreadStatic]
        private static StringBuilder s_cachedInstance;

        public static StringBuilder Acquire(int capacity = DefaultCapacity)
        {
            if (capacity <= MaxBuilderSize)
            {
                var cachedInstance = s_cachedInstance;

                // only return the cached instance if the requested capacity is less than the currently cached capacity (avoids fragmentation of chunks in case the requested capacity is larger)
                if (cachedInstance != null && capacity <= cachedInstance.Capacity)
                {
                    s_cachedInstance = null;

                    cachedInstance.Clear();

                    return cachedInstance;
                }
            }

            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder builder)
        {
            if (builder.Capacity <= MaxBuilderSize)
            {
                s_cachedInstance = builder;
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