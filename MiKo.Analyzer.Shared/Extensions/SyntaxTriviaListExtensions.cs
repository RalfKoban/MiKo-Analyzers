using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.CodeAnalysis
{
    public static class SyntaxTriviaListExtensions
    {
        /// <summary>
        /// Determines whether all elements in the <see cref="SyntaxTriviaList"/> satisfy the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool All(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (filter(value[index]) is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether any element in the <see cref="SyntaxTriviaList"/> satisfies the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (filter(value[index]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Counts the number of elements in the specified <see cref="SyntaxTriviaList"/> that satisfy the given condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The number of elements that satisfy the condition.
        /// </returns>
        internal static int Count(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount <= 0)
            {
                return 0;
            }

            var count = 0;

            for (var index = 0; index < valueCount; index++)
            {
                if (filter(value[index]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxTriviaList"/> contains no elements.
        /// </summary>
        /// <param name="source">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTriviaList source) => source.Count is 0;

        /// <summary>
        /// Filters the specified <see cref="SyntaxTriviaList"/> and returns a sequence of syntax trivia that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The list of syntax trivia to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax trivia against.
        /// </param>
        /// <returns>
        /// A sequence of syntax trivia that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> Where(this SyntaxTriviaList source, Func<SyntaxTrivia, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var value = source[index];

                    if (predicate(value))
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Filters the specified <see cref="SyntaxTriviaList.Reversed"/> and returns a sequence of syntax trivia that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The reversed list of syntax trivia to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax trivia against.
        /// </param>
        /// <returns>
        /// A sequence of syntax trivia that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> Where(this SyntaxTriviaList.Reversed source, Func<SyntaxTrivia, bool> predicate)
        {
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    yield return value;
                }
            }
        }
    }
}