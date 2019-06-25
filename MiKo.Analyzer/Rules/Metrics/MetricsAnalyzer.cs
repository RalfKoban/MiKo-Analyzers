﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    public abstract class MetricsAnalyzer : Analyzer
    {
        protected MetricsAnalyzer(string diagnosticId) : base(nameof(Metrics), diagnosticId)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterCodeBlockAction(AnalyzeCodeBlock);

        protected abstract Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol);

        protected bool TryCreateDiagnostic(ISymbol symbol, int metric, int limit, out Diagnostic result)
        {
            result = metric > limit ? Issue(symbol, metric, limit) : null;
            return result != null;
        }

        private void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var body = GetBody(context);
            if (body is null) return;

            var diagnostic = AnalyzeBody(body, context.OwningSymbol);
            if (diagnostic is null) return;

            context.ReportDiagnostic(diagnostic);
        }

        private static BlockSyntax GetBody(CodeBlockAnalysisContext context)
        {
            switch (context.CodeBlock)
            {
                case MethodDeclarationSyntax s: return s.Body;
                case ConstructorDeclarationSyntax s: return s.Body;
                case AccessorDeclarationSyntax s: return s.Body;
                default: return null;
            }
        }
    }
}