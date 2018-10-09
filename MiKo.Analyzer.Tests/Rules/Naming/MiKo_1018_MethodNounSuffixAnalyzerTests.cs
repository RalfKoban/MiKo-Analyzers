using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_name_([ValueSource(nameof(ValidMethodNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_name_([ValueSource(nameof(InvalidMethodNames))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");
        [TestCase("Function", ExpectedResult = "Function", Description = "There is no verb available")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("Comparison", ExpectedResult = "Compare")]
        [TestCase("Creation", ExpectedResult = "Create")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Installation", ExpectedResult = "Install")]
        [TestCase("Uninstallation", ExpectedResult = "Uninstall")]
        [TestCase("Configuration", ExpectedResult = "Configure")]
        [TestCase("Initialization", ExpectedResult = "Initialize")]
        [TestCase("Initialisation", ExpectedResult = "Initialise")]
        [TestCase("Information", ExpectedResult = "Inform")]
        [TestCase("Adoption", ExpectedResult = "Adopt")]
        [TestCase("Adaptation", ExpectedResult = "Adapt")]
        [TestCase("Stabilization", ExpectedResult = "Stabilize")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Estimation", ExpectedResult = "Estimate")]
        public string A_proper_name_is_found(string name)
        {
            MiKo_1018_MethodNounSuffixAnalyzer.TryFindBetterName(name, out var result);
            return result;
        }

        protected override string GetDiagnosticId() => MiKo_1018_MethodNounSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1018_MethodNounSuffixAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> ValidMethodNames() => new[] { "DoSomething", "Compare", "Manipulate", "Adopt", "FindBison", "Install", "Act" };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> InvalidMethodNames() => new[] { "DoComparison", "ApplyComparison", "ExecuteManipulation", "RunAdoption", "Installation", "DoAction", "Initialization", "Configuration", };
    }
}