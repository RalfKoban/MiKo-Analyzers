using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6048_LogicalConditionsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_logical_condition_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1 || flag2)
            return;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer();
    }
}