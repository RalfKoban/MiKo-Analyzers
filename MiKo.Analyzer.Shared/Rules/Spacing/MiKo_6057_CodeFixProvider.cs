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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6057_CodeFixProvider)), Shared]
    public sealed class MiKo_6057_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6057";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeParameterConstraintClauseSyntax>().First();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(syntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntaxRoot = null;

            if (syntax is TypeParameterConstraintClauseSyntax node)
            {
                updatedSyntaxRoot = GetUpdatedSyntaxRoot(root, node, issue);
            }

            return Task.FromResult(updatedSyntaxRoot);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, TypeParameterConstraintClauseSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            var clauses = node.GetConstraintClauses();

            return root.ReplaceNodes(
                                 clauses,
                                 (original, rewritten) =>
                                                         {
                                                             var updated = rewritten;

                                                             var updatedConstraints = updated.Constraints.Select(_ => _.WithTrailingNewLine()).ToSeparatedSyntaxList();

                                                             updated = updated.WithConstraints(updatedConstraints);

                                                             if (clauses.IndexOf(original) > 0)
                                                             {
                                                                 updated = updated.WithWhereKeyword(original.WhereKeyword.WithLeadingSpaces(spaces));
                                                             }

                                                             return updated;
                                                         });
        }
    }
}