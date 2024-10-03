using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_non_string_(string condition) => No_issue_is_reported_for(@"
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
        [TestCase("a == b || a?.GetHashCode() == 42")]
        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) is false")]
        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) is false")]
        [TestCase("a == b || a?.Equals(b) == false")]
        [TestCase("a == b || a?.Equals(b) is false")]
        [TestCase("a == b || c?.Equals(d, StringComparison.Ordinal) == true")]
        [TestCase("a == b || c?.Equals(d, StringComparison.Ordinal) is true")]
        [TestCase("a == b || c?.Equals(d) == true")]
        [TestCase("a == b || c?.Equals(d) is true")]
        [TestCase("a == b || (c != null && c.Equals(d, StringComparison.Ordinal))")]
        [TestCase("a == b || (c != null && c.Equals(d))")]
        public void No_issue_is_reported_for_condition_with_unrelated_condition_(string condition) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string a, string b, string c, string d)
    {
        if (" + condition + @")
        { }
    }
}
");

        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) == true")]
        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) is true")]
        [TestCase("a == b || (a != null && a.Equals(b, StringComparison.Ordinal))")]
        [TestCase("(a == b) || (a != null && a.Equals(b, StringComparison.Ordinal))")]
        [TestCase("(a == b || a?.Equals(b, StringComparison.Ordinal) == true)")]
        [TestCase("(a == b || a?.Equals(b, StringComparison.Ordinal) is true)")]
        [TestCase("(a == b || (a != null && a.Equals(b, StringComparison.Ordinal)))")]
        [TestCase("((a == b) || (a != null && a.Equals(b, StringComparison.Ordinal)))")]
        [TestCase("(a == b || (a?.Equals(b, StringComparison.Ordinal) == true))")]
        [TestCase("(a == b || (a?.Equals(b, StringComparison.Ordinal) is true))")]
        [TestCase("a == b || a?.Equals(b) == true")]
        [TestCase("a == b || a?.Equals(b) is true")]
        [TestCase("a == b || (a != null && a.Equals(b))")]
        [TestCase("(a == b) || (a != null && a.Equals(b))")]
        [TestCase("(a == b || a?.Equals(b) == true)")]
        [TestCase("(a == b || a?.Equals(b) is true)")]
        [TestCase("(a == b || (a != null && a.Equals(b)))")]
        [TestCase("((a == b) || (a != null && a.Equals(b)))")]
        public void An_issue_is_reported_for_condition_(string condition) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string a, string b)
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
    public string A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (left.A == right.A || (left.A != null && left.A.Equals(right.A, StringComparison.Ordinal)))
        { }
    }
}
");

        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) == true")]
        [TestCase("a == b || a?.Equals(b, StringComparison.Ordinal) is true")]
        [TestCase("a == b || (a != null && a.Equals(b, StringComparison.Ordinal))")]
        [TestCase("(a == b) || (a != null && a.Equals(b, StringComparison.Ordinal))")]
        [TestCase("(a == b || a?.Equals(b, StringComparison.Ordinal) == true)")]
        [TestCase("(a == b || a?.Equals(b, StringComparison.Ordinal) is true)")]
        [TestCase("(a == b || (a != null && a.Equals(b, StringComparison.Ordinal)))")]
        [TestCase("((a == b) || (a != null && a.Equals(b, StringComparison.Ordinal)))")]
        [TestCase("(a == b || (a?.Equals(b, StringComparison.Ordinal) == true))")]
        [TestCase("(a == b || (a?.Equals(b, StringComparison.Ordinal) is true))")]
        public void Code_gets_fixed_for_condition_with_StringComparison_(string condition)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(string a, string b)
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
    public void DoSomething(string a, string b)
    {
        if (string.Equals(a, b, StringComparison.Ordinal))
        { }
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("a == b || a?.Equals(b) == true")]
        [TestCase("a == b || a?.Equals(b) is true")]
        [TestCase("a == b || (a != null && a.Equals(b))")]
        [TestCase("(a == b) || (a != null && a.Equals(b))")]
        [TestCase("(a == b || a?.Equals(b) == true)")]
        [TestCase("(a == b || a?.Equals(b) is true)")]
        [TestCase("(a == b || (a != null && a.Equals(b)))")]
        [TestCase("((a == b) || (a != null && a.Equals(b)))")]
        public void Code_gets_fixed_for_condition_without_StringComparison_(string condition)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(string a, string b)
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
    public void DoSomething(string a, string b)
    {
        if (string.Equals(a, b))
        { }
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_condition_with_const_StringComparison()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private const StringComparison Comparison = StringComparison.Ordinal;

    public void DoSomething(string a, string b)
    {
        if (a == b || (a != null && a.Equals(b, Comparison)))
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const StringComparison Comparison = StringComparison.Ordinal;

    public void DoSomething(string a, string b)
    {
        if (string.Equals(a, b, Comparison))
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_more_complex_condition_with_const_StringComparison()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private const StringComparison Comparison = StringComparison.Ordinal;

    public void DoSomething(string a, string b, int i)
    {
        if (i == 42 || (a == b || (a != null && a.Equals(b, Comparison))))
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const StringComparison Comparison = StringComparison.Ordinal;

    public void DoSomething(string a, string b, int i)
    {
        if (i == 42 || string.Equals(a, b, Comparison))
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_condition_with_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (left.A == right.A || (left.A != null && left.A.Equals(right.A, StringComparison.Ordinal)))
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        if (string.Equals(left.A, right.A, StringComparison.Ordinal))
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_condition_with_property_and_const_StringComparison()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        const StringComparison Comparison = StringComparison.Ordinal;

        if (left.A == right.A || (left.A != null && left.A.Equals(right.A, Comparison)))
        { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string A { get; }

    public void DoSomething(TestMe left, TestMe right)
    {
        const StringComparison Comparison = StringComparison.Ordinal;

        if (string.Equals(left.A, right.A, Comparison))
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3222_LogicalConditionsUsingStringComparisonCanBeSimplifiedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3222_CodeFixProvider();
    }
}