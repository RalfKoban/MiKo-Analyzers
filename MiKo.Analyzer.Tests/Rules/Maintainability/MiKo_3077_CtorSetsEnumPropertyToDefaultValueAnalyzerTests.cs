using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3077_CtorSetsEnumPropertyToDefaultValueAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_ctor_on_class_without_properties() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_only_nonEnum_properties() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }

    public int SomeValue { get; set; }

    public string SomeText { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_Enum_property_if_property_has_initializer() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }

    public StringComparison Comparison { get; set; } = StringComparison.OrdinalIgnoreCase;
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_Enum_property_if_ctor_sets_property_value_to_default_one() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
        Comparison = StringComparison.OrdinalIgnoreCase;
    }

    public StringComparison Comparison { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_Enum_property_if_ctor_sets_property_value_to_parameter_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(StringComparison comparison)
    {
        Comparison = comparison;
    }

    public StringComparison Comparison { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_3077_CtorSetsEnumPropertyToDefaultValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3077_CtorSetsEnumPropertyToDefaultValueAnalyzer();
    }
}