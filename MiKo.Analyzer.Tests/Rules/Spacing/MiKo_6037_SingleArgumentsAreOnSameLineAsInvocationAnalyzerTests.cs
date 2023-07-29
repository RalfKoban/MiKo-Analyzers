using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6037_SingleArgumentsAreOnSameLineAsInvocationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_call_with_single_argument_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(1);
    }
}
");

        [Test]
        public void No_issue_is_reported_if_complete_call_with_multiple_arguments_are_on_different_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(1,
                   GCCollectionMode.Default);
    }
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_and_argument_are_on_different_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(
                   1);
    }
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_and_argument_are_on_different_lines_and_closing_parenthesis_is_on_same_line_as_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(
                   1);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(1);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_and_argument_are_on_different_lines_and_closing_parenthesis_is_on_other_line_than_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(
                   1
                    );
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        GC.Collect(1);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6037_SingleArgumentsAreOnSameLineAsInvocationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6037_SingleArgumentsAreOnSameLineAsInvocationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6037_CodeFixProvider();
    }
}