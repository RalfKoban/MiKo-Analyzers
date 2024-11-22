using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_directive() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
#if !DEBUG
        a = false;
#endif
        return a;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (a)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_AND_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a, bool b)
    {
        if (a && b)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_OR_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a, bool b)
    {
        if (a || b)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_condition_with_pattern_matching() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (a is false)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Expression_argument() => No_issue_is_reported_for(@"
using System;
using System.Linq.Expressions;

public class TestMe
{
    public bool DoSomething(bool a) => DoSomething(_ => !_);

    public bool DoSomething(Expression<Func<bool, bool>> expression) => expression != null;
}");

        [Test]
        public void No_issue_is_reported_for_logical_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a, bool b)
    {
        if (a != b)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_NOT_condition_on_constant() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public const bool A = true;

    public bool DoSomething()
    {
        if (!A)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_NOT_condition_to_invert_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_field;

    public void Invert()
    {
        m_field = !m_field;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_logical_NOT_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (!a)
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_logical_NOT_condition_with_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Exists()
    {
        return true;
    }

    public bool DoSomething()
    {
        if (!Exists())
            return true;
        else
            return false;
    }
}
");

        [TestCase(
                     "class TestMe { bool DoSomething(bool a) { if (!a) return true; return false; } }",
                     "class TestMe { bool DoSomething(bool a) { if (a is false) return true; return false; } }")]
        [TestCase(
             "class TestMe { bool DoSomething(bool a) { return !a; } }",
             "class TestMe { bool DoSomething(bool a) { return a is false; } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [Test]
        public void Code_gets_fixed_and_keeps_indentation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(int i, bool flag)
    {
        SomeProperty = i > 0 &&
                       !flag;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(int i, bool flag)
    {
        SomeProperty = i > 0 &&
                       flag is false;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3081_CodeFixProvider();
    }
}
