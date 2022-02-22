using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3084_CodeFixProvider)), Shared]
    public sealed class MiKo_3084_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private static readonly IDictionary<SyntaxKind, SyntaxKind> Expressions = new ConcurrentDictionary<SyntaxKind, SyntaxKind>(new Dictionary<SyntaxKind, SyntaxKind>
                                                                                      {
                                                                                          { SyntaxKind.EqualsExpression, SyntaxKind.EqualsExpression },
                                                                                          { SyntaxKind.NotEqualsExpression, SyntaxKind.NotEqualsExpression },
                                                                                          { SyntaxKind.LessThanExpression, SyntaxKind.GreaterThanExpression },
                                                                                          { SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanOrEqualExpression },
                                                                                          { SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression },
                                                                                          { SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanOrEqualExpression },
                                                                                      });

        public override string FixableDiagnosticId => MiKo_3084_YodaExpressionAnalyzer.Id;

        protected override string Title => Resources.MiKo_3084_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First(_ => Expressions.ContainsKey(_.Kind()));

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var binary = (BinaryExpressionSyntax)syntax;

            var operation = Expressions[binary.Kind()];
            var left = binary.Right.WithoutTrailingTrivia(); // avoid unnecessary spaces at the end
            var right = binary.Left.WithoutTrailingTrivia(); // avoid unnecessary spaces at the end

            return SyntaxFactory.BinaryExpression(operation, left, right);
        }
    }
}