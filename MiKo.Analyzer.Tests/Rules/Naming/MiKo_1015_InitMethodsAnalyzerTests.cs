﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1015_InitMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("DoSomething")]
        [TestCase("Initialize")]
        [TestCase("InitializeSomething")]
        public void No_issue_is_reported_for_correctly_named_method_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("DoSomething")]
        [TestCase("Initialize")]
        [TestCase("InitializeSomething")]
        public void No_issue_is_reported_for_correctly_named_local_function_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("Init")]
        [TestCase("Init2")]
        [TestCase("InitSomething")]
        public void An_issue_is_reported_for_wrong_named_method_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("Init")]
        [TestCase("Init2")]
        [TestCase("InitSomething")]
        public void An_issue_is_reported_for_wrong_named_local_function_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("class TestMe { void Init() { } }", "class TestMe { void Initialize() { } }")]
        [TestCase("class TestMe { void Init2() { } }", "class TestMe { void Initialize2() { } }")]
        [TestCase("class TestMe { void InitSomething() { } }", "class TestMe { void InitializeSomething() { } }")]
        public void Code_gets_fixed_for_method_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [TestCase("class TestMe { void Something() { void Init() { } } }", "class TestMe { void Something() { void Initialize() { } } }")]
        [TestCase("class TestMe { void Something() { void Init2() { } } }", "class TestMe { void Something() { void Initialize2() { } } }")]
        [TestCase("class TestMe { void Something() { void InitSomething() { } } }", "class TestMe { void Something() { void InitializeSomething() { } } }")]
        public void Code_gets_fixed_for_local_function_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1015_InitMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1015_InitMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1015_CodeFixProvider();
    }
}