using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static partial class StringExtensionsTests
    {
        [Test]
        public static void HumanizedTakeFirst_string_returns_empty_string_for_value() => Assert.That(((string)null).HumanizedTakeFirst(10), Is.Empty);

        [TestCase("", 10, ExpectedResult = "")]
        [TestCase("hello", 10, ExpectedResult = "hello")]
        [TestCase("hello", 5, ExpectedResult = "hello")]
        [TestCase("hello world", 5, ExpectedResult = "hello...")]
        [TestCase("hello world", 0, ExpectedResult = "hello world")]
        [TestCase("hello world", -1, ExpectedResult = "hello world")]
        [TestCase("hello   ", 10, ExpectedResult = "hello")]
        [TestCase("hello   ", 5, ExpectedResult = "hello")]
        [TestCase("hello   ", 3, ExpectedResult = "hel...")]
        [TestCase("hi  there", 4, ExpectedResult = "hi...")]
        public static string HumanizedTakeFirst_string_returns_correct_result_(string value, in int maximum) => value.HumanizedTakeFirst(maximum);

        [TestCase("", 10, ExpectedResult = "")]
        [TestCase("hello", 10, ExpectedResult = "hello")]
        [TestCase("hello", 5, ExpectedResult = "hello")]
        [TestCase("hello world", 5, ExpectedResult = "hello...")]
        [TestCase("hello world", 0, ExpectedResult = "hello world")]
        [TestCase("hello world", -1, ExpectedResult = "hello world")]
        [TestCase("hello   ", 10, ExpectedResult = "hello")]
        [TestCase("hello   ", 5, ExpectedResult = "hello")]
        [TestCase("hello   ", 3, ExpectedResult = "hel...")]
        [TestCase("hi  there", 4, ExpectedResult = "hi...")]
        public static string HumanizedTakeFirst_ReadOnlySpan_returns_correct_result_(string value, in int maximum) => ((System.ReadOnlySpan<char>)value).HumanizedTakeFirst(maximum);
    }
}
