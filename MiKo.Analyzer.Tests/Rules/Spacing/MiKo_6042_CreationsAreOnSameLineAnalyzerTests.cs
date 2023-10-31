using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6042_CreationsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_creation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new object();
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
        DoSomething(new object());
    }
}
");

        [Test]
        public void An_issue_is_reported_if_new_keyword_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new
                    object();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_new_keyword_as_argument_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething(new
                        object());
    }
}
");

        [Test]
        public void Code_gets_fixed_if_new_keyword_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new
                    object();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = new object();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_new_keyword_as_argument_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething(new
                        object());
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        DoSomething(new object());
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6042_CreationsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6042_CreationsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6042_CodeFixProvider();
    }
}