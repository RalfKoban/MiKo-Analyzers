using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5010_EqualsAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5010";

        public MiKo_5010_EqualsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool IsObjectEqualsStaticMethod(IMethodSymbol method) => method.IsStatic && method.ContainingType.SpecialType == SpecialType.System_Object;

        private static bool IsObjectEqualsOnStructMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            // find out whether it is the non-static 'Equals(object)' method
            if (parameters.Length == 1 && parameters[0].Type.IsObject())
            {
                var type = method.ContainingType;

                return type.IsValueType;
            }

            return false;
        }

        private static bool IsEnumEqualsMethod(IMethodSymbol method) => method.ContainingType.SpecialType == SpecialType.System_Enum;

        private static bool IsStruct(SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return arguments.Any(_ => _.Expression.IsStruct(semanticModel));
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            // shortcut to not analyze each single invocation node
            if (node.Expression.GetName() == nameof(object.Equals))
            {
                var issue = AnalyzeEqualsInvocation(node, context.SemanticModel);

                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeEqualsInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;

            switch (arguments.Count)
            {
                case 2:
                    return AnalyzeMethod(node, semanticModel, arguments);

                case 1 when arguments[0].Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression):
                    return AnalyzeMethod(node, semanticModel, arguments);

                case 1 when arguments[0].Expression is CastExpressionSyntax cast && cast.Type.IsObject():
                    return AnalyzeMethod(node, semanticModel, arguments);

                default:
                    return null;
            }
        }

        private Diagnostic AnalyzeMethod(InvocationExpressionSyntax node, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var symbol = node.GetSymbol(semanticModel);

            if (symbol is IMethodSymbol method)
            {
                var nodeToUnderline = node.Expression;

                if (IsEnumEqualsMethod(method))
                {
                    if (node.Expression is MemberAccessExpressionSyntax syntax)
                    {
                        nodeToUnderline = syntax.Name;
                    }

                    return Issue(symbol.Name, nodeToUnderline, "Enum.Equals");
                }

                var isStaticObjectEquals = IsObjectEqualsStaticMethod(method) && IsStruct(semanticModel, arguments);
                var isObjectEquals = IsObjectEqualsOnStructMethod(method);

                if (isStaticObjectEquals || isObjectEquals)
                {
                    // let's see who this method is that invokes Equals
                    var enclosingMethod = node.GetEnclosingMethod(semanticModel);

                    if (enclosingMethod.MethodKind != MethodKind.UserDefinedOperator)
                    {
                        if (isObjectEquals)
                        {
                            if (node.Expression is MemberAccessExpressionSyntax syntax)
                            {
                                nodeToUnderline = syntax.Name;
                            }
                        }

                        return Issue(symbol.Name, nodeToUnderline, "object.Equals");
                    }
                }
            }

            return null;
        }
    }
}