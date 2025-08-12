﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3114_UseMockOfInsteadMockObjectAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3114";

        public MiKo_3114_UseMockOfInsteadMockObjectAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool IsApplicable(Compilation compilation) => compilation.GetTypeByMetadataName(Constants.Moq.MockFullQualified) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            var issues = AnalyzeSimpleMemberAccessExpression(node);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax node) => node.TryGetMoqTypes(out _)
                                                                                                       ? new[] { Issue(node) }
                                                                                                       : Array.Empty<Diagnostic>();
    }
}