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
            var methodSyntax = query.GetEnclosing<MethodDeclarationSyntax>();

            var hasLinqMethods = methodSyntax
                                         .DescendantNodes()
                                         .OfType<MemberAccessExpressionSyntax>()
                                         .SelectMany(_ => semanticModel.LookupSymbols(_.GetLocation().SourceSpan.Start))
                                         .Select(_ => _.ContainingNamespace?.ToString())
                                         .Where(_ => _ != null)
                                         .Any(_ => _.StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase));

            return hasLinqMethods
                       ? ReportIssue(methodSyntax.Identifier.ValueText, query.GetLocation())
                       : null;
        }
    }
}