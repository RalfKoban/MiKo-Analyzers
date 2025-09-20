using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_ternary_operator_that_is_on_single_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition ? true : false);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ternary_operator_if_question_and_colon_are_placed_on_same_position_as_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_question_is_placed_on_position_before_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                   ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_question_is_placed_on_position_after_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                     ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_colon_is_placed_on_position_before_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                   : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_colon_is_placed_on_position_after_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                     : false);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_question_is_placed_on_position_before_condition()
        {
            const string OriginalCode = """

                                        using System;

                                        public class TestMe
                                        {
                                            public void DoSomething(bool condition)
                                            {
                                                DoSomething(condition
                                                           ? true
                                                            : false);
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     public class TestMe
                                     {
                                         public void DoSomething(bool condition)
                                         {
                                             DoSomething(condition
                                                         ? true
                                                         : false);
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_question_is_placed_on_position_after_condition()
        {
            const string OriginalCode = """

                                        using System;

                                        public class TestMe
                                        {
                                            public void DoSomething(bool condition)
                                            {
                                                DoSomething(condition
                                                             ? true
                                                            : false);
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     public class TestMe
                                     {
                                         public void DoSomething(bool condition)
                                         {
                                             DoSomething(condition
                                                         ? true
                                                         : false);
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_colon_is_placed_on_position_before_condition()
        {
            const string OriginalCode = """

                                        using System;

                                        public class TestMe
                                        {
                                            public void DoSomething(bool condition)
                                            {
                                                DoSomething(condition
                                                            ? true
                                                           : false);
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     public class TestMe
                                     {
                                         public void DoSomething(bool condition)
                                         {
                                             DoSomething(condition
                                                         ? true
                                                         : false);
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_colon_is_placed_on_position_after_condition()
        {
            const string OriginalCode = """

                                        using System;

                                        public class TestMe
                                        {
                                            public void DoSomething(bool condition)
                                            {
                                                DoSomething(condition
                                                            ? true
                                                             : false);
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     public class TestMe
                                     {
                                         public void DoSomething(bool condition)
                                         {
                                             DoSomething(condition
                                                         ? true
                                                         : false);
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_colon_is_placed_on_same_line_as_closing_bracket_of_initializer()
        {
            const string OriginalCode = """

                                        using System;

                                        public class Item
                                        {
                                            public int Id { get; init; }
                                            public string Name { get; init; }
                                        }

                                        public class TestMe
                                        {
                                            public Item Item { get; set; }

                                            public void DoSomething(object item)
                                            {
                                                Item = item != null
                                                    ? new Item
                                                    {
                                                        Id = 42,
                                                        Name = "Whatever",
                                                    } : null
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     using System;

                                     public class Item
                                     {
                                         public int Id { get; init; }
                                         public string Name { get; init; }
                                     }

                                     public class TestMe
                                     {
                                         public Item Item { get; set; }

                                         public void DoSomething(object item)
                                         {
                                             Item = item != null
                                                    ? new Item
                                                 {
                                                     Id = 42,
                                                     Name = "Whatever",
                                                 }
                                                    : null
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_question_mark_and_colon_are_placed_at_end_of_lines_and_string_is_returned_for_multiline_condition()
        {
            const string OriginalCode = """

                                        public record TestMe
                                        {
                                            public string Name { get; init; } = "";
                                            public string Id { get; init; } = "";
                                            public string Description { get; init; } = "";
                                            
                                            public string GetInformation(string s) => s;

                                            public override string ToString()
                                            {
                                                return string.Compare(Name, Description,
                                                    StringComparison.OrdinalIgnoreCase) == 0 ?        
                                                    GetInformation(Id) :                  
                                                    "whatever";
                                            }
                                        }

                                        """;

            const string FixedCode = """

                                     public record TestMe
                                     {
                                         public string Name { get; init; } = "";
                                         public string Id { get; init; } = "";
                                         public string Description { get; init; } = "";
                                         
                                         public string GetInformation(string s) => s;
                                     
                                         public override string ToString()
                                         {
                                             return string.Compare(Name, Description,
                                                 StringComparison.OrdinalIgnoreCase) == 0
                                                    ? GetInformation(Id)
                                                    : "whatever";
                                         }
                                     }

                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6031_CodeFixProvider();
    }
}