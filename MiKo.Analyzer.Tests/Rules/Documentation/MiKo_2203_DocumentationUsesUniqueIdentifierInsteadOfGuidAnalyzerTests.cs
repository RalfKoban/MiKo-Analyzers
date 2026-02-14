using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongGuids =
                                                      [
                                                          " guid ", " guid,", " guid;", " guid.", " guid:", " guid)", " guid]", " guid}",
                                                          " Guid ", " Guid,", " Guid;", " Guid.", " Guid:", " Guid)", " Guid]", " Guid}",
                                                          " GUID ", " GUID,", " GUID;", " GUID.", " GUID:", " GUID)", " GUID]", " GUID}",
                                                          " guids ", " guids,", " guids;", " guids.", " guids:", " guids)", " guids]", " guids}",
                                                          " Guids ", " Guids,", " Guids;", " Guids.", " Guids:", " Guids)", " Guids]", " Guids}",
                                                          " GUIDs ", " GUIDs,", " GUIDs;", " GUIDs.", " GUIDs:", " GUIDs)", " GUIDs]", " GUIDs}",
                                                          " GUIDS ", " GUIDS,", " GUIDS;", " GUIDS.", " GUIDS:", " GUIDS)", " GUIDS]", " GUIDS}",
                                                      ];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_documentation_using_unique_identifier_term_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The unique identifier something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_code_tag_([Values("c", "code")] string xmlTag, [ValueSource(nameof(WrongGuids))] string wrongValue) => No_issue_is_reported_for(@"
/// <summary>
/// <" + xmlTag + @">
/// The " + wrongValue.Trim() + @" something.
/// </" + xmlTag + @">
/// </summary>
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_documentation_in_xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongGuids))] string wrongValue) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + wrongValue.Trim() + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_type_documentation_with_([ValueSource(nameof(WrongGuids))] string wrongGuid)
        {
            const string Template = @"
/// <summary>
/// The ### something.
/// </summary>
public sealed class TestMe { }
";

            var wrongText = wrongGuid.Trim();
            var correctText = ToCorrectText(wrongText);

            VerifyCSharpFix(Template.Replace("###", wrongText), Template.Replace("###", correctText));
        }

        [Test]
        public void Code_gets_fixed_for_type_documentation_ending_with_([ValueSource(nameof(WrongGuids))] string wrongGuid)
        {
            const string Template = @"
/// <summary>
/// The ###
/// </summary>
public sealed class TestMe { }
";

            var wrongText = wrongGuid.Trim();
            var correctText = ToCorrectText(wrongText);

            VerifyCSharpFix(Template.Replace("###", wrongText), Template.Replace("###", correctText));
        }

        [Test]
        public void Code_gets_fixed_when_preceded_by_article_([ValueSource(nameof(WrongGuids))] string wrongGuid)
        {
            const string OriginalTemplate = @"
/// <summary>
/// A ### something.
/// </summary>
public sealed class TestMe { }
";

            const string FixedTemplate = @"
/// <summary>
/// An ### something.
/// </summary>
public sealed class TestMe { }
";

            var wrongText = wrongGuid.Trim();
            var correctText = ToCorrectText(wrongText);

            VerifyCSharpFix(OriginalTemplate.Replace("###", wrongText), FixedTemplate.Replace("###", correctText));
        }

        protected override string GetDiagnosticId() => MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2203_CodeFixProvider();

        private static string ToCorrectText(string wrongText) => wrongText.Replace("guids", "guids", StringComparison.OrdinalIgnoreCase) // next step will replace 'guid', so 'guids' becomes 'unique identifiers'
                                                                          .Replace("guid", "unique identifier", StringComparison.OrdinalIgnoreCase);
    }
}