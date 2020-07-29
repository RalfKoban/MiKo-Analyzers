using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags =
            {
                "example",
                "exception",
                "note",
                "overloads",
                "para",
                "param",
                "permission",
                "remarks",
                "returns",
                "summary",
                "typeparam",
                "value",
            };

        private static readonly string[] WrongTerms =
            {
                " info ",
                " info,",
                " info;",
                " info.",
                " info:",
                " Info ",
                " Info,",
                " Info;",
                " Info.",
                " Info:",
            };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Guid_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongTerms))] string term) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + term + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_correct_term_in_commented_class_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The information something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer();
    }
}