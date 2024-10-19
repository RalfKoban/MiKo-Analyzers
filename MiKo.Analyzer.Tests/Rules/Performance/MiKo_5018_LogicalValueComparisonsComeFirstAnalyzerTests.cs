using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5018_LogicalValueComparisonsComeFirstAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i) => i == 42 || i == 0815);
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_comparison_if_constants_come_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i) => 42 == i || 0815 == i);
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_reference_type_OR_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int i, object o) => i == 42 || o.ToString() == ""whatever"");
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_array_type_AND_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 && array.SequenceEqual(Array.Empty<int>());
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 || array.SequenceEqual(Array.Empty<int>());
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_NotEquals_Null_expression_with_null_on_left_side() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => null != other && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_NotEquals_Null_expression_with_null_on_right_side() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_Equals_Null_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool SomeComparison(TestMe other) => other == null || (Value != other.Value && Data != other.Data);
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_IsNotNull_pattern_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Value == other.Value && Data == other.Data;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_value_type_comparison_comes_first_and_IsNull_pattern_expression() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool SomeComparison(TestMe other) => other is null || (Value != other.Value && Data != other.Data);
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_value_type_and_array_type_AND_comparison_if_array_type_comparison_comes_first() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) && i == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_value_type_and_array_type_OR_comparison_if_array_type_comparison_comes_first() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) || i == 42;
}
");

        [Test]
        public void An_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_NotEquals_Null_expression() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Data == other.Data && Value == other.Value;
}
");

        [Test]
        public void An_issue_is_reported_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_IsNotNull_pattern_expression() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Data == other.Data && Value == other.Value;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_parenthesized_value_type_and_collection_type_comparison() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData))) && Id == other.Id;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_value_type_and_array_type_AND_comparison_if_array_type_comparison_comes_first()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) && i == 42;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 && array.SequenceEqual(Array.Empty<int>());
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_value_type_and_array_type_OR_comparison_if_array_type_comparison_comes_first()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => array.SequenceEqual(Array.Empty<int>()) || i == 42;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(int i, int[] array) => i == 42 || array.SequenceEqual(Array.Empty<int>());
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_NotEquals_Null_expression()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Data == other.Data && Value == other.Value;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other != null && Value == other.Value && Data == other.Data;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complex_class_with_value_type_and_array_type_OR_comparison_if_reference_type_comparison_comes_first_and_IsNotNull_pattern_expression()
        {
            const string OriginalCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Data == other.Data && Value == other.Value;
}
";

            const string FixedCode = @"
using System;
using System.Linq;

public class TestMe : IEquatable<TestMe>
{
    public int Value { get; set; }
    public object Data { get; set; }

    public bool Equals(TestMe other) => other is not null && Value == other.Value && Data == other.Data;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_parenthesized_value_type_and_collection_type_comparison()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData))) && Id == other.Id;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public int Id { get; set; }
    public List<Data> Data { get; set; }

    public bool Equals(TestMe other)
    {
        return Id == other.Id && ((Data == other.Data) || (Data != null && Data.SequenceEqual(otherData)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        //// TODO RKN: Add more complex AND/OR Comparisons with at least 3 comparisons and parenthesized ones

        protected override string GetDiagnosticId() => MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5018_CodeFixProvider();
    }
}