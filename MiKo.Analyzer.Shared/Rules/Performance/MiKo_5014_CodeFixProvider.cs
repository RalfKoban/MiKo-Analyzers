using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5014_CodeFixProvider)), Shared]
    public sealed class MiKo_5014_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_5014_MethodReturnsEmptyListAnalyzer.Id;

        protected override string Title => Resources.MiKo_5014_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ObjectCreationExpressionSyntax node && node.Type is GenericNameSyntax generic)
            {
                return Invocation(nameof(Array), nameof(Array.Empty), generic.TypeArgumentList.Arguments.ToArray()).WithTriviaFrom(node);
            }

            return syntax;
        }
    }
}
