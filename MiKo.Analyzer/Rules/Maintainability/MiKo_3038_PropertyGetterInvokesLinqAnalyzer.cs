using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3038_PropertyGetterInvokesLinqAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3038";

        public MiKo_3038_PropertyGetterInvokesLinqAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeGetAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);

        private void AnalyzeGetAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (AccessorDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            foreach (var issue in Analyze(node, semanticModel))
            {
                context.ReportDiagnostic(issue);
            }
        }

        private IEnumerable<Diagnostic> Analyze(AccessorDeclarationSyntax node, SemanticModel semanticModel)
        {
            var property = node.GetEnclosing<PropertyDeclarationSyntax>();
            if (property != null)
            {
                var propertyName = property.Identifier.ValueText;

                foreach (var linq in node.LinqExtensionMethods(semanticModel))
                {
                    var x = (MemberAccessExpressionSyntax)linq.Expression;
                    var linqCall = x.Name.Identifier.ValueText;

                    yield return Issue(propertyName, linq.Expression.GetLocation(), linqCall);
                }

                foreach (var yieldKeyword in node.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.YieldKeyword)))
                {
                    yield return Issue(propertyName, yieldKeyword.GetLocation(), yieldKeyword.ValueText);
                }
            }
        }
    }
}