﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6051";

        public MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.BaseConstructorInitializer, SyntaxKind.ThisConstructorInitializer);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ConstructorInitializerSyntax)context.Node;

            var startLine = node.ColonToken.GetStartingLine();
            var rightPosition = node.ThisOrBaseKeyword.GetStartPosition();

            if (startLine != rightPosition.Line)
            {
                ReportDiagnostics(context, Issue(node.ColonToken));
            }
        }
    }
}