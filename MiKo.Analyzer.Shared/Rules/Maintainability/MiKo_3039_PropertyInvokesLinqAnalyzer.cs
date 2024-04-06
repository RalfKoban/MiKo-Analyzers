using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3039_PropertyInvokesLinqAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3039";

        public MiKo_3039_PropertyInvokesLinqAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);

        private static SimpleNameSyntax FindNameSyntax(InvocationExpressionSyntax linq)
        {
            switch (linq.Expression)
            {
                case MemberAccessExpressionSyntax a: return a.Name;
                case MemberBindingExpressionSyntax b: return b.Name;
                default: return null;
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PropertyDeclarationSyntax property)
            {
                var issues = AnalyzePropertyDeclaration(property, context.SemanticModel);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzePropertyDeclaration(PropertyDeclarationSyntax property, SemanticModel semanticModel)
        {
            var propertyName = property.GetName();

            return Enumerable.Empty<Diagnostic>()
                             .Concat(AnalyzeProperty(property.ExpressionBody, propertyName, semanticModel))
                             .Concat(AnalyzeProperty(property.GetGetter(), propertyName, semanticModel))
                             .Concat(AnalyzeProperty(property.GetSetter(), propertyName, semanticModel));
        }

        private IEnumerable<Diagnostic> AnalyzeProperty(SyntaxNode node, string propertyName, SemanticModel semanticModel)
        {
            if (node is null)
            {
                yield break;
            }

            foreach (var linq in node.LinqExtensionMethods(semanticModel))
            {
                var name = FindNameSyntax(linq);

                if (name is null)
                {
                    continue;
                }

                var linqCall = name.GetName();

                if (linqCall == nameof(Enumerable.Empty))
                {
                    // Do not report 'Empty' as violation as the field behind never changes
                    continue;
                }

                yield return Issue(propertyName, name.Identifier, linqCall);
            }

            foreach (var yieldKeyword in node.DescendantTokens(SyntaxKind.YieldKeyword))
            {
                yield return Issue(propertyName, yieldKeyword, yieldKeyword.ValueText);
            }
        }
    }
}