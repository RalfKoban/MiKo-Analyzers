using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6059_SwitchExpressionArmsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_switch_expression_arm_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_arm_if_comma_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true
                                                                        ,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_arm_if_result_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal =>
                                                                    true,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_arm_if_arm_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal
                                                                 => true,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_arm_if_value_is_split_after_dot_and_placed_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.
                                                    Ordinal => true,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_arm_if_value_is_split_before_dot_and_placed_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison
                                                    .Ordinal => true,
                                        _ => false,
                                    };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_last_switch_expression_arm_if_value_is_on_different_line_but_arm_does_not_end_with_separator() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ =>
                                             false
                                    };
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_expression_arm_if_it_spans_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison
                                                    .
                                                      Ordinal
                                                            =>
                                                                true
                                                                    ,
                                        _ => false,
                                    };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ => false,
                                    };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_arm_throwing_an_exception_if_it_spans_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _
                                            =>
                                                throw
                                                    new
                                                      ArgumentOutOfRangeException(
                                                            nameof(
                                                                direction
                                                                        )
                                                                         ,
                                                                            ""some value""
                                                                                )
                                                                                    ,
                                    };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ => throw new ArgumentOutOfRangeException(nameof(direction), ""some value""),
                                    };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_last_switch_expression_arm_if_value_is_on_different_line_but_arm_does_not_end_with_separator()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ =>
                                             false
                                    };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison)
    {
            return comparison switch
                                    {
                                        StringComparison.Ordinal => true,
                                        _ => false
                                    };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6059_SwitchExpressionArmsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6059_SwitchExpressionArmsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6059_CodeFixProvider();
    }
}