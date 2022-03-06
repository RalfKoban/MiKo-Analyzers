﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2218_DocumentationShouldNotContainUsedToAnalyzerTests : CodeFixVerifier
    {
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
                                                                     {
                                                                         { "can be used to", "allows to" },
                                                                         { "that is used to", "to" },
                                                                         { "that it is used to", "to" },
                                                                         { "that are used to", "to" },
                                                                         { "that shall be used to", "to" },
                                                                         { "which is used to", "to" },
                                                                         { "which are used to", "to" },
                                                                         { "which shall be used to", "to" },
                                                                     };

        private static readonly string[] WrongPhrases = Map.Keys.ToArray();

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_text_in_documentation_([ValueSource(nameof(WrongPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// It " + phrase + @" do something.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongPhrases))] string phrase)
        {
            const string Template = @"
/// <summary>
/// It ### do something.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(Template.Replace("###", phrase), Template.Replace("###", Map[phrase]));
        }

        [TestCase("Callback used to analyze stuff.", "Callback to analyze stuff.")]
        [TestCase("Can be used to analyze stuff.", "Allows to analyze stuff.")]
        [TestCase("Does something. It is used to analyze stuff. Performs something more.", "Does something. It analyzes stuff. Performs something more.")]
        [TestCase("Does something. Used to analyze stuff. Performs something more.", "Does something. Analyzes stuff. Performs something more.")]
        [TestCase("Does something. Used to analyze stuff.", "Does something. Analyzes stuff.")]
        [TestCase("It can be used to analyze stuff.", "It allows to analyze stuff.")]
        [TestCase("Used to analyze stuff.", "Analyzes stuff.")]
        public void Code_gets_fixed_for_special_case_text_(string originalCode, string fixedCode)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    public void DoSomething()
    {
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2218_CodeFixProvider();
    }
}