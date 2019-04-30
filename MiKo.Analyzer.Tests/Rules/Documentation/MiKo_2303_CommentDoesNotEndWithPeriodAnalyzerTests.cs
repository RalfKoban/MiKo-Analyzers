using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2303_CommentDoesNotEndWithPeriodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_without_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_period() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer();
    }
}