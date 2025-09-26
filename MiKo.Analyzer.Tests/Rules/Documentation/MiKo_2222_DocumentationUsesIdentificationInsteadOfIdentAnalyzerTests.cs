using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTerms =
                                                      [
                                                          " ident ", " ident,", " ident;", " ident.", " ident:", " ident)", " ident]", " ident}", " ident>",
                                                          " Ident ", " Ident,", " Ident;", " Ident.", " Ident:", " Ident)", " Ident]", " Ident}", " Ident>",
                                                          " IDENT ", " IDENT,", " IDENT;", " IDENT.", " IDENT:", " IDENT)", " IDENT]", " IDENT}", " IDENT>",
                                                      ];

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_correct_term_in_commented_class_([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The identification something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_Ident_in_code_tag_([Values("c", "code")] string xmlTag) => No_issue_is_reported_for(@"
/// <summary>
/// <" + xmlTag + @">
/// var ident = 42;
/// </" + xmlTag + @">
/// </summary>
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Ident_in_Xml_tag_([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(WrongTerms))] string term) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The " + term.Trim() + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_type_comment_with_([ValueSource(nameof(WrongTerms))] string wrongTerm)
        {
            const string Template = @"
/// <summary>
/// The ### something.
/// </summary>
public sealed class TestMe { }
";

            var wrongText = wrongTerm.Trim();
            var correctText = wrongText.Replace("ident", "identification", StringComparison.OrdinalIgnoreCase);

            VerifyCSharpFix(Template.Replace("###", wrongText), Template.Replace("###", correctText));
        }

        [Test]
        public void Code_gets_fixed_for_type_comment_ending_with_([ValueSource(nameof(WrongTerms))] string wrongTerm)
        {
            const string Template = @"
/// <summary>
/// The ###
/// </summary>
public sealed class TestMe { }
";

            var wrongText = wrongTerm.Trim();
            var correctText = wrongText.Replace("ident", "identification", StringComparison.OrdinalIgnoreCase);

            VerifyCSharpFix(Template.Replace("###", wrongText.TrimEnd()), Template.Replace("###", correctText));
        }

        protected override string GetDiagnosticId() => MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2222_DocumentationUsesIdentificationInsteadOfIdentAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2222_CodeFixProvider();
    }
}