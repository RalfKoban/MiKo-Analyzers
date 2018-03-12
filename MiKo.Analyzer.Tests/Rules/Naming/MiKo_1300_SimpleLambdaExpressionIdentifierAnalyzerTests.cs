using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_lambda() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_correctly_named_lambda_identifier() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where(_ => _ == null)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_incorrectly_named_lambda_identifier() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        var items = new List<string>();
        if (items.Where(s => s == null)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer();
    }
}