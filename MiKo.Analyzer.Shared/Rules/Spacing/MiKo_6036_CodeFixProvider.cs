using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6036_CodeFixProvider)), Shared]
    public sealed class MiKo_6036_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6036";

        protected override string Title => Resources.MiKo_6036_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LambdaExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is LambdaExpressionSyntax lambda)
            {
                var spaces = GetProposedSpaces(issue);

                var block = GetUpdatedBlock(lambda.Block, spaces);

                return lambda.WithBlock(block);
            }

            return syntax;
        }
    }
}