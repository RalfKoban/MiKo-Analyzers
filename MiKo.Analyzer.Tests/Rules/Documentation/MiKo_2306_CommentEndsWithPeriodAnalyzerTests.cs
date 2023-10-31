using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2306_CommentEndsWithPeriodAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_comment_with_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_periods() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // another comment.
        // final comment.
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_without_period() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_multi_line_comment_with_some_missing_periods() => An_issue_is_reported_for(2, @"

public class TestMe
{
    public void DoSomething()
    {
        // some comment
        // another comment
        // final comment.
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_triple_period() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment...
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2306_CommentEndsWithPeriodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            MiKo_2306_CommentEndsWithPeriodAnalyzer.EnabledPerDefault = true;

            return new MiKo_2306_CommentEndsWithPeriodAnalyzer();
        }
    }
}