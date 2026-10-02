using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3240_MoveReturnVariableBehindEarlyReturnAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_property_getter_with_direct_return_value_within_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
                {
                    return false;
                }

                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_with_direct_return_directly_below_condition() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
                    return false;

                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_with_direct_return_on_same_line_as_if_statement() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null) return false;

                var condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_within_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                {
                    return condition;
                }

                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_directly_below_condition() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                    return condition;

                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_on_same_line_as_if_statement() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null) return condition;

                condition = true;

                return condition;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_within_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                {
                    return condition;
                }

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
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
                {
                    return false;
                }

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
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_directly_below_condition()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                    return condition;

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
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
                    return false;

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
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_on_same_line_as_if_statement()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null) return condition;

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
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null) return false;

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
        public void No_issue_is_reported_for_property_getter_with_direct_return_value_within_block_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
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
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_with_direct_return_directly_below_condition_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
                    return false;
                else
                {
                    var condition = true;

                    return condition;
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_with_direct_return_on_same_line_as_if_statement_in_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null) return false;
                else
                {
                    var condition = true;

                    return condition;
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_within_block_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
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
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_directly_below_condition_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                    return condition;
                else
                {
                    condition = true;

                    return condition;
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null) return condition;
                else
                {
                    condition = true;

                    return condition;
                }
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_within_block_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
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
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
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
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_directly_below_condition_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null)
                    return condition;
                else
                {
                    condition = true;

                    return condition;
                }
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
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null)
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
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_getter_with_variable_before_condition_with_return_on_same_line_as_if_statement_in_else_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object _o;

        public bool DoSomething
        {
            get
            {
                var condition = false;

                if (_o is null) return condition;
                else
                {
                    condition = true;

                    return condition;
                }
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
        private object _o;

        public bool DoSomething
        {
            get
            {
                if (_o is null) return false;
                else
                {
                    var condition = false;
                    condition = true;

                    return condition;
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}