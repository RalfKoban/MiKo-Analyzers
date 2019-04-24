
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2301";

        public MiKo_2301_TestArrangeActAssertCommentAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSingleLineCommentTrivia, SyntaxKind.SingleLineCommentTrivia);

        private void AnalyzeSingleLineCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is IMethodSymbol symbol && symbol.IsTestMethod())
            {
                // var node = (CommentTriviaSyntaxNode)context.Node;
            }
        }
    }
}