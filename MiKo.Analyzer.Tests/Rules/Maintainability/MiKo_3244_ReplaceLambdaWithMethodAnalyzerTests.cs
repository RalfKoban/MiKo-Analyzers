using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3244_ReplaceLambdaWithMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_short_lambda() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () => 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lambda_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                          ;
                          ;
                          ;
                     };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lambda_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                          ;
                          ;
                          ;
                          ;
                     };
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3244_ReplaceLambdaWithMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3244_ReplaceLambdaWithMethodAnalyzer { MaxLinesOfCode = 3 };
    }
}