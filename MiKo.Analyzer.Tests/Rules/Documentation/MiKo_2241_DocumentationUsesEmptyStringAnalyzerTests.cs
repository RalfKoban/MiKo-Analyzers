using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2241_DocumentationUsesEmptyStringAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_empty_string_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_empty_string_in_code_in_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some text for <code> some empty string </code>. Just to be sure.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void An_issue_is_reported_for_empty_string_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
/// <" + tag + @">
/// This is some empty string inside text.
/// </" + tag + @">
public sealed class TestMe { }
");

        [Test]
        public void Code_gets_fixed_for_an_empty_string()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is an empty string here.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is the <see cref="string.Empty"/> string ("") here.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_string()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is some empty string here.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is some <see cref="string.Empty"/> string ("") here.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_the_empty_string()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This is the empty string here.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// This is the <see cref="string.Empty"/> string ("") here.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_an_empty_string_as_start()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// An empty string here.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// The <see cref="string.Empty"/> string ("") here.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_the_empty_string_as_start()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// The empty string here.
                                        /// </summary>
                                        public sealed class TestMe { }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// The <see cref="string.Empty"/> string ("") here.
                                     /// </summary>
                                     public sealed class TestMe { }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2241_DocumentationUsesEmptyStringAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2241_DocumentationUsesEmptyStringAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2241_CodeFixProvider();
    }
}