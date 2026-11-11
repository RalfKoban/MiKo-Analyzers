using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_block_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
       }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
         }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_at_same_line_as_last_statement()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        ;}
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        ;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_directive_before_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_directive_before_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_only_block_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_only_block_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_inline_opening_brace_by_only_moving_closing_brace()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething() {
        DoSomething();
        }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
                              {
                                  DoSomething();
                              }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_delegate_parameters_with_opening_brace_on_closing_parameter_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) {
                                    return i;
                                };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value)
                                                     {
                                                         return i;
                                                     };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_delegate_parameters_with_opening_brace_on_closing_parameter_line_and_trailing_comment_before_opening_brace()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) /* keep */ {
                                    return i;
                                };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int, int> x = delegate (int i,
                                          int value) /* keep */ 
                                                                {
                                                                    return i;
                                                                };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_block_preserving_outer_braces_and_region_directives()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            #region Test
            {
                DoSomething();
            }
            #endregion
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            #region Test
            {
                DoSomething();
            }
            #endregion
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_opening_brace_on_line_below_delegate_keyword_with_leading_comment_before_opening_brace()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int> x = delegate (int i)
                           /* keep */ {
                               return i;
                           };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, int> x = delegate (int i)
        /* keep */
                                      {
                                          return i;
                                      };
    }
}
";

            // note: for whatever reason the comment gets re-formatted although the adjusted document (provided by the code fix) would have the comment at the correct place
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6080_CodeFixProvider();
    }
}