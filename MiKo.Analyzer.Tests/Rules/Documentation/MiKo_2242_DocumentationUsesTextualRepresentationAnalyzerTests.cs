using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2242_DocumentationUsesTextualRepresentationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_string_representation_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_textual_representation_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some textual representation of something.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void An_issue_is_reported_for_string_representation_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some string representation of something.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_string_representation()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some string representation of something.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some textual representation of something.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_String_representation()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some String representation of something
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some textual representation of something
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2242_DocumentationUsesTextualRepresentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2242_DocumentationUsesTextualRepresentationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2242_CodeFixProvider();
    }
}