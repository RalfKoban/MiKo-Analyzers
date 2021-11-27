using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3039_PropertyGetterInvokesLinqAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3039";

        public MiKo_3039_PropertyGetterInvokesLinqAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeGetAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);

        private static SimpleNameSyntax FindNameSyntax(InvocationExpressionSyntax linq)
        {
            switch (linq.Expression)
            {
                case MemberAccessExpressionSyntax a: return a.Name;
                case MemberBindingExpressionSyntax b: return b.Name;
                default: return null;
            }
        }

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
            if (property is null)
            {
                yield break;
            }

            var propertyName = property.GetName();

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

                yield return Issue(propertyName, name, linqCall);
            }

            foreach (var yieldKeyword in node.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.YieldKeyword)))
            {
                yield return Issue(propertyName, yieldKeyword, yieldKeyword.ValueText);
            }
        }
    }
}