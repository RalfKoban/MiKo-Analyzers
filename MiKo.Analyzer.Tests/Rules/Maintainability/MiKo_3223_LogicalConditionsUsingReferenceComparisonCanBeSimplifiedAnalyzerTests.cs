using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3223_LogicalConditionsUsingReferenceComparisonCanBeSimplifiedAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_non_reference_(string condition) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int? a, int? b)
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
    public void DoSomething(object a, object b, string c, string d)
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
    public void DoSomething(object a, object b)
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
    public object A { get; }

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
using System;

public class TestMe
{
    public void DoSomething(object a, object b)
    {
        if (" + condition + @")
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object a, object b)
    {
        if (object.Equals(a, b))
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
    public object A { get; }

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
    public object A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (object.Equals(left.A, right.A))
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multi_line_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object A { get; }
    public string B { get; }
    public object C { get; }

    public bool Equals(TestMe other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            (
                A == other.A ||
                A != null &&
                A.Equals(other.A)
            ) &&
            (
                B == other.B ||
                B != null &&
                B.Equals(other.B)
            ) &&
            (
                C == other.C ||
                C != null &&
                C.Equals(other.C)
            ) && base.Equals(other);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object A { get; }
    public string B { get; }
    public object C { get; }

    public bool Equals(TestMe other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            object.Equals(A, other.A) &&
            (
                B == other.B ||
                B != null &&
                B.Equals(other.B)
            ) &&
            object.Equals(C, other.C) && base.Equals(other);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3223_LogicalConditionsUsingReferenceComparisonCanBeSimplifiedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3223_LogicalConditionsUsingReferenceComparisonCanBeSimplifiedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3223_CodeFixProvider();
    }
}