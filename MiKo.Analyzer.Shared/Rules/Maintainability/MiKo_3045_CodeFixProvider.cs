using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3045_CodeFixProvider)), Shared]
    public sealed class MiKo_3045_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3045";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LiteralExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static InvocationExpressionSyntax GetUpdatedSyntax(SyntaxNode syntax) => NameOf((LiteralExpressionSyntax)syntax);
    }
}