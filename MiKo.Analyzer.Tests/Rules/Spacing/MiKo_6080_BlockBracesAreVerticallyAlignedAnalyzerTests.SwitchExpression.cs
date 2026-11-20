using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_switch_expression_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch { 1 => i, _ => i };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_switch_expression() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                        };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                       };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                         };
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                       };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                         };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_on_same_line_as_last_arm()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_on_same_line_as_last_arm_comma()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i, };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_on_same_line_as_last_arm_comma_with_block_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i, /* some comment */ };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i, /* some comment */
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_close_brace_on_same_line_as_last_arm_with_block_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i /* some comment */ };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i /* some comment */
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                            // some comment
                       };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                            // some comment
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                            // some comment
                         };
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            _ => i,
                            // some comment
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
#if FLAG
                            _ => i,
#endif
                       };
    }
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
#if FLAG
                            _ => i,
#endif
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
#if FLAG
                            _ => i,
#endif
                         };
    }
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
#if FLAG
                            _ => i,
#endif
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
