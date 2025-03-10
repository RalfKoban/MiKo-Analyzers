﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0005_LocalFunctionLinesOfCodeAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0005";

        public MiKo_0005_LocalFunctionLinesOfCodeAnalyzer() : base(Id, SyntaxKind.LocalFunctionStatement)
        {
        }

        public int MaxLinesOfCode { get; set; } = 20;

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LocalFunctionStatementSyntax localFunction)
            {
                var loc = Counter.CountLinesOfCode(localFunction.Body);

                if (loc > MaxLinesOfCode)
                {
                    ReportDiagnostics(context, Issue(localFunction.GetName(), localFunction.Identifier, loc, MaxLinesOfCode));
                }
            }
        }
    }
}
