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

        private static bool IsRecursiveYield(YieldStatementSyntax node, SemanticModel semanticModel, IMethodSymbol method)
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

                    foreach (var invocation in invocations)
                    {
                        if (invocation.Expression is IdentifierNameSyntax i && i.GetName() == method.Name)
                        {
                            var calledMethod = i.Identifier.GetSymbol(semanticModel);

                            if (method.Equals(calledMethod, SymbolEqualityComparer.IncludeNullability))
                            {
                                return true;
                            }
                        }
                    }

                    break;
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
                var semanticModel = context.SemanticModel;
                var methodSymbol = (IMethodSymbol)method.GetEnclosingSymbol(semanticModel);

                // https://stackoverflow.com/questions/3969963/when-not-to-use-yield-return
                foreach (var yieldStatement in method.DescendantNodes().OfType<YieldStatementSyntax>())
                {
                    if (IsRecursiveYield(yieldStatement, semanticModel, methodSymbol))
                    {
                        var issue = Issue(method.GetName(), yieldStatement.GetLocation());
                        context.ReportDiagnostic(issue);
                    }
                }
            }
        }
    }
}