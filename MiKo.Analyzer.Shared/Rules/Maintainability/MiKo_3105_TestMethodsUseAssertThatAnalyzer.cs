﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3105_TestMethodsUseAssertThatAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3105";

        private static readonly HashSet<string> AllowedAssertionMethods = new HashSet<string>
                                                                              {
                                                                                  "ThatAsync",
                                                                                  "That",
                                                                                  "Fail",
                                                                                  "Pass",
                                                                                  "CatchAsync",
                                                                                  "Catch",
                                                                                  "ThrowsAsync",
                                                                                  "Multiple",
                                                                              };

        public MiKo_3105_TestMethodsUseAssertThatAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            var issues = AnalyzeSimpleMemberAccessExpression(context, node);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] AnalyzeSimpleMemberAccessExpression(in SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax invokedClass)
            {
                if (AllowedAssertionMethods.Contains(node.GetName()))
                {
                    return Array.Empty<Diagnostic>();
                }

                if (Constants.Names.AssertionTypes.Contains(invokedClass.GetName()) is false)
                {
                    return Array.Empty<Diagnostic>();
                }

                var testFrameworkNamespace = invokedClass.GetTypeSymbol(context.SemanticModel)?.ContainingNamespace.FullyQualifiedName();

                if (Constants.Names.AssertionNamespaces.Contains(testFrameworkNamespace) is false)
                {
                    return Array.Empty<Diagnostic>();
                }

                var method = context.GetEnclosingMethod();

                if (method is null)
                {
                    // nameof() is also a SimpleMemberAccessExpression, so assignments of lists etc. may cause an NRE to be thrown
                    return Array.Empty<Diagnostic>();
                }

                return new[] { Issue(method.Name, node) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}