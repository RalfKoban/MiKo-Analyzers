using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3222_CodeFixProvider)), Shared]
    public sealed class MiKo_3222_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3222";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var node = syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

            if (node?.Parent is ParenthesizedExpressionSyntax parenthesized && parenthesized.Expression == node)
            {
                return parenthesized;
            }

            return node;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ExpressionSyntax expression && expression.WithoutParenthesis() is BinaryExpressionSyntax binary)
            {
                if (binary.Left.WithoutParenthesis() is BinaryExpressionSyntax left)
                {
                    var arguments = binary.Right.FirstDescendant<ArgumentListSyntax>()?.Arguments;

                    var argument1 = Argument(left.Left);
                    var argument2 = Argument(left.Right);

                    var access = SimpleMemberAccess(PredefinedType(SyntaxKind.StringKeyword), nameof(string.Equals));

                    return arguments?.Count > 1
                           ? Invocation(access, argument1, argument2, arguments.Value[1])
                           : Invocation(access, argument1, argument2);
                }
            }

            return syntax;
        }
    }
}