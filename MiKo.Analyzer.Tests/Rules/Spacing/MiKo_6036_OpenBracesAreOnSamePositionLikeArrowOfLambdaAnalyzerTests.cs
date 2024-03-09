using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line_and_has_no_braces() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() => GC.Collect());
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line_and_is_method_group() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(GC.Collect);
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() => { GC.Collect(); });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_brace_is_on_other_line_than_arrow_token_but_placed_below_arrow_token() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 GC.Collect();
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            GC.Collect();
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_brace_is_on_other_line_than_arrow_token_and_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                                {
                                    GC.Collect();
                                });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
");

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            GC.Collect();
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 GC.Collect();
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                                {
                                    GC.Collect();
                                });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 GC.Collect();
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_nested_if_statement_with_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            if (true)
            {
                GC.Collect();
            }
            else
            {
                GC.Collect();
            }
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 if (true)
                                 {
                                     GC.Collect();
                                 }
                                 else
                                 {
                                     GC.Collect();
                                 }
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_nested_if_statement_without_block()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            if (true)
                GC.Collect();
            else
                GC.Collect();
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 if (true)
                                     GC.Collect();
                                 else
                                     GC.Collect();
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_nested_if_statement_with_conditions_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(b =>
        {
            if (b is true
                || b is false)
            {
                GC.Collect();
            }
        });
    }

    public void DoSomethingCore(Action<bool> callback)
    {
        callback(true);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(b =>
                            {
                                if (b is true
                                    || b is false)
                                {
                                    GC.Collect();
                                }
                            });
    }

    public void DoSomethingCore(Action<bool> callback)
    {
        callback(true);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_nested_switch_statement_with_mixed_blocks()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(mode =>
        {
            switch (mode)
            {
                case GCCollectionMode.Optimized:
                {
                    GC.Collect();
                    break;
                }

                case GCCollectionMode.Forced:
                    GC.Collect();
                    break;

                default:
                {
                    GC.Collect();
                    break;
                }
            }
        });
    }

    public void DoSomethingCore(Action<GCCollectionMode> callback)
    {
        callback(GCCollectionMode.Default);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(mode =>
                               {
                                   switch (mode)
                                   {
                                       case GCCollectionMode.Optimized:
                                       {
                                           GC.Collect();
                                           break;
                                       }

                                       case GCCollectionMode.Forced:
                                           GC.Collect();
                                           break;

                                       default:
                                       {
                                           GC.Collect();
                                           break;
                                       }
                                   }
                               });
    }

    public void DoSomethingCore(Action<GCCollectionMode> callback)
    {
        callback(GCCollectionMode.Default);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_for_loop()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                GC.Collect();
            }
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 for (int i = 0; i < 10; i++)
                                 {
                                     GC.Collect();
                                 }
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_foreach_loop()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            foreach (var i in new[] { 1, 2, 3 })
            {
                GC.Collect();
            }
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 foreach (var i in new[] { 1, 2, 3 })
                                 {
                                     GC.Collect();
                                 }
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_while_loop()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            while (true)
            {
                GC.Collect();
            }
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 while (true)
                                 {
                                     GC.Collect();
                                 }
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_do_while_loop()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
        {
            do
            {
                GC.Collect();
            }
            while (true);
        });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(() =>
                             {
                                 do
                                 {
                                     GC.Collect();
                                 }
                                 while (true);
                             });
    }

    public void DoSomethingCore(Action callback)
    {
        callback();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_ternary_operator()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
        {
            return i == 42
                   ? true
                   : false;
        });
    }

    public void DoSomethingCore(Func<int, bool> callback)
    {
        callback(42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
                            {
                                return i == 42
                                       ? true
                                       : false;
                            });
    }

    public void DoSomethingCore(Func<int, bool> callback)
    {
        callback(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_call_with_argument_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
        {
            return i.ToString(
                       ""D"");
        });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
                            {
                                return i.ToString(
                                           ""D"");
                            });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_as_cast_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
        {
            var result = i
                            as string;

            return result;
        });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
                            {
                                var result = i
                                                as string;

                                return result;
                            });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_brace_is_on_other_line_than_arrow_token_and_indented_to_method_and_contains_cast_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
        {
            var result = (string)
                            i;

            return result;
        });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(i =>
                            {
                                var result = (string)
                                                i;

                                return result;
                            });
    }

    public void DoSomethingCore(Func<int, string> callback)
    {
        callback(42);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_string_combination_spans_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(s =>
        {
            DoSomethingElse(@""
using NUnit.Framework;

["" + s + @""]
public class TestMe
{
    ["" + s + @""]
    public void SomeMethod()
    {
    }
}
"");
        });
    }

    public void DoSomethingCore(Action<string> callback)
    {
        callback(""something"");
    }

    private string DoSomethingElse(string input) => input;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(s =>
                            {
                                DoSomethingElse(@""
using NUnit.Framework;

["" + s + @""]
public class TestMe
{
    ["" + s + @""]
    public void SomeMethod()
    {
    }
}
"");
                            });
    }

    public void DoSomethingCore(Action<string> callback)
    {
        callback(""something"");
    }

    private string DoSomethingElse(string input) => input;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6036_CodeFixProvider();
    }
}