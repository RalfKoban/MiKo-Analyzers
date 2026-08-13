using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3238_EmptyArgumentListsOnInitializersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3238";

        public MiKo_3238_EmptyArgumentListsOnInitializersAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ObjectInitializerExpression);

        private void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InitializerExpressionSyntax i && i.Parent is ObjectCreationExpressionSyntax o && o.ArgumentList is ArgumentListSyntax list && list.Arguments.Count is 0)
            {
                ReportDiagnostics(context, Issue(list));
            }
        }
    }
}