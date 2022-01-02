using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_nameof_in_if_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MyProperty))
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_if_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == ""MyProperty"")
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_nameof_in_if_else_if_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty1 { get; set; }
    private int MyProperty2 { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MyProperty1))
        {
            return true;
        }
        else if (e.PropertyName == nameof(MyProperty2))
        {
            return true;
        }


        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_if_else_if_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty1 { get; set; }
    private int MyProperty2 { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MyProperty1))
        {
            return true;
        }
        else if (e.PropertyName == ""MyProperty2"")
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_NotEquals_comparison_with_nameof_in_if_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MyProperty))
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_NotEquals_comparison_with_string_literal_in_if_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != ""MyProperty"")
        {
            return true;
        }

        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_nameof_in_return_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        return e.PropertyName == nameof(MyProperty);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_return_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        return e.PropertyName == ""MyProperty"";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_NotEquals_comparison_with_nameof_in_return_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        return e.PropertyName != nameof(MyProperty);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_NotEquals_comparison_with_string_literal_in_return_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        return e.PropertyName != ""MyProperty"";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_in_return_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string text)
    {
        return text == ""MyText"";
    }

    public bool DoSomethingElse(string text)
    {
        return text != ""MyText"";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_comparison_with_nameof_in_switch_case_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MyProperty):
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_switch_case_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case ""MyProperty"":
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_in_switch_case_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string text)
    {
        switch (text)
        {
            case ""MyText"":
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_comparison_with_nameof_in_switch_case_when_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private PropertyChangedEventArgs Args { get; set; }

    public static bool DoSomething(object value)
    {
        switch (value)
        {
            case TestMe t when t.MyProperty.PropertyName == nameof(MyProperty):
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_switch_case_when_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private PropertyChangedEventArgs Args { get; set; }

    public static bool DoSomething(object value)
    {
        switch (value)
        {
            case TestMe t when t.MyProperty.PropertyName == ""MyProperty"":
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_in_switch_case_when_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Text { get; set; }

    public static bool DoSomething(object value)
    {
        switch (value)
        {
            case TestMe t when t.Text == ""MyText"":
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_comparison_with_nameof_in_switch_expression_arm_statement() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e) => e.PropertyName switch
                                                                                {
                                                                                    nameof(MyProperty) => true,
                                                                                    _ => false,
                                                                                };
}
");

        [Test]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_switch_expression_arm_statement() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e) => e.PropertyName switch
                                                                                {
                                                                                    ""MyProperty"" => true,
                                                                                    _ => false,
                                                                                };
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_in_switch_expression_arm_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string text) => text switch
                                                    {
                                                        ""MyText"" => true,
                                                        _ => false,
                                                    };
}
");

        [TestCase("e.PropertyName.Equals(nameof(MyProperty))")]
        [TestCase("nameof(MyProperty).Equals(e.PropertyName)")]
        [TestCase("!e.PropertyName.Equals(nameof(MyProperty))")]
        [TestCase("!nameof(MyProperty).Equals(e.PropertyName)")]
        [TestCase("Equals(e.PropertyName, nameof(MyProperty))")]
        [TestCase("Equals(nameof(MyProperty), e.PropertyName)")]
        [TestCase("object.Equals(e.PropertyName, nameof(MyProperty))")]
        [TestCase("object.Equals(nameof(MyProperty), e.PropertyName)")]
        [TestCase("string.Equals(e.PropertyName, nameof(MyProperty))")]
        [TestCase("string.Equals(nameof(MyProperty), e.PropertyName)")]
        public void No_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_comparison_with_nameof_in_Equals_statement_(string statement) => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (" + statement + @")
            return true;

        return false;
    }
}
");

        [TestCase(@"e.PropertyName.Equals(""MyProperty"")")]
        [TestCase(@"""MyProperty"".Equals(e.PropertyName)")]
        [TestCase(@"!e.PropertyName.Equals(""MyProperty"")")]
        [TestCase(@"!""MyProperty"".Equals(e.PropertyName)")]
        [TestCase(@"Equals(e.PropertyName, ""MyProperty"")")]
        [TestCase(@"Equals(""MyProperty"", e.PropertyName)")]
        [TestCase(@"object.Equals(e.PropertyName, ""MyProperty"")")]
        [TestCase(@"object.Equals(""MyProperty"", e.PropertyName)")]
        [TestCase(@"string.Equals(e.PropertyName, ""MyProperty"")")]
        [TestCase(@"string.Equals(""MyProperty"", e.PropertyName)")]
        public void An_issue_is_reported_for_PropertyChangedEventArgs_PropertyName_Equals_comparison_with_string_literal_in_Equals_statement_(string statement) => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    private int MyProperty { get; set; }

    public bool DoSomething(PropertyChangedEventArgs e)
    {
        if (" + statement + @")
            return true;

        return false;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzer();
    }
}