﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1300";

        public MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        private void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (SimpleLambdaExpressionSyntax)context.Node;
            var diagnostic = AnalyzeSimpleLambdaExpression(node);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            if (node.Parameter is null)
            {
                return null;
            }

            var identifier = node.Parameter.Identifier;
            var parameterName = node.Parameter.GetName();
            switch (parameterName)
            {
                case null: // we don't have one
                case Constants.LambdaIdentifiers.Default: // correct identifier (default one)
                case Constants.LambdaIdentifiers.Fallback: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback2: // correct identifier (2nd fallback as there is already another identifier in the parent lambda expression)
                case Constants.LambdaIdentifiers.Fallback3: // correct identifier (3rd fallback as there is already another identifier in the parent lambda expression)
                    return null;

                default:
                    return Issue(parameterName, identifier, Constants.LambdaIdentifiers.Default);
            }
        }
    }
}