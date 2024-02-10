using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class DoNotUseSuppressNullableWarningAnalyzerCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsKind(SyntaxKind.SuppressNullableWarningExpression));

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax is PostfixUnaryExpressionSyntax suppression
                                                                                                                         ? suppression.Operand
                                                                                                                         : syntax;
    }
}