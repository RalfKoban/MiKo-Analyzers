using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1014_CodeFixProvider)), Shared]
    public sealed class MiKo_1014_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1014_CheckMethodsAnalyzer.Id;

        protected override string Title => Resources.MiKo_1014_CodeFixTitle;

        protected override string GetNewName(ISymbol symbol) => MiKo_1014_CheckMethodsAnalyzer.FindBetterName((IMethodSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}