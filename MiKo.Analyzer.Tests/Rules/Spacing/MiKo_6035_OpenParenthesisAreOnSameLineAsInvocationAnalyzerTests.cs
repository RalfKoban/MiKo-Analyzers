using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_method_invocation_and_parenthesis_are_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC
         .Collect();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_and_parenthesis_are_on_different_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect
                  ();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_generic_types_and_parenthesis_are_on_different_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingGeneric<int,
                  float>();
    }

    public void DoSomethingGeneric<T1, T2>()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_and_parenthesis_are_on_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect
                  ();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_generic_type_arguments_and_parenthesis_are_on_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingGeneric<int,
                  float,
                  decimal>();
    }

    public void DoSomethingGeneric<T1, T2, T3>()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingGeneric<int, float, decimal>();
    }

    public void DoSomethingGeneric<T1, T2, T3>()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_generic_type_arguments_and_parenthesis_are_on_different_lines_and_type_is_given()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        TestMe.DoSomethingGeneric<int,
                                  float,
                                  decimal>();
    }

    public static void DoSomethingGeneric<T1, T2, T3>()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        TestMe.DoSomethingGeneric<int, float, decimal>();
    }

    public static void DoSomethingGeneric<T1, T2, T3>()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6035_CodeFixProvider();
    }
}