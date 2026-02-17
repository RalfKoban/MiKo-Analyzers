using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3111_CodeFixProvider)), Shared]
    public sealed class MiKo_3111_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3111";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var node = (InvocationExpressionSyntax)syntax;

            if (node.Expression is MemberAccessExpressionSyntax m)
            {
                if (m.Expression.ToString() is "Has.Count")
                {
                    return MemberIs("Empty");
                }

                return Member(m.Expression, "Zero");
            }

            return node;
        }
    }
}