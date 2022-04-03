using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5013_CodeFixProvider)), Shared]
    public sealed class MiKo_5013_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_5013_EmptyArrayAnalyzer.Id;

        protected override string Title => Resources.MiKo_5013_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArrayCreationExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var node = (ArrayCreationExpressionSyntax)syntax;

            var type = SyntaxFactory.IdentifierName(nameof(Array));
            var method = SyntaxFactory.GenericName(nameof(Array.Empty)).AddTypeArgumentListArguments(node.Type.ElementType);

            var expression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);

            return SyntaxFactory.InvocationExpression(expression);
        }
    }
}