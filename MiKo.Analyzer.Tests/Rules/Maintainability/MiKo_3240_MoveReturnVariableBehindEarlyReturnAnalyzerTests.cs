using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_within_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition)
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_directly_below_condition() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition)
                return;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_on_same_line_as_if_statement() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition) return;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_value_within_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null)
            {
                return false;
            }

            var condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_directly_below_condition() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null)
                return false;

            var condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_on_same_line_as_if_statement() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null) return false;

            var condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_within_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
            {
                return condition;
            }

            condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_directly_below_condition() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
                return condition;

            condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null) return condition;

            condition = true;

            return condition;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_within_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
            {
                return condition;
            }

            condition = true;

            return condition;
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
        public bool DoSomething(object o)
        {
            if (o is null)
            {
                return false;
            }

            var condition = false;

            condition = true;

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_directly_below_condition()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
                return condition;

            condition = true;

            return condition;
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
        public bool DoSomething(object o)
        {
            if (o is null)
                return false;

            var condition = false;

            condition = true;

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null) return condition;

            condition = true;

            return condition;
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
        public bool DoSomething(object o)
        {
            if (o is null) return false;

            var condition = false;

            condition = true;

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_within_block_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition)
            {
                Console.WriteLine();
            }
            else
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_directly_below_condition_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition)
                Console.WriteLine();
            else
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_void_return_value_and_return_on_same_line_as_if_statement_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = false;

            if (condition) Console.WriteLine();
            else
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_value_within_block_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null)
            {
                return false;
            }
            else
            {
                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_directly_below_condition_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null)
                return false;
            else
            {
                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_bool_return_value_and_direct_return_on_same_line_as_if_statement_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            if (o is null) return false;
            else
            {
                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_within_block_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
                return condition;
            else
            {
                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_bool_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null) return condition;
            else
            {
                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_within_block_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = true;

                return condition;
            }
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
        public bool DoSomething(object o)
        {
            if (o is null)
            {
                return false;
            }
            else
            {
                var condition = false;
                condition = true;

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null)
                return condition;
            else
            {
                condition = true;

                return condition;
            }
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
        public bool DoSomething(object o)
        {
            if (o is null)
                return false;
            else
            {
                var condition = false;
                condition = true;

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_bool_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(object o)
        {
            var condition = false;

            if (o is null) return condition;
            else
            {
                condition = true;

                return condition;
            }
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
        public bool DoSomething(object o)
        {
            if (o is null) return false;
            else
            {
                var condition = false;
                condition = true;

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3240_CodeFixProvider();
    }
}