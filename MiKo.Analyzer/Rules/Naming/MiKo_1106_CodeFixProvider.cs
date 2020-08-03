using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1106_CodeFixProvider)), Shared]
    public sealed class MiKo_1106_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1106_OneTimeTestTeardownMethodsAnalyzer.Id;

        protected override string Title => "Rename to '" + MiKo_1106_OneTimeTestTeardownMethodsAnalyzer.ExpectedName + "'";

        protected override string GetNewName(ISymbol symbol) => MiKo_1106_OneTimeTestTeardownMethodsAnalyzer.ExpectedName;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}