using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1066_ParametersWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_with_no_number_suffix() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_number_suffix([Range(0, 10)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int i" + number + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_unfinished_parameter_in_code() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(int ) { }
}
");

        protected override string GetDiagnosticId() => MiKo_1066_ParametersWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1066_ParametersWithNumberSuffixAnalyzer();
    }
}