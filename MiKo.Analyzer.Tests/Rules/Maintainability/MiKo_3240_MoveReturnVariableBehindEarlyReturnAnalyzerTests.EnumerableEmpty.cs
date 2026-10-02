using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_value_within_block() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
            {
                return Enumerable.Empty<int>();
            }

            IEnumerable<int> condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_directly_below_condition() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
                return Enumerable.Empty<int>();

            IEnumerable<int> condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_on_same_line_as_if_statement() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null) return Enumerable.Empty<int>();

            IEnumerable<int> condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_within_block() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
            {
                return condition;
            }

            condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_directly_below_condition() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
                return condition;

            condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null) return condition;

            condition = new[] { 1 };

            return condition;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_within_block()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
            {
                return condition;
            }

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
            {
                return Enumerable.Empty<int>();
            }

            var condition = Enumerable.Empty<int>();

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_directly_below_condition()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
                return condition;

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
                return Enumerable.Empty<int>();

            var condition = Enumerable.Empty<int>();

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null) return condition;

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null) return Enumerable.Empty<int>();

            var condition = Enumerable.Empty<int>();

            condition = new[] { 1 };

            return condition;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_value_within_block_in_else_block() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
            {
                return Enumerable.Empty<int>();
            }
            else
            {
                IEnumerable<int> condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_directly_below_condition_in_else_block() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
                return Enumerable.Empty<int>();
            else
            {
                IEnumerable<int> condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_direct_return_on_same_line_as_if_statement_in_else_block() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null) return Enumerable.Empty<int>();
            else
            {
                IEnumerable<int> condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_within_block_in_else_block() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
                return condition;
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null) return condition;
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_within_block_in_else_block()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
            {
                return condition;
            }
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
            {
                return Enumerable.Empty<int>();
            }
            else
            {
                var condition = Enumerable.Empty<int>();
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_directly_below_condition_in_else_block()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null)
                return condition;
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null)
                return Enumerable.Empty<int>();
            else
            {
                var condition = Enumerable.Empty<int>();
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_Enumerable_Empty_return_value_and_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            var condition = Enumerable.Empty<int>();

            if (o is null) return condition;
            else
            {
                condition = new[] { 1 };

                return condition;
            }
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(object o)
        {
            if (o is null) return Enumerable.Empty<int>();
            else
            {
                var condition = Enumerable.Empty<int>();
                condition = new[] { 1 };

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
