using System;
using System.ComponentModel;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_thrown_([Values(nameof(ArgumentException), nameof(ArgumentNullException))] string exceptionName)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_exception_thrown_in_switch_default_clause_([Values(nameof(ArgumentOutOfRangeException), nameof(InvalidEnumArgumentException))] string exceptionName)
            => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(int x)
    {
        switch (x)
        {
            default:
                throw " + exceptionName + @"();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_exception_thrown_in_switch_default_clause_([Values(nameof(ArgumentException), nameof(ArgumentNullException))] string exceptionName)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        switch (x)
        {
            case 1: break;
            case 2:
            default:
                throw new " + exceptionName + @"();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_parameterless_exception_([Values(nameof(ArgumentException), nameof(ArgumentNullException))] string exceptionName)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        switch (x)
        {
            case 1: break;
            case 2:
            default:
                throw new ###;
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", exceptionName + "()"), Template.Replace("###", @"ArgumentOutOfRangeException(nameof(x), x, ""TODO"")"));
        }

        protected override string GetDiagnosticId() => MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3013_CodeFixProvider();
    }
}