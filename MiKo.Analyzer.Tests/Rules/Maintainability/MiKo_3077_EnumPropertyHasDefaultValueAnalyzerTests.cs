using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3077_EnumPropertyHasDefaultValueAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_on_class_without_properties() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }
}
");

        [Test]
        public void No_issue_is_reported_on_record_with_primary_ctor() => No_issue_is_reported_for(@"
using System;

public record TestMe(StringComparison Comparison);
");

        [Test]
        public void No_issue_is_reported_for_only_nonEnum_properties() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }

    public int SomeValue { get; set; }

    public string SomeText { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_Enum_property_if_property_has_initializer() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }

    public StringComparison Comparison { get; set; } = StringComparison.OrdinalIgnoreCase;
}
");

        [Test]
        public void No_issue_is_reported_for_Enum_property_if_ctor_sets_property_value_to_default_one() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_Enum_property_if_ctor_sets_property_value_to_parameter_value() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_arrow_clause_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_getter_only_arrow_clause_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_getter_only_non_auto_calculated_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_arrow_clause_non_auto_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_getter_only_arrow_clause_non_auto_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_getter_only_non_auto_Enum_property() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_property_on_class_that_invokes_other_ctor() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_arrow_clause_Enum_property_if_ctor_sets_property_backing_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
        m_comparison = StringComparison.Ordinal;
    }

    public StringComparison Comparison
    {
        get => m_comparison;
        set => m_comparison = value;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Enum_property_if_ctor_sets_property_backing_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
        m_comparison = StringComparison.Ordinal;
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
        public void No_issue_is_reported_for_arrow_clause_getter_only_backing_field_Enum_property_if_ctor_sets_property_backing_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
        m_comparison = StringComparison.Ordinal;
    }

    public StringComparison Comparison
    {
        get => m_comparison;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_getter_only_backing_field_Enum_property_if_ctor_sets_property_backing_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public TestMe()
    {
        m_comparison = StringComparison.Ordinal;
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

        [Test]
        public void No_issue_is_reported_for_arrow_clause_Enum_property_if_ctor_does_not_set_property_value_but_backing_field_has_default_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.Ordinal;

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
        public void No_issue_is_reported_for_Enum_property_if_ctor_does_not_set_property_value_but_backing_field_has_default_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.Ordinal;

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
        public void No_issue_is_reported_for_arrow_clause_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value_but_backing_field_has_default_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.Ordinal;

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
        public void No_issue_is_reported_for_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value_but_backing_field_has_default_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.Ordinal;

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

        [Test]
        public void No_issue_is_reported_for_Enum_auto_property_with_initializer_and_no_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison Comparison { get; set; } = StringComparison.Ordinal;
}
");

        [Test]
        public void An_issue_is_reported_for_Enum_auto_property_with_no_ctor() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public StringComparison Comparison { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_Enum_auto_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_arrow_clause_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_arrow_clause_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_getter_only_backing_field_Enum_property_if_ctor_does_not_set_property_value() => An_issue_is_reported_for(@"
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

        [Test]
        public void Code_gets_fixed_for_Enum_auto_property_with_no_ctor()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public StringComparison Comparison { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public StringComparison Comparison { get; set; } = StringComparison.CurrentCulture;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_arrow_clause_Enum_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public StringComparison Comparison
    {
        get => m_comparison;
        set => m_comparison = value;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.CurrentCulture;

    public StringComparison Comparison
    {
        get => m_comparison;
        set => m_comparison = value;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_getter_only_arrow_clause_Enum_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public StringComparison Comparison
    {
        get => m_comparison;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.CurrentCulture;

    public StringComparison Comparison
    {
        get => m_comparison;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Enum_property_with_backing_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison;

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
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.CurrentCulture;

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
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_getter_only_Enum_property_with_backing_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison;

    public StringComparison Comparison
    {
        get
        {
            return m_comparison;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private StringComparison m_comparison = StringComparison.CurrentCulture;

    public StringComparison Comparison
    {
        get
        {
            return m_comparison;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3077_EnumPropertyHasDefaultValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3077_EnumPropertyHasDefaultValueAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3077_CodeFixProvider();
    }
}