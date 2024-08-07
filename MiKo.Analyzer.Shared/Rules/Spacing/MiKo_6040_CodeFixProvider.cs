﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6040_CodeFixProvider)), Shared]
    public sealed class MiKo_6040_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6040";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MemberAccessExpressionSyntax m)
            {
                var spaces = GetProposedSpaces(issue);

                return m.WithOperatorToken(m.OperatorToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            // adjust  of invocations
            if (syntax is MemberAccessExpressionSyntax m && m.Parent is InvocationExpressionSyntax invocation)
            {
                var argumentList = invocation.ArgumentList;

                if (argumentList.Arguments.Count > 0)
                {
                    var descendants = SelfAndDescendantsOnSeparateLines(argumentList);
                    descendants.Remove(argumentList); // remove list to see if multiple other arguments are on different lines

                    if (descendants.Count > 0)
                    {
                        var additionalSpaces = GetProposedAdditionalSpaces(issue);
                        var updatedInvocation = invocation.WithAdditionalLeadingSpacesOnDescendants(descendants, additionalSpaces);

                        return root.ReplaceNode(invocation, updatedInvocation);
                    }
                }
            }

            return root;
        }
    }
}