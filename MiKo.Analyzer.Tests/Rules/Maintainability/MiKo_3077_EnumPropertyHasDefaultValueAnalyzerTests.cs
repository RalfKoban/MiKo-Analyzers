using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3077_EnumPropertyHasDefaultValueAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_ctor_on_class_with_arrow_clause_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison => Calculate();

    private StringComparison Calculate() => StringComparison.Ordinal;
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_getter_only_arrow_clause_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get => Calculate();
    }

    private StringComparison Calculate() => StringComparison.Ordinal;
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_getter_only_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get
        {
            return Calculate();
        }
    }

    private StringComparison Calculate() => StringComparison.Ordinal;
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_arrow_clause_non_auto_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison => StringComparison.Ordinal;
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_getter_only_arrow_clause_non_auto_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get => StringComparison.Ordinal;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_with_getter_only_non_auto_Enum_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get
        {
            return StringComparison.Ordinal;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_on_class_that_invokes_other_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() : this(StringComparison.Ordinal)
    {
    }

    public TestMe(StringComparison comparison) => Comparison = comparison;

    public StringComparison Comparison { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_Enum_auto_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
    }

    public StringComparison Comparison { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_arrow_clause_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get => m_comparison;
        set => m_comparison = value;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get
        {
            return m_comparison;
        }

        set
        {
            m_comparison = value;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_arrow_clause_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get => m_comparison;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_on_class_with_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
    }

    public StringComparison Comparison
    {
        get
        {
            return m_comparison;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3077_EnumPropertyHasDefaultValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3077_EnumPropertyHasDefaultValueAnalyzer();
    }
}