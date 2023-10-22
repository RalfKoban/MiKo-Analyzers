using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2228_DocumentationIsNotNegativeAnalyzerTests : CodeFixVerifier
    {
        protected override string GetDiagnosticId() => MiKo_2228_DocumentationIsNotNegativeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2228_DocumentationIsNotNegativeAnalyzer();
    }
}