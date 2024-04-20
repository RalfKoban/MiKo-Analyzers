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
        public static string ToStringTrimmed_trims_string_(string s) => new StringBuilder(s).ToStringTrimmed();
    }
}