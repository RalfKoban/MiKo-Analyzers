using System;

using Microsoft.CodeAnalysis.CodeFixes;
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
                " Id ",
                " Id,",
                " Id;",
                " Id.",
                " Id:",
                " ID ",
                " ID,",
                " ID;",
                " ID.",
                " ID:",
            };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Id_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongIds))] string id) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correct_term_in_commented_class_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identifier something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_type_([ValueSource(nameof(WrongIds))] string wrongText)
        {
            const string Template = @"
/// <summary>
/// The ### something.
/// </summary>
public sealed class TestMe { }
";

            var correctText = wrongText.Replace("id", "identifier", StringComparison.OrdinalIgnoreCase);

            VerifyCSharpFix(Template.Replace("###", wrongText), Template.Replace("###", correctText));
        }

        protected override string GetDiagnosticId() => MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2202_CodeFixProvider();
    }
}