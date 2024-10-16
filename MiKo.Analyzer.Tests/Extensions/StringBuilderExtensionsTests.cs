using System;
using System.Text;

using MiKoSolutions.Analyzers.Linguistics;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringBuilderExtensionsTests
    {
        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = "")]
        [TestCase("  ", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "")]
        [TestCase("This is a test", ExpectedResult = "This is a test")]
        [TestCase("This is a test     ", ExpectedResult = "This is a test")]
        [TestCase("    This is a test", ExpectedResult = "This is a test")]
        [TestCase("    This is a test     ", ExpectedResult = "This is a test")]
        public static string Trimmed_trims_string_(string s) => new StringBuilder(s).Trimmed().ToString();

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

        [TestCase("", FirstWordHandling.KeepLeadingSpace, "")]
        [TestCase(" ", FirstWordHandling.KeepLeadingSpace, " ")]
        [TestCase("  ", FirstWordHandling.KeepLeadingSpace, " ")]
        [TestCase("   ", FirstWordHandling.KeepLeadingSpace, " ")]
        [TestCase("This is a test", FirstWordHandling.KeepLeadingSpace, "This is a test")]
        [TestCase(" This is a test", FirstWordHandling.KeepLeadingSpace, " This is a test")]
        [TestCase("   This is a test", FirstWordHandling.KeepLeadingSpace, " This is a test")]
        [TestCase("This is a test", FirstWordHandling.MakeLowerCase, "this is a test")]
        [TestCase("this is a test", FirstWordHandling.MakeLowerCase, "this is a test")]
        [TestCase(" THis is a Test", FirstWordHandling.MakeLowerCase, "tHis is a Test")]
        [TestCase("   THis is a Test", FirstWordHandling.MakeLowerCase, "tHis is a Test")]
        [TestCase(" THis is a Test", FirstWordHandling.MakeLowerCase | FirstWordHandling.KeepLeadingSpace, " tHis is a Test")]
        [TestCase("   THis is a Test", FirstWordHandling.MakeLowerCase | FirstWordHandling.KeepLeadingSpace, " tHis is a Test")]
        [TestCase("This is a test", FirstWordHandling.MakeUpperCase, "This is a test")]
        [TestCase("this is a test", FirstWordHandling.MakeUpperCase, "This is a test")]
        [TestCase(" this is a test", FirstWordHandling.MakeUpperCase, "This is a test")]
        [TestCase("   this is a test", FirstWordHandling.MakeUpperCase, "This is a test")]
        [TestCase("this is a test", FirstWordHandling.MakeUpperCase | FirstWordHandling.KeepLeadingSpace, "This is a test")]
        [TestCase(" this is a test", FirstWordHandling.MakeUpperCase | FirstWordHandling.KeepLeadingSpace, " This is a test")]
        [TestCase("   this is a test", FirstWordHandling.MakeUpperCase | FirstWordHandling.KeepLeadingSpace, " This is a test")]
        [TestCase("represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase("   represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represents someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent someone")]
        [TestCase("   represents someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent someone")]
        [TestCase("represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, "represent")]
        [TestCase(" represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase(" represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent")]
        [TestCase("   represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("   represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent")]
        [TestCase("represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase("   represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represent someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent someone")]
        [TestCase("   represent someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent someone")]
        [TestCase("represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, "represent")]
        [TestCase(" represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase(" represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent")]
        [TestCase("   represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("   represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepLeadingSpace, " represent")]
        [TestCase("message", FirstWordHandling.MakePlural, "messages")]
        [TestCase("message", FirstWordHandling.MakePlural | FirstWordHandling.KeepLeadingSpace, "messages")]
        [TestCase(" message", FirstWordHandling.MakePlural | FirstWordHandling.KeepLeadingSpace, " messages")]
        [TestCase("   message", FirstWordHandling.MakePlural | FirstWordHandling.KeepLeadingSpace, " messages")]
        public static void AdjustFirstWordHandling(string s, FirstWordHandling handling, string expectedResult)
        {
            var resultFromSB = new StringBuilder(s).AdjustFirstWord(handling).ToString();
            var resultFromS = s.AdjustFirstWord(handling);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(resultFromSB, Is.EqualTo(expectedResult), "Bug in StringBuilder extension");
                                     Assert.That(resultFromS, Is.EqualTo(expectedResult), "Bug in String extension");
                                 });
        }
    }
}