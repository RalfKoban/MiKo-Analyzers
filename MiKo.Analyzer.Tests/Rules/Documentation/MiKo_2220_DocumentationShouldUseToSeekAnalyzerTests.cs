﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2220_DocumentationShouldUseToSeekAnalyzerTests : CodeFixVerifier
    {
        private static readonly Dictionary<string, string> Map = new()
                                                                     {
                                                                         { "to find", "to seek" },
                                                                         { "to inspect for ", "to seek for " },
                                                                         { "to look for", "to seek" },
                                                                         { "to test for", "to seek" },

                                                                         // endings with dot
                                                                         { "to find.", "to seek." },
                                                                         { "to inspect for.", "to seek." },
                                                                         { "to look for.", "to seek." },
                                                                         { "to test for.", "to seek." },
                                                                     };

        private static readonly string[] WrongPhrases = [.. Map.Keys];

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
/// It is used " + phrase.Trim() + @" something.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongPhrases))] string phrase)
        {
            const string Template = @"
/// <summary>
/// It is used ### something.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(Template.Replace("###", phrase.Trim()), Template.Replace("###", Map[phrase].Trim()));
        }

        protected override string GetDiagnosticId() => MiKo_2220_DocumentationShouldUseToSeekAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2220_DocumentationShouldUseToSeekAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2220_CodeFixProvider();
    }
}