using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4007_OperatorsOrderedBeforeMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_without_operator() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_only_operator() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool operator ==(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_operator_before_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public static bool operator ==(TestMe left, TestMe right) => false;

    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_operator_after_method() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public static bool operator ==(TestMe left, TestMe right) => false;
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_operator_after_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething() { }

    public static bool operator ==(TestMe left, TestMe right) => false;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public static bool operator ==(TestMe left, TestMe right) => false;

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_field_and_operator_after_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private readonly int m_field = 42;

    public void DoSomething() { }

    public static bool operator ==(TestMe left, TestMe right) => false;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private readonly int m_field = 42;

    public static bool operator ==(TestMe left, TestMe right) => false;

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4007_OperatorsOrderedBeforeMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4007_OperatorsOrderedBeforeMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4007_CodeFixProvider();
    }
}