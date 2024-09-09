using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3221_CodeFixProvider)), Shared]
    public sealed class MiKo_3221_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3221";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MethodDeclarationSyntax method)
            {
                var nodes = new List<SyntaxNode>();

                foreach (var expression in method.DescendantNodes<MemberAccessExpressionSyntax>())
                {
                    if (expression.Name.GetName() == nameof(GetHashCode))
                    {
                        switch (expression.Expression)
                        {
                            case IdentifierNameSyntax identifier:
                                nodes.Add(identifier);

                                break;

                            case BaseExpressionSyntax _:
                                nodes.Add(expression.Parent);

                                break;
                        }
                    }
                }

                var count = nodes.Count;

                if (count > 8)
                {
                    return GetUpdatedSyntaxWithHashCodeCreation(nodes, method);
                }

                if (count > 0)
                {
                    return GetUpdatedSyntaxWithHashCodeCombine(nodes, method);
                }
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxWithHashCodeCreation(IEnumerable<SyntaxNode> nodes, MethodDeclarationSyntax method)
        {
            var statements = nodes.Select(_ => SyntaxFactory.ParseStatement("hash.Add(" + _ + ");")).ToList();
            statements.Insert(0, SyntaxFactory.ParseStatement("var hash = default(HashCode);"));
            statements.Add(SyntaxFactory.ParseStatement("return hash.ToHashCode();"));

            return method.WithBody(SyntaxFactory.Block(statements.Select(_ => _.WithIndentation().WithTrailingNewLine())))
                         .WithExpressionBody(null)
                         .WithSemicolonToken(default);
        }

        private static SyntaxNode GetUpdatedSyntaxWithHashCodeCombine(IEnumerable<SyntaxNode> nodes, MethodDeclarationSyntax method)
        {
            var arguments = nodes.Select(_ => Argument(_.ToString())).ToArray();
            var invocation = Invocation("HashCode", "Combine", arguments);

            return method.WithBody(null)
                         .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(invocation))
                         .WithSemicolonToken(SyntaxKind.SemicolonToken.AsToken());
        }
    }
}