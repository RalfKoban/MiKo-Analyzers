using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3005_TryMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Try_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");
        [Test]
        public void No_issue_is_reported_for_test_class() => No_issue_is_reported_for(@"
using System;

[TestFixture]
public class TestMe
{
    public void TryDoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_Try_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool TryDoSomething(out int i)
    {
        i = 42;
        return true;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Try_method_without_boolean_return_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void TryDoSomething(out int i)
    {
        i = 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Try_method_without_out_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool TryDoSomething(int i)
    {
        i = 42;
        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Try_method_without_out_parameter_not_as_last_one() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool TryDoSomething(out int i, int j)
    {
        i = j;
        return false;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3005_TryMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3005_TryMethodsAnalyzer();
    }
}