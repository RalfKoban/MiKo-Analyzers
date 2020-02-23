using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3086_DoNotNestConditionalExpressionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_nested_conditional_expression() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => o != null ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_nested_conditional_in_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => (o != null ? true : false) ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_nested_conditional_in_true_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items != null
                ? (items.Count > 1 ? true : false)
                : false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_nested_conditional_in_false_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items == null
                ? false
                : (items.Count > 1 ? true : false);
    }
}");

        [Test]
        public void An_issue_is_reported_for_coalesce_expression_inside_conditional_expression() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o1, object o2) => (o1 ?? o2) != null ? true : false;
}");

        protected override string GetDiagnosticId() => MiKo_3086_DoNotNestConditionalExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3086_DoNotNestConditionalExpressionAnalyzer();
    }
}