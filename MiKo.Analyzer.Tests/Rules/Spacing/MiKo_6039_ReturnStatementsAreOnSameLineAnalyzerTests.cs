using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6039_ReturnStatementsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_there_is_no_return_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is null) return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_return_value_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        return o;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_return_value_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        return
               o;
    }
}
");

        [Test]
        public void Code_gets_fixed_if_return_value_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        return
               o;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public object DoSomething(object o)
    {
        return o;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6039_ReturnStatementsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6039_ReturnStatementsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6039_CodeFixProvider();
    }
}