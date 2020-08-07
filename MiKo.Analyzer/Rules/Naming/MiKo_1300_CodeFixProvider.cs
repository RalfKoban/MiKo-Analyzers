using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_CodeFixProvider)), Shared]
    public sealed class MiKo_1300_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override string Title => "Name it '" + Constants.LambdaIdentifiers.Default + "'";

        protected override string GetNewName(ISymbol symbol) => Constants.LambdaIdentifiers.Default;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<SimpleLambdaExpressionSyntax>().First().Parameter;
    }
}