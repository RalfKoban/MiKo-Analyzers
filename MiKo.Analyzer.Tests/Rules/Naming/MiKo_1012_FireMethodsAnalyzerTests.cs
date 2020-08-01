using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1012_FireMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("Raise")]
        [TestCase("Firewall")]
        [TestCase("_firewall")]
        public void No_issue_is_reported_for_correctly_named_method_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("OnFire")]
        [TestCase("FireEvent")]
        [TestCase("DoFireSomething")]
        [TestCase("IsFiringSomething")]
        [TestCase("_fire")]
        [TestCase("_firing")]
        public void An_issue_is_reported_for_wrong_named_method_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("FireEvent", "RaiseEvent")]
        [TestCase("OnEventFired", "OnEventRaised")]
        [TestCase("IsFiringEvent", "IsRaisingEvent")]
        [TestCase("_fire_", "_raise_")]
        [TestCase("_fired_", "_raised_")]
        [TestCase("_fires_", "_raises_")]
        [TestCase("_firing_", "_raising_")]
        public void Code_gets_fixed_(string method, string wanted) => VerifyCSharpFix(
                                                                                 @"using System; class TestMe { void " + method + "() { } }",
                                                                                 @"using System; class TestMe { void " + wanted + "() { } }");

        protected override string GetDiagnosticId() => MiKo_1012_FireMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1012_FireMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1012_CodeFixProvider();
    }
}