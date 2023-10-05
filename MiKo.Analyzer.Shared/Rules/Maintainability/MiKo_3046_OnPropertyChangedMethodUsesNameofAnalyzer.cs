using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3046";

        private static readonly HashSet<string> ApplicableMethodNames = new HashSet<string>
                                                                            {
                                                                                "NotifyPropertyChanged",
                                                                                "NotifyPropertyChanging",
                                                                                "OnNotifyPropertyChanged",
                                                                                "OnNotifyPropertyChanging",
                                                                                "OnPropertyChanged",
                                                                                "OnPropertyChanging",
                                                                                "OnRaisePropertyChanged",
                                                                                "OnRaisePropertyChanging",
                                                                                "OnTriggerPropertyChanged",
                                                                                "OnTriggerPropertyChanging",
                                                                                "RaisePropertyChanged",
                                                                                "RaisePropertyChanging",
                                                                                "TriggerPropertyChanged",
                                                                                "TriggerPropertyChanging",
                                                                            };

        public MiKo_3046_OnPropertyChangedMethodUsesNameofAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.StringLiteralExpression);

        private static bool HasIssue(SyntaxNode node) => node.Parent is ArgumentSyntax a && a.Parent is ArgumentListSyntax al && al.Parent is InvocationExpressionSyntax i && ApplicableMethodNames.Contains(i.GetName());

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}