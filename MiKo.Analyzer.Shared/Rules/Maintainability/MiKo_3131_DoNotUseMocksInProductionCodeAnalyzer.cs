using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3131_DoNotUseMocksInProductionCodeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3131";

        public MiKo_3131_DoNotUseMocksInProductionCodeAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => false; // this is the special case were we are no unit test analyzer but analyze production code to use mocks

        protected override bool IsApplicable(Compilation compilation)
        {
            var assemblyName = compilation.AssemblyName;

            if (assemblyName is null || assemblyName.Contains("Test"))
            {
                return false;
            }

            return compilation.GetTypeByMetadataName(Constants.Moq.MockFullQualified) != null
                || compilation.GetTypeByMetadataName(Constants.NSubstitute.SubstituteFullQualified) != null
                || compilation.GetTypeByMetadataName(Constants.Rhino.MockRepositoryFullQualified) != null;
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordDeclaration);
        }

        private static bool HasIssue(InvocationExpressionSyntax invocation)
        {
            switch (invocation.GetIdentifierName())
            {
                case Constants.NSubstitute.Substitute:
                    return invocation.GetName() is Constants.NSubstitute.For;

                case Constants.Moq.Mock:
                    return invocation.GetName() is Constants.Moq.Of;

                case Constants.Rhino.MockRepository:
                    switch (invocation.GetName())
                    {
                        case Constants.Rhino.CreateMock:
                        case Constants.Rhino.CreateMockObject:
                        case Constants.Rhino.CreateMockWithRemoting:
                        case Constants.Rhino.CreateMultiMock:
                        case Constants.Rhino.DynamicMock:
                        case Constants.Rhino.DynamicMockWithRemoting:
                        case Constants.Rhino.DynamicMultiMock:
                        case Constants.Rhino.GenerateDynamicMockWithRemoting:
                        case Constants.Rhino.GenerateMock:
                        case Constants.Rhino.GeneratePartialMock:
                        case Constants.Rhino.GenerateStrictMock:
                        case Constants.Rhino.GenerateStrictMockWithRemoting:
                        case Constants.Rhino.GenerateStub:
                        case Constants.Rhino.PartialMock:
                        case Constants.Rhino.PartialMultiMock:
                        case Constants.Rhino.RemotingMock:
                        case Constants.Rhino.StrictMock:
                        case Constants.Rhino.StrictMockWithRemoting:
                        case Constants.Rhino.StrictMultiMock:
                            return true;

                        default:
                            return false;
                    }

                default:
                    return false;
            }
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax creation)
        {
            switch (creation.Type)
            {
                case QualifiedNameSyntax q:
                    return q.Right.GetName() is Constants.Moq.Mock;

                case SimpleNameSyntax s:
                    return s.GetName() is Constants.Moq.Mock;

                default:
                    return false;
            }
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is UsingDirectiveSyntax usingDirective)
            {
                switch (usingDirective.GetName())
                {
                    case Constants.Moq.Namespace:
                    case Constants.NSubstitute.Namespace:
                    case Constants.Rhino.Namespace:
                    {
                        ReportDiagnostics(context, Issue(usingDirective));

                        break;
                    }
                }
            }
        }

        private void AnalyzeDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax declaration)
            {
                var issues = AnalyzeDeclaration(declaration);

                if (issues.Length > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private Diagnostic[] AnalyzeDeclaration(ClassDeclarationSyntax declaration)
        {
            List<Diagnostic> issues = null;

            foreach (var node in declaration.DescendantNodes())
            {
                switch (node)
                {
                    case InvocationExpressionSyntax invocation when HasIssue(invocation):
                    case ObjectCreationExpressionSyntax creation when HasIssue(creation):
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>();
                        }

                        issues.Add(Issue(node));

                        break;
                    }
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}