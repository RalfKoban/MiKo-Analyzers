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
                var expressionBody = property.ExpressionBody;

                if (expressionBody != null)
                {
                    ReportDiagnostics(context, Analyze(expressionBody, property, context.SemanticModel));
                }
                else
                {
                    var getter = property.GetGetter();
                    if (getter != null)
                    {
                        ReportDiagnostics(context, Analyze(getter, property, context.SemanticModel));
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNode node, PropertyDeclarationSyntax property, SemanticModel semanticModel)
        {
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

                yield return Issue(propertyName, name.Identifier, linqCall);
            }

            foreach (var yieldKeyword in node.DescendantTokens(SyntaxKind.YieldKeyword))
            {
                yield return Issue(propertyName, yieldKeyword, yieldKeyword.ValueText);
            }
        }
    }
}