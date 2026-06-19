using System;
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

                if (issues.Length > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private Diagnostic[] AnalyzePropertyDeclaration(PropertyDeclarationSyntax property, SemanticModel semanticModel)
        {
            var propertyName = property.GetName();

            List<Diagnostic> issues = null; // per default we do not have some, so avoid lots of useless object creations

            if (property.ExpressionBody is ArrowExpressionClauseSyntax body)
            {
                AnalyzeProperty(body, propertyName, semanticModel, ref issues);
            }

            if (property.GetGetter() is AccessorDeclarationSyntax getter)
            {
                AnalyzeProperty(getter.Body ?? (SyntaxNode)getter.ExpressionBody, propertyName, semanticModel, ref issues);
            }

            if (property.GetSetter() is AccessorDeclarationSyntax setter)
            {
                AnalyzeProperty(setter.Body ?? (SyntaxNode)setter.ExpressionBody, propertyName, semanticModel, ref issues);
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }

        private void AnalyzeProperty(SyntaxNode node, string propertyName, SemanticModel semanticModel, ref List<Diagnostic> issues)
        {
            if (node is null)
            {
                return;
            }

            foreach (var linq in node.LinqExtensionMethods(semanticModel))
            {
                var name = FindNameSyntax(linq);

                if (name is null)
                {
                    continue;
                }

                var linqCall = name.GetName();

                if (linqCall is nameof(Enumerable.Empty))
                {
                    // Do not report 'Empty' as violation as the field behind never changes
                    continue;
                }

                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(propertyName, name.Identifier, linqCall));
            }

            foreach (var yieldKeyword in node.DescendantTokens(SyntaxKind.YieldKeyword))
            {
                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(propertyName, yieldKeyword, yieldKeyword.ValueText));
            }
        }
    }
}