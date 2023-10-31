using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests : CodeFixVerifier
    {
        protected override string GetDiagnosticId() => MiKo_3011_ArgumentExceptionsParamNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3011_ArgumentExceptionsParamNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3011_CodeFixProvider();
    }
}