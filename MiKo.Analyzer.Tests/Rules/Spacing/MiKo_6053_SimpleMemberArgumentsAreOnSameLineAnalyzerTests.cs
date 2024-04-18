using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_without_arguments() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_number_as_argument() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_simple_member_as_argument_when_on_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.MaxValue);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_simple_member_as_argument_when_name_is_on_other_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.
                      MaxValue);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_simple_member_as_argument_when_operator_is_on_other_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int
                      .MaxValue);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_simple_member_as_argument_when_name_and_operator_are_on_other_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int
                      .
                      MaxValue);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_simple_member_as_argument_when_name_is_on_other_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.
                      MaxValue);
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.MaxValue);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_simple_member_as_argument_when_operator_is_on_other_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int
                      .MaxValue);
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.MaxValue);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_simple_member_as_argument_when_name_and_operator_are_on_other_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int
                      .
                      MaxValue);
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        DoSomething(int.MaxValue);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6053_CodeFixProvider();
    }
}