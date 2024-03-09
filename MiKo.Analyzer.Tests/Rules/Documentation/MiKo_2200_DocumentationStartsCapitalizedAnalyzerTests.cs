using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2200_DocumentationStartsCapitalizedAnalyzerTests : CodeFixVerifier
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

        private static readonly char[] ValidStartingCharacter = "abcdefghijklmnopqrstuvwxyz".ToUpperInvariant().ToCharArray();

        private static readonly char[] InvalidStartingCharacter = "abcdefghijklmnopqrstuvwxyz1234567890-#+*.,;".ToCharArray();

        [Test, Combinatorial]
        public void Documentation_starting_with_upper_case_is_not_reported_for_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(ValidStartingCharacter))] char startingChar) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void Documentation_starting_with_lower_case_is_reported_for_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(InvalidStartingCharacter))] char startingChar) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Empty_documentation_is_not_reported_for_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @" />
public sealed class TestMe { }
");

        [Test]
        public void Nested_XML_documentation_is_not_reported_for_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <some />
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Special_Or_phrase_in_Para_tag_of_XML_documentation_is_not_reported() => No_issue_is_reported_for(@"
/// <para>-or-</para>
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_documentation_starting_with_lower_case_and_white_spaces()
        {
            const string OriginalText = @"
/// <summary>
/// some text.
/// </summary>
public sealed class TestMe { }
";

            const string FixedText = @"
/// <summary>
/// Some text.
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_starting_with_lower_case_and_no_white_spaces()
        {
            const string OriginalText = @"
/// <summary>
///some text.
/// </summary>
public sealed class TestMe { }
";

            const string FixedText = @"
/// <summary>
///Some text.
/// </summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_documentation_starting_with_lower_case_all_on_same_line()
        {
            const string OriginalText = @"
/// <summary>some text.</summary>
public sealed class TestMe { }
";

            const string FixedText = @"
/// <summary>Some text.</summary>
public sealed class TestMe { }
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2200_DocumentationStartsCapitalizedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2200_DocumentationStartsCapitalizedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2200_CodeFixProvider();
    }
}