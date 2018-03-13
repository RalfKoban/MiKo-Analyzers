using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1201_ExceptionParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_parameters() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_non_matching_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_exception([Values("ex", "exception")] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Exception " + name + @")
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_exception([Values("e", "exc", "except")] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Exception " + name + @")
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1201_ExceptionParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1201_ExceptionParameterAnalyzer();
    }
}