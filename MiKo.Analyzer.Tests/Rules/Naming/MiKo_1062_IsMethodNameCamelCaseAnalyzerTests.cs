using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public class MiKo_1062_IsMethodNameCamelCaseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_matching_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [TestCase("IsSomething")]
        [TestCase("HasSomething")]
        [TestCase("ContainsSomething")]
        [TestCase("IsSomethingThatFits")]
        [TestCase("HasSomethingThatFits")]
        [TestCase("ContainsSomethingStillFitting")]
        public void No_issue_is_reported_for_correctly_named_method(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("IsSomethingThatNotFits")]
        [TestCase("HasSomethingThatNotFits")]
        [TestCase("ContainsSomethingNotFittingAnymore")]
        public void An_issue_is_reported_for_correctly_named_method(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_matching_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething { get; set; }
}
");

        [TestCase("IsSomething")]
        [TestCase("HasSomething")]
        [TestCase("ContainsSomething")]
        [TestCase("IsSomethingFitting")]
        [TestCase("HasSomethingFitting")]
        [TestCase("ContainsSomethingFitting")]
        public void No_issue_is_reported_for_correctly_named_property(string propertyName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + propertyName + @" { get; set; }
}
");

        [TestCase("IsSomethingNotFitting")]
        [TestCase("HasSomethingNotFitting")]
        [TestCase("ContainsSomethingNotFitting")]
        public void An_issue_is_reported_for_correctly_named_property(string propertyName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + propertyName + @" { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1062_IsMethodNameCamelCaseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1062_IsMethodNameCamelCaseAnalyzer();
    }
}