using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is TypeParameterConstraintClauseSyntax node)
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

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }
    }
}