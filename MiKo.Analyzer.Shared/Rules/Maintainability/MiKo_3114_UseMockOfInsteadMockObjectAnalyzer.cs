using System.Collections.Generic;
using System.Linq;

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

        internal static bool HasIssue(MemberAccessExpressionSyntax node, out TypeSyntax[] types)
        {
            types = null;

            if (node.GetName() == "Object" && node.Expression is ObjectCreationExpressionSyntax o && o.Type.GetNameOnlyPartWithoutGeneric() == "Mock" && o.Type is GenericNameSyntax genericName)
            {
                types = genericName.TypeArgumentList.Arguments.ToArray();
            }

            return types != null;
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName("Moq.Mock") != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;
            var issues = AnalyzeSimpleMemberAccessExpression(node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (HasIssue(node, out _))
            {
                yield return Issue(node);
            }
        }
    }
}