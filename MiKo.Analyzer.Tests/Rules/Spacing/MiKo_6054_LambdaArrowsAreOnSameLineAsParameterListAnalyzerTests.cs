using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6054_LambdaArrowsAreOnSameLineAsParameterListAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_simple_lambda_body_that_has_its_body_on_another_line_as_the_arrow() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_simple_lambda_body_that_has_its_parameter_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i
                                     => { return 42; });
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
        public void No_issue_is_reported_for_parenthesized_lambda_body_that_has_its_body_on_another_line_as_the_arrow() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) =>
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
        public void An_issue_is_reported_for_parenthesized_lambda_body_that_has_its_parameter_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       => { return 42; });
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_body_that_has_both_its_parameter_and_body_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       =>
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
        public void An_issue_is_reported_for_simple_lambda_expression_body_that_has_its_expression_body_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_simple_lambda_expression_body_that_has_its_parameter_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_simple_lambda_expression_body_that_has_both_its_parameter_and_expression_body_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore(i
                                     =>
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
        public void No_issue_is_reported_for_parenthesized_lambda_expression_body_that_spans_single_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) => 42);
        }

        private int DoSomethingCore(Func<int, int> callback)
        {
            return callback(42);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_its_expression_body_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) =>
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
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_its_parameter_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
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
        public void An_issue_is_reported_for_parenthesized_lambda_expression_body_that_has_both_its_parameter_and_expression_body_on_another_line_as_the_arrow() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       =>
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
        public void Code_gets_fixed_for_simple_lambda_body_that_has_its_parameter_on_another_line_as_the_arrow()
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
                                     => { return 42; });
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
            return DoSomethingCore(i => { return 42; });
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
        public void Code_gets_fixed_for_parenthesized_lambda_body_that_has_its_parameter_on_another_line_as_the_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       => { return 42; });
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
            return DoSomethingCore((i) => { return 42; });
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
        public void Code_gets_fixed_for_parenthesized_lambda_body_that_has_both_its_parameter_and_body_on_another_line_as_the_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       =>
                                          { return 42; });
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
            return DoSomethingCore((i) => { return 42; });
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
        public void Code_gets_fixed_for_simple_lambda_expression_body_that_has_its_expression_body_on_another_line_as_the_arrow()
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
        public void Code_gets_fixed_for_simple_lambda_expression_body_that_has_its_parameter_on_another_line_as_the_arrow()
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
        public void Code_gets_fixed_for_simple_lambda_expression_body_that_has_both_its_parameter_and_expression_body_on_another_line_as_the_arrow()
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
                                     =>
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
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_that_has_its_expression_body_on_another_line_as_the_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i) =>
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
            return DoSomethingCore((i) => 42);
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
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_that_has_its_parameter_on_another_line_as_the_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
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
            return DoSomethingCore((i) => 42);
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
        public void Code_gets_fixed_for_parenthesized_lambda_expression_body_that_has_both_its_parameter_and_expression_body_on_another_line_as_the_arrow()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return DoSomethingCore((i)
                                       =>
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
            return DoSomethingCore((i) => 42);
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

        protected override string GetDiagnosticId() => MiKo_6054_LambdaArrowsAreOnSameLineAsParameterListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6054_LambdaArrowsAreOnSameLineAsParameterListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6054_CodeFixProvider();
    }
}