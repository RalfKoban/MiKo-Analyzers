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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3119_CodeFixProvider)), Shared]
    public sealed class MiKo_3119_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3119";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntaxRoot = null;

            if (syntax is MethodDeclarationSyntax method && method.ExpressionBody != null)
            {
                updatedSyntaxRoot = GetUpdatedSyntaxRoot(root, method);
            }

            return Task.FromResult(updatedSyntaxRoot);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is MethodDeclarationSyntax method && method.Body is BlockSyntax body)
            {
                var returnStatements = body.DescendantNodes<ReturnStatementSyntax>(_ => _.ReturnsCompletedTask());

                return method.Without(returnStatements)
                             .WithReturnType(PredefinedType(SyntaxKind.VoidKeyword));
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, MethodDeclarationSyntax method)
        {
            var siblings = method.Siblings();

            var nodesToUpdate = new List<SyntaxNode>(2);

            if (siblings.Count > 1 && method.Equals(siblings[0]))
            {
                nodesToUpdate.Add(siblings[1]);
            }

            nodesToUpdate.Add(method);

            var updatedRoot = root.ReplaceNodes(nodesToUpdate, (original, rewritten) => original == method ? null : rewritten.WithoutLeadingEndOfLine());

            return updatedRoot;
        }
    }
}
