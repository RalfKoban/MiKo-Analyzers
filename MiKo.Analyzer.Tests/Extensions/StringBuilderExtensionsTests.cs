﻿using System;
using System.Text;

using MiKoSolutions.Analyzers.Linguistics;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringBuilderExtensionsTests
    {
        [TestCase("", null, StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase("", "test", StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase("Something", "test", StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase("test", " test", StringComparison.Ordinal, ExpectedResult = false)]
        [TestCase("test", "test", StringComparison.Ordinal, ExpectedResult = true)]
        [TestCase(" test", "test", StringComparison.Ordinal, ExpectedResult = true)]
        [TestCase("Some_test", "test", StringComparison.Ordinal, ExpectedResult = true)]
        public static bool EndsWith_detects_ending_(string builderValue, string ending, in StringComparison comparison) => new StringBuilder(builderValue).EndsWith(ending, comparison);

        [TestCase("", -1)]
        [TestCase("", 1)]
        public static void TrimEndBy_throws_ArgumentOutOfRangeException_for_(string s, int count) => Assert.That(() => new StringBuilder(s).TrimEndBy(count), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());

        [TestCase("", 0, ExpectedResult = "")]
        [TestCase("test", 0, ExpectedResult = "test")]
        [TestCase("test", 1, ExpectedResult = "tes")]
        [TestCase("test", 2, ExpectedResult = "te")]
        [TestCase("test", 3, ExpectedResult = "t")]
        [TestCase("test", 4, ExpectedResult = "")]
        public static string TrimEndBy_trims_string_(string s, in int count) => new StringBuilder(s).TrimEndBy(count).ToString();

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

        [TestCase("", FirstWordHandling.KeepSingleLeadingSpace, "")]
        [TestCase(" ", FirstWordHandling.KeepSingleLeadingSpace, " ")]
        [TestCase("  ", FirstWordHandling.KeepSingleLeadingSpace, " ")]
        [TestCase("   ", FirstWordHandling.KeepSingleLeadingSpace, " ")]
        [TestCase("This is a test", FirstWordHandling.KeepSingleLeadingSpace, "This is a test")]
        [TestCase(" This is a test", FirstWordHandling.KeepSingleLeadingSpace, " This is a test")]
        [TestCase("   This is a test", FirstWordHandling.KeepSingleLeadingSpace, " This is a test")]
        [TestCase("This is a test", FirstWordHandling.StartLowerCase, "this is a test")]
        [TestCase("this is a test", FirstWordHandling.StartLowerCase, "this is a test")]
        [TestCase(" THis is a Test", FirstWordHandling.StartLowerCase, "tHis is a Test")]
        [TestCase("   THis is a Test", FirstWordHandling.StartLowerCase, "tHis is a Test")]
        [TestCase(" THis is a Test", FirstWordHandling.StartLowerCase | FirstWordHandling.KeepSingleLeadingSpace, " tHis is a Test")]
        [TestCase("   THis is a Test", FirstWordHandling.StartLowerCase | FirstWordHandling.KeepSingleLeadingSpace, " tHis is a Test")]
        [TestCase("This is a test", FirstWordHandling.StartUpperCase, "This is a test")]
        [TestCase("this is a test", FirstWordHandling.StartUpperCase, "This is a test")]
        [TestCase(" this is a test", FirstWordHandling.StartUpperCase, "This is a test")]
        [TestCase("   this is a test", FirstWordHandling.StartUpperCase, "This is a test")]
        [TestCase("this is a test", FirstWordHandling.StartUpperCase | FirstWordHandling.KeepSingleLeadingSpace, "This is a test")]
        [TestCase(" this is a test", FirstWordHandling.StartUpperCase | FirstWordHandling.KeepSingleLeadingSpace, " This is a test")]
        [TestCase("   this is a test", FirstWordHandling.StartUpperCase | FirstWordHandling.KeepSingleLeadingSpace, " This is a test")]
        [TestCase("represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase("   represents someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represents someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent someone")]
        [TestCase("   represents someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent someone")]
        [TestCase("represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, "represent")]
        [TestCase(" represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase(" represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent")]
        [TestCase("   represents", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("   represents", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent")]
        [TestCase("represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase("   represent someone", FirstWordHandling.MakeInfinite, "represent someone")]
        [TestCase(" represent someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent someone")]
        [TestCase("   represent someone", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent someone")]
        [TestCase("represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, "represent")]
        [TestCase(" represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase(" represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent")]
        [TestCase("   represent", FirstWordHandling.MakeInfinite, "represent")]
        [TestCase("   represent", FirstWordHandling.MakeInfinite | FirstWordHandling.KeepSingleLeadingSpace, " represent")]
        [TestCase("message", FirstWordHandling.MakePlural, "messages")]
        [TestCase("message", FirstWordHandling.MakePlural | FirstWordHandling.KeepSingleLeadingSpace, "messages")]
        [TestCase(" message", FirstWordHandling.MakePlural | FirstWordHandling.KeepSingleLeadingSpace, " messages")]
        [TestCase("   message", FirstWordHandling.MakePlural | FirstWordHandling.KeepSingleLeadingSpace, " messages")]

        [TestCase("Abc", FirstWordHandling.StartLowerCase, "abc")] // just as clarification, situation is tested by other tests as well
        [TestCase("abc", FirstWordHandling.StartLowerCase, "abc")] // just as clarification, situation is tested by other tests as well
        [TestCase("ABC", FirstWordHandling.StartLowerCase, "aBC")] // just as clarification, situation is tested by other tests as well
        [TestCase("aBC", FirstWordHandling.StartLowerCase, "aBC")] // just as clarification, situation is tested by other tests as well
        [TestCase("AbC", FirstWordHandling.StartLowerCase, "abC")] // just as clarification, situation is tested by other tests as well
        [TestCase("abC", FirstWordHandling.StartLowerCase, "abC")] // just as clarification, situation is tested by other tests as well

        [TestCase("Abc", FirstWordHandling.StartUpperCase, "Abc")] // just as clarification, situation is tested by other tests as well
        [TestCase("abc", FirstWordHandling.StartUpperCase, "Abc")] // just as clarification, situation is tested by other tests as well
        [TestCase("ABC", FirstWordHandling.StartUpperCase, "ABC")] // just as clarification, situation is tested by other tests as well
        [TestCase("aBC", FirstWordHandling.StartUpperCase, "ABC")] // just as clarification, situation is tested by other tests as well
        [TestCase("AbC", FirstWordHandling.StartUpperCase, "AbC")] // just as clarification, situation is tested by other tests as well
        [TestCase("abC", FirstWordHandling.StartUpperCase, "AbC")] // just as clarification, situation is tested by other tests as well
        public static void AdjustFirstWordHandling(string s, in FirstWordHandling handling, string expectedResult)
        {
            var resultFromSB = new StringBuilder(s).AdjustFirstWord(handling).ToString();
            var resultFromS = s.AdjustFirstWord(handling);

            Assert.Multiple(() =>
                                 {
                                     Assert.That(resultFromSB, Is.EqualTo(expectedResult), "Bug in StringBuilder extension");
                                     Assert.That(resultFromS, Is.EqualTo(expectedResult), "Bug in String extension");
                                 });
        }

        [TestCase("", ExpectedResult = "")]
        [TestCase("SomeValue", ExpectedResult = "Some_value")]
        [TestCase("SomeValueWithNumber1234", ExpectedResult = "Some_value_with_number_1234")]
        [TestCase("SomeValueWithNumber1234InBetween", ExpectedResult = "Some_value_with_number_1234_in_between")]
        [TestCase("SomeValueWithNumber1234AfterWhatever", ExpectedResult = "Some_value_with_number_1234_after_whatever")]
        public static string SeparateWords_separates_words_at_underscores_(string s) => new StringBuilder(s).SeparateWords(Constants.Underscore).ToString();

        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = " ")]
        [TestCase("  ", ExpectedResult = " ")]
        [TestCase("   ", ExpectedResult = " ")]
        [TestCase("    ", ExpectedResult = " ")]
        [TestCase("a    ", ExpectedResult = "a ")]
        [TestCase("a    b", ExpectedResult = "a b")]
        public static string WithoutMultipleWhiteSpaces_shortens_multiple_whitespaces_to_single_(string s) => new StringBuilder(s).WithoutMultipleWhiteSpaces().ToString();

        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices", "DoEvents", "##Events", ExpectedResult = "CallsDownloadWorkflowForMultipleParameterDownloadDevices")]
        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices", "Download", "My", ExpectedResult = "CallsMyWorkflowForMultipleParameterMyDevices")]
        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices", "Workflow", "#", ExpectedResult = "CallsDownload#ForMultipleParameterDownloadDevices")]
        public static string ReplaceWithProbe_above_threshold_uses_arrays_properly_to_adjust_(string start, string name, string other) => new StringBuilder(start).ReplaceWithProbe(name, other).ToString();
    }
}