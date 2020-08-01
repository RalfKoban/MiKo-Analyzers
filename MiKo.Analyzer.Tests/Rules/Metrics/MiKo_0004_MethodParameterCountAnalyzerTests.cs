﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0004_MethodParameterCountAnalyzerTests : CodeFixVerifier
    {
        [TestCase("")]
        [TestCase("int a")]
        [TestCase("int a, int b")]
        [TestCase("int a, int b, int c")]
        [TestCase("int a, int b, int c, int d")]
        [TestCase("int a, int b, int c, int d, int e")]
        public void Less_than_max_parameters_are_allowed_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void More_than_max_parameters_are_reported_(string parameters) => An_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void More_than_max_parameters_are_not_reported_for_extern_method_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private static extern bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void More_than_max_parameters_are_reported_for_ctor_(string parameters) => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(" + parameters + @") { }
}");

        protected override string GetDiagnosticId() => MiKo_0004_MethodParameterCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0004_MethodParameterCountAnalyzer();
    }
}