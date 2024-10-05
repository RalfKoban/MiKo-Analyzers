using System;
using System.Text;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringBuilderExtensionsTests
    {
        private const int KeepLeadingSpace = 1 << 0;
        private const int MakeUpperCase = 1 << 1;
        private const int MakeLowerCase = 1 << 2;
        private const int MakeInfinite = 1 << 3;
        private const int MakePlural = 1 << 4;

        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = "")]
        [TestCase("  ", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "")]
        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test")]
        [TestCase("    This is a test", ExpectedResult = "This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "This is a test")]
        public static string Trim_trims_string_(string s) => new StringBuilder(s).Trim();

        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = "")]
        [TestCase("  ", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "")]
        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test     ")]
        [TestCase("    This is a test", ExpectedResult = "This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "This is a test     ")]
        public static string TrimStart_trims_string_at_start_(string s) => new StringBuilder(s).TrimStart();

        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = "")]
        [TestCase("  ", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "")]
        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test")]
        [TestCase("    This is a test", ExpectedResult = "    This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "    This is a test")]
        public static string TrimEnd_trims_string_at_end_(string s) => new StringBuilder(s).TrimEnd();

        [TestCase("", KeepLeadingSpace, "")]
        [TestCase(" ", KeepLeadingSpace, " ")]
        [TestCase("  ", KeepLeadingSpace, " ")]
        [TestCase("   ", KeepLeadingSpace, " ")]
        [TestCase("This is a test", KeepLeadingSpace, "This is a test")]
        [TestCase(" This is a test", KeepLeadingSpace, " This is a test")]
        [TestCase("   This is a test", KeepLeadingSpace, " This is a test")]
        [TestCase("This is a test", MakeLowerCase, "this is a test")]
        [TestCase("this is a test", MakeLowerCase, "this is a test")]
        [TestCase(" THis is a Test", MakeLowerCase, "tHis is a Test")]
        [TestCase("   THis is a Test", MakeLowerCase, "tHis is a Test")]
        [TestCase(" THis is a Test", MakeLowerCase | KeepLeadingSpace, " tHis is a Test")]
        [TestCase("   THis is a Test", MakeLowerCase | KeepLeadingSpace, " tHis is a Test")]
        [TestCase("This is a test", MakeUpperCase, "This is a test")]
        [TestCase("this is a test", MakeUpperCase, "This is a test")]
        [TestCase(" this is a test", MakeUpperCase, "This is a test")]
        [TestCase("   this is a test", MakeUpperCase, "This is a test")]
        [TestCase("this is a test", MakeUpperCase | KeepLeadingSpace, "This is a test")]
        [TestCase(" this is a test", MakeUpperCase | KeepLeadingSpace, " This is a test")]
        [TestCase("   this is a test", MakeUpperCase | KeepLeadingSpace, " This is a test")]
        [TestCase("represents someone", MakeInfinite, "represent someone")]
        [TestCase(" represents someone", MakeInfinite, "represent someone")]
        [TestCase("   represents someone", MakeInfinite, "represent someone")]
        [TestCase(" represents someone", MakeInfinite | KeepLeadingSpace, " represent someone")]
        [TestCase("   represents someone", MakeInfinite | KeepLeadingSpace, " represent someone")]
        [TestCase("represents", MakeInfinite, "represent")]
        [TestCase("represents", MakeInfinite | KeepLeadingSpace, "represent")]
        [TestCase(" represents", MakeInfinite, "represent")]
        [TestCase(" represents", MakeInfinite | KeepLeadingSpace, " represent")]
        [TestCase("   represents", MakeInfinite, "represent")]
        [TestCase("   represents", MakeInfinite | KeepLeadingSpace, " represent")]
        [TestCase("represent someone", MakeInfinite, "represent someone")]
        [TestCase(" represent someone", MakeInfinite, "represent someone")]
        [TestCase("   represent someone", MakeInfinite, "represent someone")]
        [TestCase(" represent someone", MakeInfinite | KeepLeadingSpace, " represent someone")]
        [TestCase("   represent someone", MakeInfinite | KeepLeadingSpace, " represent someone")]
        [TestCase("represent", MakeInfinite, "represent")]
        [TestCase("represent", MakeInfinite | KeepLeadingSpace, "represent")]
        [TestCase(" represent", MakeInfinite, "represent")]
        [TestCase(" represent", MakeInfinite | KeepLeadingSpace, " represent")]
        [TestCase("   represent", MakeInfinite, "represent")]
        [TestCase("   represent", MakeInfinite | KeepLeadingSpace, " represent")]
        [TestCase("message", MakePlural, "messages")]
        [TestCase("message", MakePlural | KeepLeadingSpace, "messages")]
        [TestCase(" message", MakePlural | KeepLeadingSpace, " messages")]
        [TestCase("   message", MakePlural | KeepLeadingSpace, " messages")]
        public static void AdjustFirstWordHandling(string s, int handling, string expectedResult)
        {
            var builder = new StringBuilder(s);
            var builderMethod = typeof(StringBuilderExtensions).GetMethod(nameof(StringBuilderExtensions.AdjustFirstWord));
            var stringMethod = typeof(StringExtensions).GetMethod(nameof(StringExtensions.AdjustFirstWord));

            var resultFromSB = builderMethod?.Invoke(null, [builder, (ushort)handling]) as string;
            var resultFromS = stringMethod?.Invoke(null, [s, (ushort)handling]) as string;

            Assert.Multiple(() =>
                                 {
                                     Assert.That(resultFromSB, Is.EqualTo(expectedResult), "Bug in StringBuilder extension");
                                     Assert.That(resultFromS, Is.EqualTo(expectedResult), "Bug in String extension");
                                 });
        }
    }
}