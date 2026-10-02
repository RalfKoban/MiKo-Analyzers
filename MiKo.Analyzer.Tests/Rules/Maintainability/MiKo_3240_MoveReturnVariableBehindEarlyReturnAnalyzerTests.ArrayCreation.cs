using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_value_within_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null)
            {
                return new int[0];
            }

            var condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_directly_below_condition() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null)
                return new int[0];

            var condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_on_same_line_as_if_statement() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null) return new int[0];

            var condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_within_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
            {
                return condition;
            }

            condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_directly_below_condition() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
                return condition;

            condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null) return condition;

            condition = new int[1];

            return condition;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_within_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
            {
                return condition;
            }

            condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null)
            {
                return new int[0];
            }

            var condition = new int[0];

            condition = new int[1];

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_directly_below_condition()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
                return condition;

            condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null)
                return new int[0];

            var condition = new int[0];

            condition = new int[1];

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null) return condition;

            condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null) return new int[0];

            var condition = new int[0];

            condition = new int[1];

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_value_within_block_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null)
            {
                return new int[0];
            }
            else
            {
                var condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_directly_below_condition_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null)
                return new int[0];
            else
            {
                var condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_array_creation_return_value_and_direct_return_on_same_line_as_if_statement_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            if (o is null) return new int[0];
            else
            {
                var condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_within_block_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
                return condition;
            else
            {
                condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null) return condition;
            else
            {
                condition = new int[1];

                return condition;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_within_block_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null)
            {
                return new int[0];
            }
            else
            {
                var condition = new int[0];
                condition = new int[1];

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null)
                return condition;
            else
            {
                condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null)
                return new int[0];
            else
            {
                var condition = new int[0];
                condition = new int[1];

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_array_creation_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int[] DoSomething(object o)
        {
            var condition = new int[0];

            if (o is null) return condition;
            else
            {
                condition = new int[1];

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
        public int[] DoSomething(object o)
        {
            if (o is null) return new int[0];
            else
            {
                var condition = new int[0];
                condition = new int[1];

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
