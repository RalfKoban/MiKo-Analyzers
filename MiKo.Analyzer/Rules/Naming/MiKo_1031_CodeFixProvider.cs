using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1031_CodeFixProvider)), Shared]
    public sealed class MiKo_1031_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1031_TypeModelSuffixAnalyzer.Id;

        protected override string Title => "Remove 'Model' indicator";

        protected override string GetNewName(ISymbol symbol) => MiKo_1031_TypeModelSuffixAnalyzer.FindBetterName((INamedTypeSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
    }
}