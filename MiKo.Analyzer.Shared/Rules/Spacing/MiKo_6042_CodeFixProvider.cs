using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6042_CodeFixProvider)), Shared]
    public sealed class MiKo_6042_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6042_CreationsAreOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_6042_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ObjectCreationExpressionSyntax creation)
            {
                return creation.WithNewKeyword(creation.NewKeyword.WithoutTrivia())
                               .WithType(creation.Type.WithLeadingSpace());
            }

            return syntax;
        }
    }
}