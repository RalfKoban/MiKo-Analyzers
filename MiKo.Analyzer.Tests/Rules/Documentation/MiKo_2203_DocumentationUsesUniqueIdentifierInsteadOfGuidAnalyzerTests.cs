using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzerTests : CodeFixVerifier
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

        private static readonly string[] WrongGuids =
            {
                " guid ",
                " guid,",
                " guid;",
                " guid.",
                " guid:",
                " Guid ",
                " Guid,",
                " Guid;",
                " Guid.",
                " Guid:",
                " GUID ",
                " GUID,",
                " GUID;",
                " GUID.",
                " GUID:",
            };

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Guid_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongGuids))] string guid) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + guid + @" something.
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
/// The unique identifier something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_type_([ValueSource(nameof(WrongGuids))] string wrongText)
        {
            const string Template = @"
/// <summary>
/// The ### something.
/// </summary>
public sealed class TestMe { }
";

            var correctText = wrongText.Replace("guid", "unique identifier", StringComparison.OrdinalIgnoreCase);

            VerifyCSharpFix(Template.Replace("###", wrongText), Template.Replace("###", correctText));
        }

        protected override string GetDiagnosticId() => MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2203_CodeFixProvider();
    }
}