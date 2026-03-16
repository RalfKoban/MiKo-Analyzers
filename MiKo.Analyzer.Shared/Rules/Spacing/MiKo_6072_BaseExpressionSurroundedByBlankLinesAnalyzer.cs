using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6072_BaseExpressionSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6072";

        public MiKo_6072_BaseExpressionSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsCall(ITypeSymbol type) => false;

        protected override bool IsCall(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel) => syntax.Expression is BaseExpressionSyntax;
    }
}