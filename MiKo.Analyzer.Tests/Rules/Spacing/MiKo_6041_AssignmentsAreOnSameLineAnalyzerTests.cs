using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6041_AssignmentsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
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

        protected override string GetDiagnosticId() => MiKo_6041_AssignmentsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6041_AssignmentsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6041_CodeFixProvider();
    }
}