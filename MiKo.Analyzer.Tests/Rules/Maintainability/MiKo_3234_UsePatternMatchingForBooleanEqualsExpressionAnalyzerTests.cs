using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3234_UsePatternMatchingForBooleanEqualsExpressionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_comparisons_via_([Values("==", "!=")] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value " + @operator + @" true)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_character_literal() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals('x'))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_null_literal() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(null))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(42))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_string_literal() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(""abc""))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Equals_comparison_via_utf8_string_literal() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public ReadOnlySpan<byte> Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(""abc""u8))
        {
        }
    }
}
");

        [Test] // we cannot fix UTF8 strings as they are no constants usable for pattern matching
        public void No_issue_is_reported_for_Equals_comparison_via_utf8_string_literal_compared_to_true() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public ReadOnlySpan<byte> Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(""abc""u8) is true)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_compared_to_false_via_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(" + value + @") == false)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_compared_to_false_via_method_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(" + value + @") == false)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_via_is_false_via_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(" + value + @") is false)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_via_is_false_via_method_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(" + value + @") is false)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_with_false_compared_to_via_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (false == a?.SubItem?.Value?.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_conditional_nested_Equals_result_with_false_compared_to_via_method_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (false == a?.GetSubItem()?.Value?.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_compared_to_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(" + value + @") == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_compared_to_true_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(" + value + @") == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_via_is_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(" + value + @") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_via_is_true_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(" + value + @") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_with_true_compared_to_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a?.SubItem?.Value?.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_nested_Equals_result_with_true_compared_to_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (true == a?.GetSubItem()?.Value?.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_comparison_via_character_literal_compared_to_true() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals('x') == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_comparison_via_null_literal_compared_to_true() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(null) == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_comparison_via_number_compared_to_true() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(42) == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_comparison_via_string_literal_compared_to_true() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(""abc"") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_result_compared_to_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(" + value + @") == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_result_via_is_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(" + value + @") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Equals_result_with_true_compared_to_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.Value.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_compared_to_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals(" + value + @") == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_compared_to_true_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals(" + value + @") == true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_via_is_true_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals(" + value + @") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_via_is_true_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals(" + value + @") is true)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_with_true_compared_to_via_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.SubItem.Value.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_Equals_result_with_true_compared_to_via_method_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (true == a.GetSubItem().Value.Equals(" + value + @"))
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_Equals_false()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(false) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_false_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(false) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_false_via_method_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (true == a?.GetSubItem()?.Value?.Equals(false))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_false_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a?.SubItem?.Value?.Equals(false))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(true) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(true) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_true_via_method_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (true == a?.GetSubItem()?.Value?.Equals(true))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_true_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a?.SubItem?.Value?.Equals(true))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_is_false()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(false) is true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_is_false_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(false) is true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is false)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_is_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(true) is true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_is_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(true) is true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool? Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is true)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_number_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(42) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_number_compared_to_true_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.Value.Equals(42))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_number_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals(42) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_number_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals(42) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_number_compared_to_true()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(42) == true)
        {
        }
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public int Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_number_compared_to_true_via_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(42) == true)
        {
        }
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public int Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is 42)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_character_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals('x') == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_character_literal_compared_to_true_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.Value.Equals('x'))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_character_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals('x') == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_character_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals('x') == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public char Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_character_literal_compared_to_true()
        {
            const string OriginalCode = @"
public class TestMe
{
    public char Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals('x') == true)
        {
        }
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public char Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_character_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public char Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals('x') == true)
        {
        }
    }
}";

            const string FixedCode = @"
public class TestMe
{
    public char Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is 'x')
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_string_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(""abc"") == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_string_literal_compared_to_true_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.Value.Equals(""abc""))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_string_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals(""abc"") == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_string_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals(""abc"") == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_string_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(""abc"") == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_string_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(""abc"") == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is ""abc"")
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_null_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value.Equals(null) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Equals_null_literal_compared_to_true_with_swapped_operands()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (true == a.Value.Equals(null))
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public void DoSomething(TestMe a)
    {
        if (a.Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_null_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value.Equals(null) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a.SubItem.Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_Equals_null_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value.Equals(null) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a.GetSubItem().Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_null_literal_compared_to_true()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value?.Equals(null) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe SubItem { get; }

    public void DoSomething(TestMe a)
    {
        if (a?.SubItem?.Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditional_nested_Equals_null_literal_compared_to_true_via_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value?.Equals(null) == true)
        {
        }
    }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object Value { get; }

    public TestMe GetSubItem() => null;

    public void DoSomething(TestMe a)
    {
        if (a?.GetSubItem()?.Value is null)
        {
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3234_CodeFixProvider();

        protected override string GetDiagnosticId() => MiKo_3234_UsePatternMatchingForBooleanEqualsExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3234_UsePatternMatchingForBooleanEqualsExpressionAnalyzer();
    }
}