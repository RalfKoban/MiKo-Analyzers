using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3051_CodeFixProvider)), Shared]
    public sealed class MiKo_3051_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3051_DependencyPropertyRegisterAnalyzer.Id;

        protected override string Title => Resources.MiKo_3051_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                if (node is LiteralExpressionSyntax)
                {
                    return node;
                }

                if (node is TypeOfExpressionSyntax)
                {
                    return node;
                }

                if (node is InvocationExpressionSyntax)
                {
                    return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            if (syntax is LiteralExpressionSyntax literal)
            {
                return NameOf(literal);
            }

            if (syntax is TypeOfExpressionSyntax)
            {
                if (diagnostic.Properties.TryGetValue(MiKo_3051_DependencyPropertyRegisterAnalyzer.Value, out var value))
                {
                    return TypeOf(value);
                }
            }

            return null;
        }
    }
}