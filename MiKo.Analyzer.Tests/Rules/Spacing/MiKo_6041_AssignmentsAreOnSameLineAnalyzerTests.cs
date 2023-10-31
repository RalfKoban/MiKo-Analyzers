using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6041_AssignmentsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssignmentOperators = { "+=", "-=" };

        [Test]
        public void No_issue_is_reported_if_assignment_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_assignment_to_array_is_on_other_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int[] x =
                  {
                    1,
                    2,
                    3,
                  };
    }
}
");

        [Test]
        public void An_issue_is_reported_if_assignment_of_value_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = 
                o;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_assignment_with_equals_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x 
              = o;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_specific_assignment_is_on_same_line_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @" 1;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_specific_assignment_of_value_is_on_different_line_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @"
                1;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_specific_assignment_with_equals_is_on_different_line_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i
          " + assignmentOperator + @" 1;
    }
}
");

        [Test]
        public void Code_gets_fixed_if_assignment_of_value_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = 
                o;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_assignment_with_equals_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x 
              = o;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_complete_assignment_is_spread_over_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x 
              =
                o;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_specific_assignment_of_value_is_on_different_line_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @"
                1;
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @" 1;
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_specific_assignment_with_equals_is_on_different_line_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @"
                1;
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @" 1;
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_specific_assignment_is_spread_over_different_lines_([ValueSource(nameof(AssignmentOperators))] string assignmentOperator)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i 
          " + assignmentOperator + @"
                1; // some comment
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0;

        i " + assignmentOperator + @" 1; // some comment
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_assignment_with_comment_spans_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i =

                // some comment
                0;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        int i = 0; // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_field_assignment_with_comment_spans_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int m_field =

                            // some comment
                            0;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int m_field = 0; // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6041_AssignmentsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6041_AssignmentsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6041_CodeFixProvider();
    }
}