using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2221_DocumentationIsNotEmptyAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Tags =
            {
                Constants.XmlTag.Code,
                Constants.XmlTag.Example,
                Constants.XmlTag.Exception,
                Constants.XmlTag.Overloads,
                Constants.XmlTag.Remarks,
                Constants.XmlTag.Returns,
                Constants.XmlTag.Summary,
            };

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment([ValueSource(nameof(Tags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// Some summary.
/// </" + tag + @">
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_empty_XML_in_documentation_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
/// <" + tag + @"/>
public class TestMe
{
}");

        protected override string GetDiagnosticId() => MiKo_2221_DocumentationIsNotEmptyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2221_DocumentationIsNotEmptyAnalyzer();
    }
}