using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1090_ParametersWrongSuffixedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_properly_named_parameter([Values("comparer", "view", "item", "entity")] string name) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(object " + name + @") { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_incorrectly_named_parameter([Values("myComparer", "myView", "myItem", "myEntity")] string name) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(object " + name + @") { }
}
");

        protected override string GetDiagnosticId() => MiKo_1090_ParametersWrongSuffixedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1090_ParametersWrongSuffixedAnalyzer();
    }
}