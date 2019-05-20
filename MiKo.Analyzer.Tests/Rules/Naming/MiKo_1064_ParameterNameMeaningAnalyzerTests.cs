
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
        public void An_issue_is_reported_for_wrong_name() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(TestMe testMe)
    { }
}
");

        protected override string GetDiagnosticId() => MiKo_1064_ParameterNameMeaningAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1064_ParameterNameMeaningAnalyzer();
    }
}