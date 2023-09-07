using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_switch_expression_if_brace_is_indented_correctly() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            2 => i,
                            _ => i,
                        };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_if_brace_is_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                       {
                           1 => i,
                           2 => i,
                           _ => i,
                       };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_if_brace_is_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                         {
                             1 => i,
                             2 => i,
                             _ => i,
                         };
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_expression_if_brace_is_indented_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                       {
                           1 => i,
                           2 => i,
                           _ => i,
                       };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            2 => i,
                            _ => i,
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_expression_if_brace_is_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                         {
                             1 => i,
                             2 => i,
                             _ => i,
                         };
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(int i)
    {
        return i switch
                        {
                            1 => i,
                            2 => i,
                            _ => i,
                        };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6047_CodeFixProvider();
    }
}