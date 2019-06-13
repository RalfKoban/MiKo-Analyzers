
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1064_ParameterNameMeaningAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(TestMe culprit)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_named_after_type_and_some_uppercase_letters() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(TestMe testMe)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_overridden_method_with_parameter_named_after_type_and_some_uppercase_letters() => No_issue_is_reported_for(@"
public class TestMe
{
    public override void DoSomething(TestMe testMe)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_type_and_only_lowercase_letters() => No_issue_is_reported_for(@"
public class Side
{
    public void DoSomething(Side side)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_type_and_used_in_ctor() => No_issue_is_reported_for(@"
public enum Side
{
}

public class TestMe
{
    public TestMe(Side side) => Side = side;

    public Side Side { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_named_after_interface_type() => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe
{
    public void DoSomething(ITestMe testMe)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1064_ParameterNameMeaningAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1064_ParameterNameMeaningAnalyzer();
    }
}