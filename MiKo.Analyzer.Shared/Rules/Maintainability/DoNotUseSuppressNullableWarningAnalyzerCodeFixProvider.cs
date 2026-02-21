using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class DoNotUseSuppressNullableWarningAnalyzerCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsKind(SyntaxKind.SuppressNullableWarningExpression));

        protected sealed override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => syntax is PostfixUnaryExpressionSyntax suppression
                                                                         ? suppression.Operand
                                                                         : syntax;
    }
}