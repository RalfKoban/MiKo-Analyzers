using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3232_DoNotUseEmptyPropertyPatternInsideConditionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_property_pattern_in_extended_property_pattern() => No_issue_is_reported_for(@"
using System;
using System.Xml;

public class TestMe
{
    public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
    {
        if (propertyLambda.Body is not MemberExpression
            {
                Member: PropertyInfo
                {
                    ReflectedType: { } reflectedType,
                    Name: { } name
                }
            })
        {
            throw new ArgumentException(nameof(propertyLambda));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_property_pattern_in_is_pattern_in_if_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(TestMe testee)
    {
        if (testee.Name is { } name)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_property_pattern_in_is_pattern_conditional_expression() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public string DoSomething(TestMe testee) => testee.Name is { } name ? name : "";
}
");

        [Test]
        public void An_issue_is_reported_for_empty_property_pattern_in_is_not_pattern_in_if_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(TestMe testee)
    {
        if (testee.Name is not { } name)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_property_pattern_in_is_not_pattern_in_conditional_expression() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public string DoSomething(TestMe testee) => testee.Name is not { } name ? "" : name;
}
");

        protected override string GetDiagnosticId() => MiKo_3232_DoNotUseEmptyPropertyPatternInsideConditionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3232_DoNotUseEmptyPropertyPatternInsideConditionAnalyzer();
    }
}