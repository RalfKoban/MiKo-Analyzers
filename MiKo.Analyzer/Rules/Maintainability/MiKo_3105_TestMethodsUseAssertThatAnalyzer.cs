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

        private const string AssertionNamespace = "NUnit.Framework";

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

        public MiKo_3105_TestMethodsUseAssertThatAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax i)
            {
                var calledMethod = node.Name;

                if (AllowedAssertionMethods.Contains(calledMethod.Identifier.ValueText))
                    return;

                if (!AssertionTypes.Contains(i.Identifier.ValueText))
                    return;

                if (context.SemanticModel.GetTypeInfo(i).Type?.ContainingNamespace.FullyQualifiedName() != AssertionNamespace)
                    return; // ignore other test frameworks

                var method = context.GetEnclosingMethod();
                context.ReportDiagnostic(ReportIssue(method.Name, calledMethod.GetLocation()));
            }
        }
    }
}