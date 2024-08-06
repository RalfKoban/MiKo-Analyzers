using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MethodDeclarationSyntax method && method.Body is BlockSyntax body)
            {
                var returnStatements = body.DescendantNodes<ReturnStatementSyntax>().Where(_ => _.ReturnsCompletedTask());

                return method.Without(returnStatements)
                             .WithReturnType(PredefinedType(SyntaxKind.VoidKeyword));
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is MethodDeclarationSyntax method && method.ExpressionBody != null)
            {
                var siblings = method.Siblings();

                var nodesToUpdate = new List<SyntaxNode>();

                if (siblings.Count > 1 && method.Equals(siblings[0]))
                {
                    nodesToUpdate.Add(siblings[1]);
                }

                nodesToUpdate.Add(method);

                var updatedRoot = root.ReplaceNodes(nodesToUpdate, (original, rewritten) => original == method ? null : rewritten.WithoutLeadingEndOfLine());

                return updatedRoot;
            }

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }
    }
}
