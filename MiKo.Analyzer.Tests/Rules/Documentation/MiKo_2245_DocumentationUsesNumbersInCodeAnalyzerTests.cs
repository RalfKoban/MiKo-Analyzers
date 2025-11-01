using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2245_DocumentationUsesNumbersInCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_number_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_number_suffix_in_name_([Values("X123", "X-123")] string name) => No_issue_is_reported_for(@"
/// <summary>
/// This is some text for " + name + @".
/// </summary>
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_number_in_c_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text for <c>-1</c>. Just to be sure.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_number_in_code_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text for <code>-1</code>. Just to be sure.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void An_issue_is_reported_for_a_number_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text for -1. Just to be sure.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void An_issue_is_reported_for_a_number_followed_by_a_colon_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text for -1, just to be sure.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_number()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some text for -1. Just to be sure.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some text for <c>-1</c>. Just to be sure.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_number_at_the_end()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some text for -1
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some text for <c>-1</c>
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_big_number_at_end()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some text for 12345
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some text for <c>12345</c>
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_numbers()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some text for -1. Just to be sure. It is not 0, not 2 and not -10. Just -1.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some text for <c>-1</c>. Just to be sure. It is not <c>0</c>, not <c>2</c> and not <c>-10</c>. Just <c>-1</c>.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Pi()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is Pi with around 3.1415927. Just to be sure.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is Pi with around <c>3.1415927</c>. Just to be sure.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_number_but_not_in_name()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is a -1 used by TestMe2 to see what happens.
                                        /// </summary>
                                        public sealed class TestMe2 { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is a <c>-1</c> used by TestMe2 to see what happens.
                                     /// </summary>
                                     public sealed class TestMe2 { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2245_DocumentationUsesNumbersInCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2245_DocumentationUsesNumbersInCodeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2245_CodeFixProvider();
    }
}