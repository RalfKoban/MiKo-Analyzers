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
            switch (syntax)
            {
                case LiteralExpressionSyntax literal:
                    return NameOf(literal);

                case TypeOfExpressionSyntax _ when issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.ParameterValue, out var value):
                    return TypeOf(value);

                default:
                    return null;
            }
        }
    }
}