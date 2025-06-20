using System.Diagnostics.CodeAnalysis;

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
        public void No_issue_is_reported_for_field_with_collection_expression_containing_comments() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                                // First element
                                1,
                                // Second element
                                2,
                                3
                            ];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_containing_mixed_types() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object[] MyField = [
                                  1,
                                  ""string"",
                                  3.14
                               ];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_containing_single_element() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [1];
}
");

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
        public void No_issue_is_reported_for_field_with_empty_collection_expression() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_nested_collection_expression() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[][] MyField = [
                                [1, 2],
                                [3, 4]
                              ];
}
");

        [Test]
        public void No_issue_is_reported_for_list_assignment_with_collection_expression_open_bracket_placed_on_other_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                       [
                            1,
                            2,
                            3
                       ];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_list_assignment_with_collection_expression_open_bracket_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values = [
                            1,
                            2,
                            3
                       ];
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
        public void No_issue_is_reported_for_instance_with_nested_instances() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        var testMe = new TestMe
                         {
                             NestedInstances =
                                               [
                                                   new TestMe
                                                       {
                                                           NestedInstances =
                                                                             [
                                                                                 new TestMe(),
                                                                                 new TestMe()
                                                                             ]
                                                        },
                                                   new TestMe
                                                       {
                                                           NestedInstances =
                                                                             [
                                                                                 new TestMe(),
                                                                                 new TestMe()
                                                                             ]
                                                        }
                                               ]
                         };
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_instance_with_nested_instances() => An_issue_is_reported_for(3, @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        var testMe = new TestMe
                         {
                             NestedInstances =
                [
                        new TestMe
                            {
                                NestedInstances =
                                        [
                                            new TestMe(),
                                            new TestMe()
                                        ]
                            },
                        new TestMe
                            {
                                NestedInstances =
                                                            [
                                                                new TestMe(),
                                                                new TestMe()
                                                            ]
                            }
                ]
                         };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_open_bracket_placed_on_same_line_as_equals_sign_but_closed_bracket_placed_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3];
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
        public void An_issue_is_reported_for_list_assignment_with_collection_expression_open_and_close_bracket_placed_on_different_line_outdented_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
[
     1,
     2,
     3
];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_list_assignment_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
[
                        1,
                        2,
                        3
                    ];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_list_assignment_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                        [
                            1,
                            2,
                            3
                        ];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_list_assignment_with_collection_expression_open_bracket_placed_on_same_line_as_equals_sign_but_closed_bracket_placed_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values = [
                            1,
                            2,
                            3];
    }
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
        public void Code_gets_fixed_for_field_with_collection_expression_open_bracket_placed_on_same_line_as_equals_sign_but_closed_bracket_placed_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
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
        public void Code_gets_fixed_for_list_assignment_with_collection_expression_open_and_close_bracket_placed_on_different_line_outdented_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
[
     1,
     2,
     3
];
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                       [
                           1,
                           2,
                           3
                       ];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_assignment_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
[
                        1,
                        2,
                        3
                    ];
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                       [
                           1,
                           2,
                           3
                       ];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_assignment_with_collection_expression_open_bracket_placed_on_different_line_outdented_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                          [
                              1,
                              2,
                              3
                          ];
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values =
                       [
                           1,
                           2,
                           3
                       ];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_assignment_with_collection_expression_open_bracket_placed_on_same_line_as_equals_sign_but_closed_bracket_placed_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values = [
                            1,
                            2,
                            3];
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] values = [
                            1,
                            2,
                            3
                       ];
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

        [Test]
        public void Code_gets_fixed_for_parameter_with_collection_expression_close_bracket_placed_on_different_line_outdented_to_the_left_by_1_and_last_item_with_separator()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int[] values)
    {
        DoSomething([
                        1,
                        2,
                        3,
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
                        3,
                    ]);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parameter_with_collection_expression_close_bracket_placed_on_different_line_outdented_to_the_left_by_1_and_last_item_without_separator()
        {
            const string OriginalCode = @"
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
        public void Code_gets_fixed_for_instance_with_nested_object_creations()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        var testMe = new TestMe
                         {
                             NestedInstances =
                [
                        new TestMe
                            {
                                NestedInstances =
                                        [
                                            new TestMe(),
                                            new TestMe()
                                        ]
                            },
                        new TestMe
                            {
                                NestedInstances =
                                                            [
                                                                new TestMe(),
                                                                new TestMe()
                                                            ]
                            }
                ]
                         };
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        var testMe = new TestMe
                         {
                             NestedInstances =
                                               [
                                                   new TestMe
                                                       {
                                                           NestedInstances =
                                                                             [
                                                                                 new TestMe(),
                                                                                 new TestMe()
                                                                             ]
                                                       },
                                                   new TestMe
                                                       {
                                                           NestedInstances =
                                                                             [
                                                                                 new TestMe(),
                                                                                 new TestMe()
                                                                             ]
                                                       }
                                               ]
                         };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_instance_with_nested_implicit_object_creations()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        TestMe testMe = new()
                            {
                                NestedInstances =
                   [
                           new()
                               {
                                   NestedInstances =
                                           [
                                               new TestMe(),
                                               new TestMe()
                                           ]
                               },
                           new()
                               {
                                   NestedInstances =
                                                               [
                                                                   new TestMe(),
                                                                   new TestMe()
                                                               ]
                               }
                   ]
                            };
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> NestedInstances { get; set; }

    public static void Main()
    {
        TestMe testMe = new()
                            {
                                NestedInstances =
                                                  [
                                                      new()
                                                          {
                                                              NestedInstances =
                                                                                [
                                                                                    new TestMe(),
                                                                                    new TestMe()
                                                                                ]
                                                          },
                                                      new()
                                                          {
                                                              NestedInstances =
                                                                                [
                                                                                    new TestMe(),
                                                                                    new TestMe()
                                                                                ]
                                                          }
                                                  ]
                            };
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