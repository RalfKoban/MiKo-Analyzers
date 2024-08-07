using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5013_CodeFixProvider)), Shared]
    public sealed class MiKo_5013_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_5013";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case ArrayCreationExpressionSyntax _:
                    case InitializerExpressionSyntax _:
                        return node;

                    default:
                        return null;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case ArrayCreationExpressionSyntax node:
                    return Invocation(nameof(Array), nameof(Array.Empty), node.Type.ElementType).WithTriviaFrom(node);

                case InitializerExpressionSyntax node when node.Parent is EqualsValueClauseSyntax c && c.Parent is VariableDeclaratorSyntax v && v.Parent is VariableDeclarationSyntax d && d.Type is ArrayTypeSyntax a:
                    return Invocation(nameof(Array), nameof(Array.Empty), a.ElementType).WithTriviaFrom(node);

                default:
                    return syntax;
            }
        }
    }
}