﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_call() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_after_variable_declaration() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;
            x = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_after_variable_declaration_and_assignment() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            int x;

            var y = something ? 0 : -1;
            x = DoSomething(something) + y;

            return x;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_in_parenthesized_lambda_assignment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var y = something ? 0 : -1;
            int x;

            Callback(() => x = DoSomething(something) + y);
            Callback(() => y = DoSomething(something) + x);

            return x;
        }

        private void Callback(Action a) => a();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_variable_assignments_as_first_statements_after_variable_declarations() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x, y;
            x = 1;
            y = 2;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_variable_assignments_as_first_statements_inside_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            int x, y;

            switch (something)
            {
                case 0:
                    x = 1;
                    y = 2;

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_assignment_preceded_by_if_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int X { get; set;}

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            X = 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_assignment_followed_by_if_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int X { get; set;}

        public void DoSomething(bool something)
        {
            X = 1;
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer() => No_issue_is_reported_for(@"
using System;
using System.Collections;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<TestCaseData> DoSomething(string phrase, string fixedPhrase)
        {
            yield return new TestCaseData
                             {
                                 Wrong = phrase,
                                 Fixed = fixedPhrase,
                             };
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_setter_with_assignment_to_discarded_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int SomeValue
        {
            get => 0:

            set
            {
                _ = value;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_assignment_to_discarded_value() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int value)
        {
            _ = value;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_assignment_preceded_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;

            if (something)
            {
                // some comment
            }
            x = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_assignment_preceded_by_method_call() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;

            DoSomething();
            x = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_assignment_preceded_by_method_call_inside_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            int x, y;

            switch (something)
            {
                case 0:
                    DoSomething(42);
                    x = 1;

                    break;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_if_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            if (something)
            {
                // some comment
            }
            x = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            if (something)
            {
                // some comment
            }

            x = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_method_call()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            DoSomething(something);
            x = 1;
            DoSomething(something);
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            DoSomething(something);

            x = 1;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_variables_with_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x, y;

            DoSomething(something);
            x = something;
            y = something;
            DoSomething(something);
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x, y;

            DoSomething(something);

            x = something;
            y = something;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;
            DoSomething();
            x = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;
            DoSomething();

            x = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6004_CodeFixProvider();
    }
}