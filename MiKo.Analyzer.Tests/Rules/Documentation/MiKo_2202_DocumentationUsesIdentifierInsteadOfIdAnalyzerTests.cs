
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzerTests : CodeFixVerifier
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

        private static readonly string[] WrongIds =
            {
                " id ",
                " id,",
                " id;",
                " id.",
                " id:",
                " id ",
                " id,",
                " id;",
                " id.",
                " id:",
            };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Id_in_Xml_tag([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongIds))] string id) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + id + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_correct_term_in_commented_class([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer();
    }
}