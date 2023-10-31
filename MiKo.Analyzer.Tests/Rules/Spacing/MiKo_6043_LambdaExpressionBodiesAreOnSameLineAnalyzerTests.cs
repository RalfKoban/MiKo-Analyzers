using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_simple_lambda_body_that_spans_single_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => { return 42; });
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_lambda_body_that_spans_multiple_lines() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 
                                        { return 42; });
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_body_that_spans_single_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) => { return 42; });
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_body_that_spans_multiple_lines() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) => 
                                        { return 
                                                 42; });
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_lambda_expression_body_that_spans_single_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_lambda_expression_body_that_contains_an_Initializer_expression_and_spans_multiple_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int SomeProperty { get; set; }

        public TestMe DoSomething()
        {
            return DoSomethingCore(i => new TestMe
                                            {
                                                SomeProperty = i,
                                            });
        }

        private TestMe DoSomethingCore(Func<int, TestMe> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_lambda_expression_body_that_contains_a_nested_lambda_with_an_Initializer_expression_and_spans_multiple_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int SomeProperty { get; set; }

        public TestMe DoSomething()
        {
            return DoSomethingCore(i => DoSomethingCore(j => new TestMe
                                                                 {
                                                                       SomeProperty = j,
                                                                 }));
        }

        private TestMe DoSomethingCore(Func<int, TestMe> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_lambda_expression_body_that_contains_a_deeply_nested_lambda_with_an_Initializer_expression_and_spans_multiple_line() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public int SomeProperty { get; set; }

        public TestMe DoSomething()
        {
            var items = new List<int>();

            return DoSomethingCore(i => items.Select(x => new TestMe
                                                          {
                                                              SomeProperty = i + x,
                                                          })
                                             .ToList());
        }

        private TestMe DoSomethingCore(Func<int, IEnumerable<TestMe>> callback)
        {
            return callback(42).FirstOrDefault();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_single_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_and_contains_an_object_creation_with_initializer() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Property1 { get; set; }

        public int Property2 { get; set; }

        public int DoSomething()
        {
            return DoSomethingCore(() => new TestMe
                                             {
                                                 Property1 = 1,
                                                 Property2 = 2,
                                             });
        }

        private TestMe DoSomethingCore(Func<TestMe> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_and_contains_an_object_creation_with_argumentList() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Property1 { get; set; }

        public int Property2 { get; set; }

        public int DoSomething()
        {
            return DoSomethingCore(() => new TestMe(
                                                 Property1 = 1,
                                                 Property2 = 2)
                                  );
        }

        private TestMe DoSomethingCore(Func<TestMe> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_with_logical_expressions_that_spans_multiple_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool a, bool b, bool c)
        {
            return DoSomethingCore(() => a
                                         && b
                                         || c);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_with_multiple_invocation_expressions_that_span_multiple_line_and_would_be_too_long_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(TestMe other)
        {
            return DoSomethingCore(() => other.SomethingVeryLongThatReturnsMe()
                                              .SomethingVeryLongThatReturnsMe()
                                              .SomethingVeryLongThatReturnsMe()
                                              .SomethingVeryLongThatReturnsMe()
                                              .SomethingVeryLongThatReturnsMe()
                                              .SomeValue);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }

        private TestMe SomethingVeryLongThatReturnsMe() => this;

        private int SomeValue { get; }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_simple_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_before_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i 
                                     => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_simple_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 
                                        42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_before_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() 
                                     => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 
                                            42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_multi_parameter_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((x, y) => 
                                            x
                                                +
                                                    y);
        }

        private int DoSomethingCore(Func<int, int, int> callback)
        {
            return callback();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_simple_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_before_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i 
                                     => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_simple_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 
                                        42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_before_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() 
                                     => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 
                                            42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(() => 42);
        }

        private int DoSomethingCore(Func<int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_2_parameter_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((x, y) => 
                                            x
                                                +
                                                    y);
        }

        private int DoSomethingCore(Func<int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((x, y) => x + y);
        }

        private int DoSomethingCore(Func<int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_3_parameter_lambda_expression_body_that_spans_multiple_lines_if_line_break_is_after_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((x, y, z) => 
                                                x
                                                  +
                                                    y
                                                +
                                       z);
        }

        private int DoSomethingCore(Func<int, int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((x, y, z) => x + y + z);
        }

        private int DoSomethingCore(Func<int, int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_with_invocation_that_spans_multiple_lines()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y, z) => 
                                                TestMe
                                                    .DoSomething(
                                                                x,
                                                                    y,
                                                                        z));
        }

        private int DoSomethingCore(Func<int, int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y, z) => TestMe.DoSomething(x, y, z));
        }

        private int DoSomethingCore(Func<int, int, int, int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_with_invocation_whose_parameters_span_multiple_lines()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y, z) => TestMe.DoSomething(x,
                                                                       string.Format(""{0} is some very strange number, {1} would fit much better whereas {2} would be best"", a, b, c),
                                                                            z));
        }

        private int DoSomethingCore(Func<int, string, int, int> callback)
        {
            return callback();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y, z) => TestMe.DoSomething(x, string.Format(""{0} is some very strange number, {1} would fit much better whereas {2} would be best"", a, b, c), z));
        }

        private int DoSomethingCore(Func<int, string, int, int> callback)
        {
            return callback();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_lambda_expression_body_with_invocation_whose_parameters_span_multiple_lines()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y) => TestMe.DoSomethingCore((d, e) => TestMe.DoSomething(
                                                                                                e,
                                                                                                d,
                                                                                                y))));
        }

        private static int DoSomethingCore(Func<int, int, int> callback)
        {
            return callback(1, 2);
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething(int a, int b, int c)
        {
            return DoSomethingCore((x, y) => TestMe.DoSomethingCore((d, e) => TestMe.DoSomething(e, d, y))));
        }

        private static int DoSomethingCore(Func<int, int, int> callback)
        {
            return callback(1, 2);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6043_CodeFixProvider();
    }
}