using System.Collections;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5012_RecursiveCallUsesYieldAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5012";

        public MiKo_5012_RecursiveCallUsesYieldAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationSyntax, SyntaxKind.MethodDeclaration);

        private static bool IsRecursiveYield(YieldStatementSyntax node, string methodName)
        {
            foreach (var ancestor in node.Ancestors())
            {
                if (ancestor is MethodDeclarationSyntax)
                {
                    break;
                }

                if (ancestor is ForEachStatementSyntax foreachLoop)
                {
                    var invocations = foreachLoop.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    var isRecursiveYield = invocations.Any(_ => _.Expression is IdentifierNameSyntax i && i.GetName() == methodName);
                    return isRecursiveYield;
                }
            }

            return false;
        }

        private static bool ReturnsEnumerable(MethodDeclarationSyntax method)
        {
            foreach (var unknown in method.ChildNodes())
            {
                switch (unknown)
                {
                    case GenericNameSyntax g when g.GetName() == nameof(IEnumerable):
                    case IdentifierNameSyntax i when i.GetName() == nameof(IEnumerable):
                        return true;
                }
            }

            return false;
        }

        private void AnalyzeMethodDeclarationSyntax(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (ReturnsEnumerable(method))
            {
                var methodName = method.GetName();

                // https://stackoverflow.com/questions/3969963/when-not-to-use-yield-return
                foreach (var yieldStatement in method.DescendantNodes().OfType<YieldStatementSyntax>())
                {
                    if (IsRecursiveYield(yieldStatement, methodName))
                    {
                        var issue = Issue(methodName, yieldStatement.GetLocation());
                        context.ReportDiagnostic(issue);
                    }
                }
            }
        }
    }
}