using System;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringExtensionsTests
    {
        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase("a", ExpectedResult = "a")]
        [TestCase("a1", ExpectedResult = "a")]
        [TestCase("ab", ExpectedResult = "ab")]
        [TestCase("ab1", ExpectedResult = "ab")]
        [TestCase("abc", ExpectedResult = "abc")]
        [TestCase("abc42", ExpectedResult = "abc")]
        [TestCase("a1bc", ExpectedResult = "a1bc")]
        [TestCase("a1bc42", ExpectedResult = "a1bc")]
        public static string WithoutNumberSuffix_works_(string input) => input.WithoutNumberSuffix();

        [TestCase("ab123", ExpectedResult = true)]
        [TestCase("ab1", ExpectedResult = true)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = false)]
        [TestCase("1", ExpectedResult = true)]
        public static bool EndsWithNumber_works_(string input) => input.EndsWithNumber();

        [TestCase("ab123", ExpectedResult = false)]
        [TestCase("ab1", ExpectedResult = false)]
        [TestCase("ab", ExpectedResult = false)]
        [TestCase("a8b", ExpectedResult = false)]
        [TestCase("42ab", ExpectedResult = true)]
        [TestCase("1ab", ExpectedResult = true)]
        [TestCase("1", ExpectedResult = true)]
        public static bool StartsWithNumber_works_(string input) => input.StartsWithNumber();
    }
}