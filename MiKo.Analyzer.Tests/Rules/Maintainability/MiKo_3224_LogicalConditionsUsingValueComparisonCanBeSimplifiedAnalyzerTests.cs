using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3224_LogicalConditionsUsingValueComparisonCanBeSimplifiedAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_logical_And_condition() => No_issue_is_reported_for(@"
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

        [TestCase("a == b || (a != null && a.Equals(b))")]
        [TestCase("a == b || (a?.Equals(b) is true)")]
        [TestCase("a == b || a?.Equals(b) is true")]
        public void No_issue_is_reported_for_reference_(string condition) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object a, object b)
    {
        if (" + condition + @")
        { }
    }
}
");

        [TestCase("""a == b || a?.ToString("D") is null""")]
        [TestCase("a == b || (c != null && c.Equals(d))")]
        [TestCase("a == b || a?.Equals(b) == false")]
        [TestCase("a == b || a?.Equals(b) is false")]
        [TestCase("a == b || a?.GetHashCode() == 42")]
        [TestCase("a == b || c?.Equals(d) == true")]
        [TestCase("a == b || c?.Equals(d) is true")]
        public void No_issue_is_reported_for_condition_with_unrelated_condition_(string condition) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int a, int b, object c, object d)
    {
        if (" + condition + @")
        { }
    }
}
");

        [TestCase("((a == b) || (a != null && a.Equals(b)))")]
        [TestCase("(a == b || (a != null && a.Equals(b)))")]
        [TestCase("(a == b || (a?.Equals(b) == true))")]
        [TestCase("(a == b || (a?.Equals(b) is true))")]
        [TestCase("(a == b || a?.Equals(b) == true)")]
        [TestCase("(a == b || a?.Equals(b) is true)")]
        [TestCase("(a == b) || (a != null && a.Equals(b))")]
        [TestCase("a == b || (a != null && a.Equals(b))")]
        [TestCase("a == b || a?.Equals(b) == true")]
        [TestCase("a == b || a?.Equals(b) is true")]
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
        if (left.A == right.A || (left.A != null && left.A.Equals(right.A)))
        { }
    }
}
");

        [TestCase("a == b || a?.Equals(b) == true")]
        [TestCase("a == b || a?.Equals(b) is true")]
        [TestCase("a == b || (a != null && a.Equals(b))")]
        [TestCase("(a == b) || (a != null && a.Equals(b))")]
        [TestCase("(a == b || a?.Equals(b) == true)")]
        [TestCase("(a == b || a?.Equals(b) is true)")]
        [TestCase("(a == b || (a != null && a.Equals(b)))")]
        [TestCase("((a == b) || (a != null && a.Equals(b)))")]
        public void Code_gets_fixed_for_condition__(string condition)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething(int a, int b)
    {
        if (" + condition + @")
        { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int a, int b)
    {
        if (a == b)
        { }
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

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
        if (left.A == right.A || (left.A != null && left.A.Equals(right.A)))
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

        protected override string GetDiagnosticId() => MiKo_3224_LogicalConditionsUsingValueComparisonCanBeSimplifiedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3224_LogicalConditionsUsingValueComparisonCanBeSimplifiedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3224_CodeFixProvider();
    }
}