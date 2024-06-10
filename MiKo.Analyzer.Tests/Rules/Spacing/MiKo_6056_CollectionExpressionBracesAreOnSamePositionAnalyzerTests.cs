using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6056_CollectionExpressionBracesAreOnSamePositionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [1, 2, 3];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_placed_on_same_position_as_hypothetical_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                            [
                                1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_placed_on_same_position_as_hypothetical_type_but_outdented_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
    [
        1,
        2,
        3
    ];
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_placed_on_position_before_position_of_hypothetical_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                          [
                                1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_placed_on_position_after_position_of_hypothetical_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                             [
                                1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_with_collection_expression_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething([1, 2, 3]);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_with_collection_expression_open_bracket_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething([
                        1,
                        2,
                        3
                    ]);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_value_with_collection_expression_that_contains_a_spread_element() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static string[] GetSomething() => [.. new HashSet<string>([
                                                                          ""value1"",
                                                                          ""value2"",
                                                                      ]).OrderBy(_ => _)];
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething(
[
                        1,
                        2,
                        3
                    ]);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething(
                        [
                            1,
                            2,
                            3
                        ]);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_placed_on_position_before_position_of_hypothetical_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                          [
                                1,
                                2,
                                3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                            [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_placed_on_position_after_position_of_hypothetical_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                             [
                                1,
                                2,
                                3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                            [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_placed_on_same_position_as_hypothetical_type_but_outdented_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField =
    [
        1,
        2,
        3
    ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                            [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething(
[
                        1,
                        2,
                        3
                    ]);
    }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething([
                        1,
                        2,
                        3
                    ]);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_1st_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething(
                        [
                            1,
                            2,
                            3
                        ]);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething([
                        1,
                        2,
                        3
                    ]);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_2nd_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(float value, int[] values)
    {
        DoSomething(
                    float.MinValue,
                        [
                            1,
                            2,
                            3
                        ]);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(float value, int[] values)
    {
        DoSomething(
                    float.MinValue,
                    [
                        1,
                        2,
                        3
                    ]);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_middle_parameter_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(float minValue, int[] values, float maxValue)
    {
        DoSomething(
                    float.MinValue,
                        [
                            1,
                            2,
                            3
                        ],
                    float.MaxValue);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(float minValue, int[] values, float maxValue)
    {
        DoSomething(
                    float.MinValue,
                    [
                        1,
                        2,
                        3
                    ],
                    float.MaxValue);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6056_CollectionExpressionBracesAreOnSamePositionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6056_CollectionExpressionBracesAreOnSamePositionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6056_CodeFixProvider();
    }
}