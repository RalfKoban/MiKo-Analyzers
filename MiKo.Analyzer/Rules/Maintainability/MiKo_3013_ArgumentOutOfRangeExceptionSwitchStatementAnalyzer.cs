using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3013";

        public MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer() : base(Id, (SymbolKind)(-1))
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
                case nameof(ArgumentException):
                case nameof(ArgumentNullException):
                {
                    var switchSection = node.GetEnclosing<SwitchSectionSyntax>();
                    if (switchSection == null) return null;

                    // if there is a 'default' switch label in the specific switch section, then we are in the 'default:' clause
                    var isBelow = switchSection.DescendantNodes().OfType<DefaultSwitchLabelSyntax>().Any();
                    return isBelow
                               ? ReportIssue(type, node.GetLocation(), nameof(ArgumentOutOfRangeException))
                               : null;
                }

                default:
                    return null;
            }
        }
    }
}