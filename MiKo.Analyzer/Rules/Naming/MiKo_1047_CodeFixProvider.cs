using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1047_CodeFixProvider)), Shared]
    public sealed class MiKo_1047_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer.Id;

        protected override string Title => "Remove '" + Constants.AsyncSuffix + "' suffix";

        protected override string GetNewName(ISymbol symbol) => MiKo_1047_NonAsyncMethodsButAsyncSuffixAnalyzer.FindBetterName((IMethodSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}