using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2001_EventSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly IEnumerable<string> StartingPhrases = CreatePhrases();

        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_non_commented_event_on_interface() => No_issue_is_reported_for(@"
public interface TestMe
{
    event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_event_on_class() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Occurs always.
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_event_on_class_with_para_tags() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Occurs always.
    /// </para>
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [TestCase("Occur")]
        [TestCase("The")]
        [TestCase("Whatever that comment means")]
        [TestCase("Invoked if the something changed")]
        [TestCase("Raised at some time")]
        public void An_issue_is_reported_for_wrong_comment_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_empty_comment() => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_inherited_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <inheritdoc />
    public event EventHandler MyEvent;
}
");

        [Test]
        public void Code_gets_fixed_for_event_phrase_([ValueSource(nameof(StartingPhrases))] string originalComment, [Values("after", "before", "when", "for")] string condition)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ### something.
    /// </summary>
    public event EventHandler MyEvent;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment + " " + condition), Template.Replace("###", "Occurs " + condition));
        }

        [Test]
        public void Code_gets_fixed_for_([Values("When", "Indicates that", "Invoked if", "Invoked when")] string original)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ### something.
    /// </summary>
    public event EventHandler MyEvent;
}
";

            VerifyCSharpFix(Template.Replace("###", original), Template.Replace("###", "Occurs when"));
        }

        protected override string GetDiagnosticId() => MiKo_2001_EventSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2001_EventSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2001_CodeFixProvider();

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
        private static HashSet<string> CreatePhrases()
        {
            string[] starts = ["Event", "This event", "The event", "An event", "A event"];
            string[] adverbs = [string.Empty,
                                "is ", "that is ", "which is ",
                                "can be ", "that can be ", "which can be ",
                                "could be ", "that could be ", "which could be ",
                                "shall be ", "that shall be ", "which shall be ",
                                "should be ", "that should be ", "which should be ",
                                "will be ", "that will be ", "which will be ",
                                "would be ", "that would be ", "which would be "];
            string[] verbs = ["fired", "raised", "caused", "triggered", "occurred", "occured"];

            var results = new HashSet<string>();

            foreach (var verb in verbs)
            {
                results.Add(verb.ToUpperCaseAt(0));

                foreach (var adverb in adverbs)
                {
                    var end = string.Concat(adverb, verb);

                    results.Add(end.ToUpperCaseAt(0));

                    foreach (var start in starts)
                    {
                        results.Add(string.Concat(start, " ", end));
                    }
                }
            }

            string[] midTerms = ["to",
                                 "can", "that can", "which can",
                                 "could", "that could", "which could",
                                 "shall", "that shall", "which shall",
                                 "should",  "that should", "which should",
                                 "will", "that will", "which will",
                                 "would", "that would", "which would"];
            string[] verbsInfinite = ["fire", "raise", "cause", "trigger", "occur"];

            foreach (var start in starts)
            {
                foreach (var midTerm in midTerms)
                {
                    var begin = string.Concat(start, " ", midTerm, " ");

                    foreach (var verb in verbsInfinite)
                    {
                        results.Add(string.Concat(begin, verb));
                    }
                }
            }

            string[] verbsPresent = ["fires", "raises", "causes", "triggers", "occurs"];

            foreach (var start in starts)
            {
                foreach (var verb in verbsPresent)
                {
                    results.Add(string.Concat(start, " ", verb));
                }
            }

            return results;
        }
    }
}