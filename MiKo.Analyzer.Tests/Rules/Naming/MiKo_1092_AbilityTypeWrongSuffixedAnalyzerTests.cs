using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1092_AbilityTypeWrongSuffixedAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames =
            {
                "ComparableItem",
                "ComparableEntity",
                "ComparableElement",
                "ComparableElementInfo",
                "ComparableInfo",
                "ComparableInformation",
            };

        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_wrong_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"

public class " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1092_AbilityTypeWrongSuffixedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1092_AbilityTypeWrongSuffixedAnalyzer();
    }
}