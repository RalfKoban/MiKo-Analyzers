using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3243_AvoidDoWhileLoopAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_if_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_do_while_statement() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
        }
        while (flag);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3243_AvoidDoWhileLoopAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3243_AvoidDoWhileLoopAnalyzer();
    }
}