using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3040_BooleanMethodParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameterless_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_parameter_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x) { }
}
");

        [TestCase("bool b")]
        [TestCase("bool b, int x")]
        [TestCase("int x, bool b, int y")]
        [TestCase("int x, bool b")]
        [TestCase("bool b1, bool b2, bool b3")]
        public void An_issue_is_reported_for_boolean_parameter_on_method(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        protected override string GetDiagnosticId() => MiKo_3040_BooleanMethodParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3040_BooleanMethodParametersAnalyzer();
    }
}