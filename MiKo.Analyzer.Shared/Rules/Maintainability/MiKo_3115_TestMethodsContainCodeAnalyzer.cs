﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3115_TestMethodsContainCodeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3115";

        public MiKo_3115_TestMethodsContainCodeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var methodBody = method.Body;

            if (methodBody != null && methodBody.Statements.Count == 0 && method.IsTestMethod() && method.IsAbstract() is false)
            {
                ReportDiagnostics(context, Issue(method));
            }
        }
    }
}