using System;
using System.ComponentModel;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3012";

        public MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeObjectCreation(node);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeObjectCreation(ObjectCreationExpressionSyntax node)
        {
            var type = node.Type.ToString();
            switch (type)
            {
                case nameof(ArgumentOutOfRangeException):
                case nameof(InvalidEnumArgumentException):
                case "System." + nameof(ArgumentOutOfRangeException):
                case "System.ComponentModel." + nameof(InvalidEnumArgumentException):
                {
                    if (node.ArgumentList.Arguments.Count != 3)
                        return ReportIssue(type, node.GetLocation());

                    break;
                }
            }

            return null;
        }
    }
}