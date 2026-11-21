using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6082_LocalFunctionsSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_as_first_statement_followed_by_blank_line_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }

            var x = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_as_last_statement_preceded_by_blank_line_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_surrounded_by_blank_lines_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void LocalFunction()
            {
            }

            var y = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_consecutive_local_functions_separated_by_blank_line_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void LocalFunction1()
            {
            }

            void LocalFunction2()
            {
            }

            var y = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_as_only_statement_in_local_function() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_as_first_statement_followed_by_blank_line_in_local_function() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }

                var x = 42;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_as_last_statement_preceded_by_blank_line_in_local_function() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;

                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_surrounded_by_blank_lines_in_local_function() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void Outer()
            {
                var y = 42;

                void Inner()
                {
                }

                var z = 42;
            }

            var a = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_as_last_statement_not_preceded_by_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_as_first_statement_not_followed_by_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }
            var x = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_neither_preceded_nor_followed_by_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void LocalFunction()
            {
            }
            var y = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_consecutive_local_functions_without_blank_line_in_method() => An_issue_is_reported_for(2, @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction1()
            {
            }
            void LocalFunction2()
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_as_last_statement_not_preceded_by_blank_line_in_local_function() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_as_first_statement_not_followed_by_blank_line_in_local_function() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }
                var x = 42;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_neither_preceded_nor_followed_by_blank_line_in_local_function() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                void Inner()
                {
                }
                var y = 42;
            }
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_outer_and_nested_local_function_without_blank_lines_in_method() => An_issue_is_reported_for(2, @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void Outer()
            {
                var y = 42;
                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_local_function_as_last_statement_not_preceded_by_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void LocalFunction()
            {
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void LocalFunction()
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_as_first_statement_not_followed_by_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }
            var x = 42;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }

            var x = 42;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_neither_preceded_nor_followed_by_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void LocalFunction()
            {
            }
            var y = 42;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void LocalFunction()
            {
            }

            var y = 42;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_local_functions_without_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction1()
            {
            }
            void LocalFunction2()
            {
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction1()
            {
            }

            void LocalFunction2()
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_as_last_statement_not_preceded_by_blank_line_in_local_function()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                void Inner()
                {
                }
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;

                void Inner()
                {
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_as_first_statement_not_followed_by_blank_line_in_local_function()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }
                var x = 42;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }

                var x = 42;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_neither_preceded_nor_followed_by_blank_line_in_local_function()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                void Inner()
                {
                }
                var y = 42;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;

                void Inner()
                {
                }

                var y = 42;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_outer_and_nested_local_function_without_blank_lines_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            void Outer()
            {
                var y = 42;
                void Inner()
                {
                }
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            void Outer()
            {
                var y = 42;

                void Inner()
                {
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_local_function_with_leading_comment_preceded_by_blank_line_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            // helper
            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_followed_by_blank_line_and_comment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }

            // comment
            var x = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_after_statement_with_multiline_trailing_comment_and_blank_line_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42; /* first line
                           second line */

            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_with_leading_comment_preceded_by_blank_line_in_local_function() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;

                // helper
                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_leading_comment_not_preceded_by_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            // helper
            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_followed_by_comment_without_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }
            // comment
            var x = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_after_statement_with_multiline_trailing_comment_without_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42; /* first line
                           second line */
            void LocalFunction()
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_multiline_trailing_comment_not_followed_by_blank_line_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            } /* first line
                 second line */
            var x = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_with_leading_comment_not_preceded_by_blank_line_in_local_function() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                // helper
                void Inner()
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_followed_by_comment_without_blank_line_in_local_function() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }
                // comment
                var x = 42;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_local_function_with_leading_comment_not_preceded_by_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;
            // helper
            void LocalFunction()
            {
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42;

            // helper
            void LocalFunction()
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_followed_by_comment_without_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }
            // comment
            var x = 42;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            }

            // comment
            var x = 42;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_after_statement_with_multiline_trailing_comment_without_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42; /* first line
                           second line */
            void LocalFunction()
            {
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 42; /* first line
                           second line */

            void LocalFunction()
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_with_multiline_trailing_comment_not_followed_by_blank_line_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            } /* first line
                 second line */
            var x = 42;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void LocalFunction()
            {
            } /* first line
                 second line */

            var x = 42;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_with_leading_comment_not_preceded_by_blank_line_in_local_function()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;
                // helper
                void Inner()
                {
                }
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                var x = 42;

                // helper
                void Inner()
                {
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_followed_by_comment_without_blank_line_in_local_function()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }
                // comment
                var x = 42;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            void Outer()
            {
                void Inner()
                {
                }

                // comment
                var x = 42;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_local_function_as_first_statement_followed_by_blank_line_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_surrounded_by_blank_lines_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;

                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_consecutive_local_functions_separated_by_blank_line_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction1()
                    {
                    }

                    void LocalFunction2()
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_local_function_surrounded_by_blank_lines_in_local_function_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void Outer()
                    {
                        var x = 42;

                        void Inner()
                        {
                        }

                        var y = 42;
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_as_first_statement_not_followed_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction()
                    {
                    }
                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_not_preceded_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;
                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_neither_preceded_nor_followed_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;
                    void LocalFunction()
                    {
                    }
                    break;
            }
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_consecutive_local_functions_without_blank_line_in_switch_section() => An_issue_is_reported_for(2, @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction1()
                    {
                    }
                    void LocalFunction2()
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_local_function_not_preceded_by_blank_line_in_local_function_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void Outer()
                    {
                        var x = 42;
                        void Inner()
                        {
                        }
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_local_function_as_first_statement_not_followed_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction()
                    {
                    }
                    break;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_not_preceded_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;
                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;

                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_neither_preceded_nor_followed_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;
                    void LocalFunction()
                    {
                    }
                    break;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var x = 42;

                    void LocalFunction()
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_local_functions_without_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction1()
                    {
                    }
                    void LocalFunction2()
                    {
                    }

                    break;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void LocalFunction1()
                    {
                    }

                    void LocalFunction2()
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_local_function_not_preceded_by_blank_line_in_local_function_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void Outer()
                    {
                        var x = 42;
                        void Inner()
                        {
                        }
                    }

                    break;
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    void Outer()
                    {
                        var x = 42;

                        void Inner()
                        {
                        }
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6082_LocalFunctionsSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6082_LocalFunctionsSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6082_CodeFixProvider();
    }
}