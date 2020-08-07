using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Phrases = CreatePhrases();

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void An_issue_is_reported_for_XML_summary_of_named_type_(string type) => An_issue_is_reported_for(@"

/// <summary>
/// <see cref='bla' />
/// </summary>
public " + type + @" TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_method_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_property_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public int DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_field_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    private int doSomething;
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_event_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2020_InheritdocSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2020_InheritdocSummaryAnalyzer();

        [ExcludeFromCodeCoverage]
        private static string[] CreatePhrases()
        {
            var phrases = new[]
                              {
                                  "<see cref='bla'/>",
                                  "<see cref='bla' />",
                                  "<seealso cref='bla'/>",
                                  "<seealso cref='bla' />",
                              };

            var results = new List<string>(phrases);
            results.AddRange(phrases.Select(_ => _ + "."));
            results.AddRange(phrases.Select(_ => "see " + _));
            results.AddRange(phrases.Select(_ => "see " + _ + "."));
            results.AddRange(phrases.Select(_ => "seealso " + _));
            results.AddRange(phrases.Select(_ => "seealso " + _ + "."));
            results.AddRange(results.Select(_ => _.ToUpper()).ToList());

            results.Sort();

            return results.Distinct().ToArray();
        }
    }
}