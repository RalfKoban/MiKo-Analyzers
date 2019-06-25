using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3081_DoNotUseLogicalNotExpressionAnalyzerTests : CodeFixVerifier
    {
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

        protected override string GetDiagnosticId() => MiKo_3081_DoNotUseLogicalNotExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3081_DoNotUseLogicalNotExpressionAnalyzer();
    }
}