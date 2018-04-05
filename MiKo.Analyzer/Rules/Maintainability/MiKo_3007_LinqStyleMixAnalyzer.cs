using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3007_LinqStyleMixAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3007";

        public MiKo_3007_LinqStyleMixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (QueryExpressionSyntax)context.Node;

            var diagnostic = AnalyzeQueryExpression(node, context.SemanticModel);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeQueryExpression(QueryExpressionSyntax query, SemanticModel semanticModel)
        {
            var methodSyntax = GetEnclosingMethodSyntax(query);

            var hasLinqMethods = methodSyntax
                                         .DescendantNodes(_ => true)
                                         .OfType<MemberAccessExpressionSyntax>()
                                         .SelectMany(_ => semanticModel.LookupSymbols(_.GetLocation().SourceSpan.Start))
                                         .Select(_ => _.ContainingNamespace)
                                         .Any(_ => _.ToString().StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase));

            return hasLinqMethods
                       ? ReportIssue(GetEnclosingMethod(query, semanticModel))
                       : null;
        }
    }
}