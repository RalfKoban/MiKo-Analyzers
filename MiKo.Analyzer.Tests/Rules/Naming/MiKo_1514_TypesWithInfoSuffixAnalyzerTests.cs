using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1514_TypesWithInfoSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TypeKinds = ["interface", "class", "struct", "record", "enum"];

        [Test]
        public void No_issue_is_reported_for_type_without_suffix_Info_([ValueSource(nameof(TypeKinds))] string type) => No_issue_is_reported_for(@"
namespace Bla
{
    public " + type + @" TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_suffix_Info_([ValueSource(nameof(TypeKinds))] string type) => An_issue_is_reported_for(@"
namespace Bla
{
    public " + type + @" TestMeInfo
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1514_TypesWithInfoSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1514_TypesWithInfoSuffixAnalyzer();
    }
}