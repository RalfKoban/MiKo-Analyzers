using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3085_ConditionalExpressionTooLongAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_short_condition_and_paths() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => o != null ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_long_condition_and_short_paths() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_true_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items != null
                ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                : false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_false_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items == null
                ? false
                : items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
    }
}");

        protected override string GetDiagnosticId() => MiKo_3085_ConditionalExpressionTooLongAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3085_ConditionalExpressionTooLongAnalyzer();
    }
}