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

        private static readonly HashSet<string> AssertionTypes = new HashSet<string>
                                                                     {
                                                                         "Assert",
                                                                         "StringAssert",
                                                                         "CollectionAssert",
                                                                         "FileAssert",
                                                                         "DirectoryAssert",
                                                                     };

        private static readonly HashSet<string> AllowedAssertionMethods = new HashSet<string>
                                                                              {
                                                                                  "That",
                                                                                  "Fail",
                                                                                  "CatchAsync",
                                                                                  "Catch",
                                                                                  "ThrowsAsync",
                                                                                  "Throws",
                                                                              };

        private const string AssertionNamespace = "NUnit.Framework";


        public MiKo_3105_TestMethodsUseAssertThatAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var method = context.GetEnclosingMethod();
            if (method == null)
                return;

            if (method.IsTestMethod() || method.ContainingType.IsTestClass())
            {
                var node = (MemberAccessExpressionSyntax)context.Node;

                if (node.Expression is IdentifierNameSyntax i
                 && AssertionTypes.Contains(i.Identifier.ValueText)
                 && AllowedAssertionMethods.Contains(node.Name.Identifier.ValueText) == false
                 && context.SemanticModel.GetTypeInfo(i).Type?.ContainingNamespace.FullyQualifiedName() == AssertionNamespace)
                {
                    context.ReportDiagnostic(ReportIssue(method.Name, node.Name.GetLocation()));
                }
            }
        }
    }
}