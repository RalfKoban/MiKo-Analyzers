using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6045_CodeFixProvider)), Shared]
    public sealed class MiKo_6045_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6045_ComparisonsAreOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_6045_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                return binary.WithLeft(binary.Left.WithoutTrivia())
                             .WithOperatorToken(binary.OperatorToken.WithLeadingSpace().WithTrailingSpace())
                             .WithRight(binary.Right.WithoutTrivia());
            }

            return syntax;
        }
    }
}