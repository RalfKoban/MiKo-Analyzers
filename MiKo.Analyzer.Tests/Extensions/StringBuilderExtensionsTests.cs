using System.Text;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringBuilderExtensionsTests
    {
        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test")]
        [TestCase("    This is a test", ExpectedResult = "This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "This is a test")]
        public static string Trim_trims_string_(string s) => new StringBuilder(s).Trim();

        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test     ")]
        [TestCase("    This is a test", ExpectedResult = "This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "This is a test     ")]
        public static string TrimStart_trims_string_at_start_(string s) => new StringBuilder(s).TrimStart();

        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test")]
        [TestCase("    This is a test", ExpectedResult = "    This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "    This is a test")]
        public static string TrimEnd_trims_string_at_end_(string s) => new StringBuilder(s).TrimEnd();
    }
}