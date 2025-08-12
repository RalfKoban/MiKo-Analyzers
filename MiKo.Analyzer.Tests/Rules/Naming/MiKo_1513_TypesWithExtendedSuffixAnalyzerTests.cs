using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1513_TypesWithExtendedSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TypeKinds = ["interface", "class", "struct", "record", "enum"];

        private static readonly string[] WrongSuffixes = ["Advanced", "Complex", "Enhanced", "Extended", "Simple", "Simplified"];

        [Test]
        public void No_issue_is_reported_for_type_without_suffix_([ValueSource(nameof(WrongSuffixes))] string suffix, [ValueSource(nameof(TypeKinds))] string type) => No_issue_is_reported_for(@"
namespace Bla
{
    public " + type + @" TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_suffix_([ValueSource(nameof(WrongSuffixes))] string suffix, [ValueSource(nameof(TypeKinds))] string type) => An_issue_is_reported_for(@"
namespace Bla
{
    public " + type + @" TestMe" + suffix + @"
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_type_with_suffix_([ValueSource(nameof(WrongSuffixes))] string suffix, [ValueSource(nameof(TypeKinds))] string type)
        {
            var originalCode = @"
namespace Bla
{
    public " + type + @" TestMe" + suffix + @"
    {
    }
}
";

            var fixedCode = @"
namespace Bla
{
    public " + type + " " + suffix + @"TestMe
    {
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1513_TypesWithExtendedSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1513_TypesWithExtendedSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1513_CodeFixProvider();
    }
}