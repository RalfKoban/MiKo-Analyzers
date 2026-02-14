using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2011_UnsealedClassSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_struct() => No_issue_is_reported_for("""

                                                                         /// <summary>
                                                                         /// Something.
                                                                         /// </summary>
                                                                         public struct TestMe
                                                                         {
                                                                         }

                                                                         """);

        [Test]
        public void No_issue_is_reported_for_sealed_public_class() => No_issue_is_reported_for("""

                                                                                      /// <summary>
                                                                                      /// This class cannot be inherited.
                                                                                      /// </summary>
                                                                                      public sealed class TestMe
                                                                                      {
                                                                                      }

                                                                                      """);

        [Test]
        public void No_issue_is_reported_for_sealed_non_public_class() => No_issue_is_reported_for("""

                                                                                          /// <summary>
                                                                                          /// Something.
                                                                                          /// </summary>
                                                                                          private sealed class TestMe
                                                                                          {
                                                                                          }

                                                                                          """);

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for("""

                                                                                   public sealed class TestMe
                                                                                   {
                                                                                   }

                                                                                   """);

        [Test]
        public void No_issue_is_reported_for_unsealed_class_without_inheritance_statement() => No_issue_is_reported_for("""

                                                                                        /// <summary>
                                                                                        /// Something.
                                                                                        /// </summary>
                                                                                        public class TestMe
                                                                                        {
                                                                                        }

                                                                                        """);

        [Test]
        public void An_issue_is_reported_for_unsealed_class_with_inheritance_statement_at_start() => An_issue_is_reported_for("""

                                                                                               /// <summary>
                                                                                               /// This class cannot be inherited.
                                                                                               /// Something.
                                                                                               /// </summary>
                                                                                               public class TestMe
                                                                                               {
                                                                                               }

                                                                                               """);

        [Test]
        public void An_issue_is_reported_for_unsealed_class_with_inheritance_statement_at_end() => An_issue_is_reported_for("""

                                                                                             /// <summary>
                                                                                             /// Something.
                                                                                             /// This class cannot be inherited.
                                                                                             /// </summary>
                                                                                             public class TestMe
                                                                                             {
                                                                                             }

                                                                                             """);

        [Test]
        public void No_issue_is_reported_for_unsealed_test_class_with_inheritance_statement_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation
/// This class cannot be inherited.
/// </summary>
[" + fixture + @"]
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_unsealed_class_with_inheritance_statement_after_incomplete_sentence() => An_issue_is_reported_for("""

                                                                                      /// <summary>
                                                                                      /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref="XmlRibbonLayout"/>
                                                                                      /// This class cannot be inherited.
                                                                                      /// </summary>
                                                                                      public class TestMe
                                                                                      {
                                                                                      }

                                                                                      """);

        [Test]
        public void Code_gets_fixed_by_removing_inheritance_statement_from_end_of_summary()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// Some documentation.
                                        /// This class cannot be inherited.
                                        /// </summary>
                                        public class TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Some documentation.
                                     /// </summary>
                                     public class TestMe
                                     {
                                     }

                                     """;
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_removing_inheritance_statement_from_end_of_summary_for_record()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// Some documentation.
                                        /// This class cannot be inherited.
                                        /// </summary>
                                        public record TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Some documentation.
                                     /// </summary>
                                     public record TestMe
                                     {
                                     }

                                     """;
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_removing_inheritance_statement_from_start_of_summary()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// This class cannot be inherited.
                                        /// Some documentation.
                                        /// </summary>
                                        public class TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Some documentation.
                                     /// </summary>
                                     public class TestMe
                                     {
                                     }

                                     """;
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_removing_inheritance_statement_from_middle_of_line()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// Some text. This class cannot be inherited.
                                        /// Some documentation.
                                        /// </summary>
                                        public class TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Some text. 
                                     /// Some documentation.
                                     /// </summary>
                                     public class TestMe
                                     {
                                     }

                                     """;
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_extracting_and_removing_inheritance_statement_from_single_line()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// Some text. This class cannot be inherited. Some documentation.
                                        /// </summary>
                                        public class TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Some text. Some documentation.
                                     /// </summary>
                                     public class TestMe
                                     {
                                     }

                                     """;
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_removing_inheritance_statement_after_incomplete_sentence()
        {
            const string OriginalCode = """

                                        /// <summary>
                                        /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref="XmlRibbonLayout"/>
                                        /// This class cannot be inherited.
                                        /// </summary>
                                        public class TestMe
                                        {
                                        }

                                        """;

            const string FixedCode = """

                                     /// <summary>
                                     /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref="XmlRibbonLayout"/>
                                     /// </summary>
                                     public class TestMe
                                     {
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2011_UnsealedClassSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2011_UnsealedClassSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2011_CodeFixProvider();
    }
}