using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3060_CodeFixProvider)), Shared]
    public sealed class MiKo_3060_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3060";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ExpressionStatementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            // we want to remove the syntax
            return Task.FromResult<SyntaxNode>(null);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedRoot = GetUpdatedSyntaxRoot(root);

            return Task.FromResult(updatedRoot);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root)
        {
            if (root.DescendantNodes<IdentifierNameSyntax>().None(_ => IsDebugOrTrace(_.GetName())))
            {
                return root.WithoutUsing("System.Diagnostics"); // remove unused "using System.Diagnostics;"
            }

            return root;
        }

        private static bool IsDebugOrTrace(string name)
        {
            switch (name)
            {
                case nameof(Debug):
                case nameof(Trace):
                    return true;

                default:
                    return false;
            }
        }
    }
}