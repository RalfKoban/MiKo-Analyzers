using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        protected override string GetDiagnosticId() => MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6036_CodeFixProvider();
    }
}