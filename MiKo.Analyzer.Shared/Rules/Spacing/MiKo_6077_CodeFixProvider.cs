using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6077_CodeFixProvider)), Shared]
    public sealed class MiKo_6077_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6077";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterListSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is ParameterListSyntax list)
            {
                return list.PlacedOnSameLine()
                           .WithTrailingTriviaFrom(list);
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is ParameterListSyntax list)
            {
                switch (list.Parent)
                {
                    case MethodDeclarationSyntax m:
                    {
                        var updated = m.TypeParameterList is TypeParameterListSyntax tpl
                                      ? m.WithTypeParameterList(tpl.WithoutTrailingTrivia())
                                      : m.WithIdentifier(m.Identifier.WithoutTrailingTrivia());

                        return root.ReplaceNode(m, updated);
                    }

                    case ConstructorDeclarationSyntax ctor:
                    {
                        var updated = ctor.WithIdentifier(ctor.Identifier.WithoutTrailingTrivia());

                        return root.ReplaceNode(ctor, updated);
                    }

                    case DestructorDeclarationSyntax dtor:
                    {
                        var updated = dtor.WithIdentifier(dtor.Identifier.WithoutTrailingTrivia());

                        return root.ReplaceNode(dtor, updated);
                    }

                    case OperatorDeclarationSyntax o:
                    {
                        var updated = o.WithOperatorToken(o.OperatorToken.WithoutTrailingTrivia());

                        return root.ReplaceNode(o, updated);
                    }

                    case ConversionOperatorDeclarationSyntax conversion:
                    {
                        var updated = conversion.WithType(conversion.Type.WithoutTrailingTrivia());

                        return root.ReplaceNode(conversion, updated);
                    }
                }
            }

            return root;
        }
    }
}