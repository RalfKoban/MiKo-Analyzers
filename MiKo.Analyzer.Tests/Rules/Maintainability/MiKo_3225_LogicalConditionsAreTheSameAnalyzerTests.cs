using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3225_LogicalConditionsAreTheSameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_logical_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null)
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_condition_doing_different_things() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null && o is string s)
        { }
    }
}
");

        [TestCase("a == b || a == b")]
        [TestCase("(a == b) || a == b")]
        [TestCase("(a == b) || (a == b)")]
        [TestCase("a == b || (a == b)")]
        [TestCase("a == b && a == b")]
        [TestCase("(a == b) && a == b")]
        [TestCase("(a == b) && (a == b)")]
        [TestCase("a == b && (a == b)")]
        public void An_issue_is_reported_for_condition_(string condition) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        if (" + condition + @")
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_condition_with_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (left.A == right.A || left.A == right.A)
        { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_condition_with_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (left.A == right.A || left.A == right.A)
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (left.A == right.A)
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("a == b || a == b", "a == b")]
        [TestCase("(a == b) || a == b", "a == b")]
        [TestCase("(a == b) || (a == b)", "a == b")]
        [TestCase("a == b || (a == b)", "a == b")]
        [TestCase("a == b && a == b", "a == b")]
        [TestCase("(a == b) && a == b", "a == b")]
        [TestCase("(a == b) && (a == b)", "a == b")]
        [TestCase("a == b && (a == b)", "a == b")]
        public void Code_gets_fixed_for_condition_(string originalCondition, string fixedCondition)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething(int a, int b)
    {
        if (###)
        { }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalCondition), Template.Replace("###", fixedCondition));
        }

        protected override string GetDiagnosticId() => MiKo_3225_LogicalConditionsAreTheSameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3225_LogicalConditionsAreTheSameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3225_CodeFixProvider();
    }
}