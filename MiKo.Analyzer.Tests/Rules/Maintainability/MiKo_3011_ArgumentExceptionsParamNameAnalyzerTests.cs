using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests : CodeFixVerifier
    {
        [TestCase("\"x\", 42, typeof(StringComparison)")]
        [TestCase("nameof(x), 42, typeof(StringComparison)")]
        public void No_issue_is_reported_for_correctly_thrown_InvalidEnumArgumentException_(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison x)
    {
        if (x == StringComparison.OrdinalIgnoreCase) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, typeof(StringComparison)")]
        [TestCase("\"some message\", 42, \"some message\"")]
        public void An_issue_is_reported_for_incorrectly_thrown_InvalidEnumArgumentException_(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        // TODO RKN: Codefix for InvalidEnumArgumentException
        protected override string GetDiagnosticId() => MiKo_3011_ArgumentExceptionsParamNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3011_ArgumentExceptionsParamNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3011_CodeFixProvider();
    }
}