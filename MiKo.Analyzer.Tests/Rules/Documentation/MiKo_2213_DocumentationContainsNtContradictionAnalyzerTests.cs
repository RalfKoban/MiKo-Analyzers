using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2213_DocumentationContainsNtContradictionAnalyzerTests : CodeFixVerifier
    {
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
                                                                     {
                                                                         { "aren't", "are not" },
                                                                         { "can't", "cannot" },
                                                                         { "couldn't", "could not" },
                                                                         { "daren't", "dare not" },
                                                                         { "didn't", "did not" },
                                                                         { "doesn't", "does not" },
                                                                         { "don't", "do not" },
                                                                         { "hadn't", "had not" },
                                                                         { "hasn't", "has not" },
                                                                         { "haven't", "have not" },
                                                                         { "isn't", "is not" },
                                                                         { "needn't", "need not" },
                                                                         { "shouldn't", "should not" },
                                                                         { "wasn't", "was not" },
                                                                         { "weren't", "were not" },
                                                                         { "won't", "will not" },
                                                                         { "wouldn't", "would not" },

                                                                         // capitalized
                                                                         { "Aren't", "Are not" },
                                                                         { "Can't", "Cannot" },
                                                                         { "Couldn't", "Could not" },
                                                                         { "Daren't", "Dare not" },
                                                                         { "Didn't", "Did not" },
                                                                         { "Doesn't", "Does not" },
                                                                         { "Don't", "Do not" },
                                                                         { "Hadn't", "Had not" },
                                                                         { "Hasn't", "Has not" },
                                                                         { "Haven't", "Have not" },
                                                                         { "Isn't", "Is not" },
                                                                         { "Needn't", "Need not" },
                                                                         { "Shouldn't", "Should not" },
                                                                         { "Wasn't", "Was not" },
                                                                         { "Weren't", "Were not" },
                                                                         { "Won't", "Will not" },
                                                                         { "Wouldn't", "Would not" },
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
        public void An_issue_is_reported_for_contradiction_in_documentation_([ValueSource(nameof(WrongPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// This " + phrase + @" intended.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongPhrases))] string phrase)
        {
            const string Template = @"
/// <summary>
/// This ### intended.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(Template.Replace("###", phrase), Template.Replace("###", Map[phrase]));
        }

        protected override string GetDiagnosticId() => MiKo_2213_DocumentationContainsNtContradictionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2213_DocumentationContainsNtContradictionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2213_CodeFixProvider();
    }
}