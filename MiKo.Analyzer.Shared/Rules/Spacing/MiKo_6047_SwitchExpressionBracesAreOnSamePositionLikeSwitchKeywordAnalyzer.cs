﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6047";

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.SwitchExpression };

        public MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchExpressionSyntax syntax)
            {
                var position = syntax.SwitchKeyword.GetEndPosition();
                var switchPosition = new LinePosition(position.Line, position.Character + 1);

                var openBraceToken = syntax.OpenBraceToken;
                var openBracePosition = openBraceToken.GetStartPosition();

                if (switchPosition.Line != openBracePosition.Line && switchPosition.Character != openBracePosition.Character)
                {
                    var issue = Issue(openBraceToken, CreateProposalForLinePosition(switchPosition));

                    ReportDiagnostics(context, issue);
                }
            }
        }
    }
}