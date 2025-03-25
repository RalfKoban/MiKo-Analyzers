using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6064_PropertyInvocationIsOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_invocation_on_property_is_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        var result = someObject.SomePropertyA
                               .SomePropertyB
                               .SomePropertyC
                               .ToString();
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_if_invocation_on_property_is_on_other_line() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        var result = someObject
                            .SomePropertyA
                            .SomePropertyB
                            .SomePropertyC
                            .ToString();
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_if_invocation_on_property_is_on_different_line()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        var result = someObject
                            .SomePropertyA
                            .SomePropertyB
                            .SomePropertyC
                            .ToString();
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        var result = someObject.SomePropertyA
                            .SomePropertyB
                            .SomePropertyC
                            .ToString();
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_invocation_is_on_different_line_inside_lambda()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        DoSomethingCore(_ =>
                             {
                                 TestMe
                                    .SomePropertyA
                                    .SomePropertyB
                                    .SomePropertyC
                                    .ToString();
                             });
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }

    private void DoSomethingCore(Action<object> callback) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(TestMe someObject)
    {
        DoSomethingCore(_ =>
                             {
                                 TestMe.SomePropertyA
                                    .SomePropertyB
                                    .SomePropertyC
                                    .ToString();
                             });
    }

    private TestMe SomePropertyA { get; set; }

    private TestMe SomePropertyB { get; set; }

    private TestMe SomePropertyC { get; set; }

    private void DoSomethingCore(Action<object> callback) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6064_PropertyInvocationIsOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6064_PropertyInvocationIsOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6064_CodeFixProvider();
    }
}