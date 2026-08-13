using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_less_than_maximum_parameters_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void No_issue_is_reported_for_extern_method_having_more_than_maximum_parameters_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private static extern bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void No_issue_is_reported_for_overridden_method_having_more_than_maximum_parameters_(string parameters) => No_issue_is_reported_for(@"
#pragma warning disable MiKo_0004

public class Base
{
    public virtual bool DoSomething(" + parameters + @") => true;
}

#pragma warning restore MiKo_0004

public class TestMe : Base
{
    public override bool DoSomething(" + parameters + @") => false;
}");

        [TestCase("int a, int b, int c, int d, int e, out int f")]
        [TestCase("int a, int b, int c, int d, int e, out int f, out int g")]
        [TestCase("int a, int b, int c, int d, int e, out int f, out int g, out int h")]
        public void No_issue_is_reported_for_more_than_maximum_parameters_caused_by_out_parameters_(string parameters) => No_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        [TestCase("int a, int b, int c, int d, int e, int f, out int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, out int g, out int h")]
        public void An_issue_is_reported_for_more_than_maximum_parameters_(string parameters) => An_issue_is_reported_for(@"
public class TestMe
{
    private bool DoSomething(" + parameters + @") => true;
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void An_issue_is_reported_for_for_ctor_having_more_than_maximum_parameters_(string parameters) => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(" + parameters + @") { }
}");

        [TestCase("int a, int b, int c, int d, int e, int f")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g")]
        [TestCase("int a, int b, int c, int d, int e, int f, int g, int h")]
        public void An_issue_is_reported_for_virtual_method_having_more_than_maximum_parameters_(string parameters) => An_issue_is_reported_for(@"
public class TestMe : Base
{
    public virtual bool DoSomething(" + parameters + @") => false;
}");

        protected override string GetDiagnosticId() => MiKo_0004_MethodParameterCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0004_MethodParameterCountAnalyzer();
    }
}