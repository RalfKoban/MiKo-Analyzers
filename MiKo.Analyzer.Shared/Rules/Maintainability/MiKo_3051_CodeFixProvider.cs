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
        public override string FixableDiagnosticId => "MiKo_3051";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case LiteralExpressionSyntax _:
                    case TypeOfExpressionSyntax _:
                    case InvocationExpressionSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case LiteralExpressionSyntax literal:
                    return NameOf(literal).WithTriviaFrom(literal);

                case TypeOfExpressionSyntax expression when issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.ParameterValue, out var value):
                    return TypeOf(value).WithTriviaFrom(expression);

                default:
                    return null;
            }
        }
    }
}