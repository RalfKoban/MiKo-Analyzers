using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3502_DoNotUseSuppressNullableWarningOnLinqCallAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] LinqCalls =
                                                     {
                                                         nameof(Enumerable.DefaultIfEmpty),
                                                         nameof(Enumerable.ElementAtOrDefault),
                                                         nameof(Enumerable.FirstOrDefault),
                                                         nameof(Enumerable.LastOrDefault),
                                                         nameof(Enumerable.SingleOrDefault),
                                                     };

        [Test]
        public void No_issue_is_reported_for_Nullable_suppression() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = o!.ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Linq_call_without_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IEnumerable<object> items)
    {
        var s = items." + linqCall + @"();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_embedded_Linq_call_without_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IEnumerable<object> items)
    {
        var s = items." + linqCall + @"().ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Linq_call_with_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"()!;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_embedded_Linq_call_with_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"()!.ToString();
    }
}
");

        [Test]
        public void Code_gets_fixed_for_Linq_call_with_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"()!;
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_embedded_Linq_call_with_Nullable_suppression_([ValueSource(nameof(LinqCalls))] string linqCall)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"()!.ToString();
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        var s = items." + linqCall + @"().ToString();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3502_DoNotUseSuppressNullableWarningOnLinqCallAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3502_DoNotUseSuppressNullableWarningOnLinqCallAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3502_CodeFixProvider();
    }
}