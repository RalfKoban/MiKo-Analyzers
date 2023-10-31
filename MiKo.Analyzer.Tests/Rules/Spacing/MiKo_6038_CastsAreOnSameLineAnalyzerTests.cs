using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6038_CastsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_cast_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = (int)o;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_cast_as_argument_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething((int)o);
    }

    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_if_cast_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = (int)
                    o;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_cast_as_argument_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething((int)
                        o);
    }

    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_if_cast_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = (int)
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
        var x = (int)o;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_cast_as_argument_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething((int)
                        o);
    }

    public void DoSomething(int i)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething((int)o);
    }

    public void DoSomething(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6038_CastsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6038_CastsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6038_CodeFixProvider();
    }
}