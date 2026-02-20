using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3084_CodeFixProvider)), Shared]
    public sealed class MiKo_3084_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> Expressions = new Dictionary<SyntaxKind, SyntaxKind>
                                                                                     {
                                                                                         { SyntaxKind.EqualsExpression, SyntaxKind.EqualsExpression },
                                                                                         { SyntaxKind.NotEqualsExpression, SyntaxKind.NotEqualsExpression },
                                                                                         { SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression },
                                                                                         { SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanOrEqualExpression },
                                                                                         { SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression },
                                                                                         { SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanOrEqualExpression },
                                                                                     };

        public override string FixableDiagnosticId => "MiKo_3084";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => Expressions.ContainsKey(_.Kind()));

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var operation = Expressions[binary.Kind()];
                var left = binary.Right.WithoutTrailingTrivia(); // avoid unnecessary spaces at the end
                var right = binary.Left.WithoutTrailingTrivia(); // avoid unnecessary spaces at the end

                return SyntaxFactory.BinaryExpression(operation, left, right);
            }

            return syntax;
        }
    }
}