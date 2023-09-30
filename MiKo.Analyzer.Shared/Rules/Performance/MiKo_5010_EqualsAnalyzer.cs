using System.Collections.Generic;
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

        private static readonly SyntaxKind[] StrangeMarkers = { SyntaxKind.LogicalNotExpression, SyntaxKind.IsPatternExpression, SyntaxKind.ParenthesizedExpression };

        public MiKo_5010_EqualsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool IsObjectEqualsStaticMethod(IMethodSymbol method) => method.ContainingType.SpecialType == SpecialType.System_Object && method.IsStatic;

        private static bool IsObjectEqualsOnStructMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            // find out whether it is the non-static 'Equals(object)' method
            if (parameters.Length == 1 && parameters[0].Type.IsObject())
            {
                return method.ContainingType.IsValueType;
            }

            return false;
        }

        private static bool IsObjectEqualsInsideOwnOperator(ISymbol containingSymbol, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            if (arguments.Count == 2 && containingSymbol is IMethodSymbol enclosingMethod && enclosingMethod.MethodKind == MethodKind.UserDefinedOperator)
            {
                return IsSameType(arguments[0], semanticModel, enclosingMethod) || IsSameType(arguments[1], semanticModel, enclosingMethod);
            }

            return false;
        }

        private static bool IsOwnEqualsInsideOwnOperator(ISymbol containingSymbol, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            if (arguments.Count == 1 && containingSymbol is IMethodSymbol enclosingMethod && enclosingMethod.MethodKind == MethodKind.UserDefinedOperator)
            {
                return IsSameType(arguments[0], semanticModel, enclosingMethod);
            }

            return false;
        }

        private static bool IsDynamicOrGeneric(SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel) => arguments.Any(_ =>
                                                                                                                                                {
                                                                                                                                                    switch (_.GetTypeSymbol(semanticModel)?.TypeKind)
                                                                                                                                                    {
                                                                                                                                                        case TypeKind.Dynamic:
                                                                                                                                                        case TypeKind.TypeParameter:
                                                                                                                                                            return true;

                                                                                                                                                        default:
                                                                                                                                                            return false;
                                                                                                                                                    }
                                                                                                                                                });

        private static bool IsEnumEqualsMethod(IMethodSymbol method) => method.ContainingType.SpecialType == SpecialType.System_Enum;

        private static bool IsStringEqualsMethod(IMethodSymbol method) => method.ContainingType.SpecialType == SpecialType.System_String && method.Parameters.All(_ =>
                                                                                                                                                                      {
                                                                                                                                                                          var type = _.Type;

                                                                                                                                                                          return type.SpecialType == SpecialType.System_String || type.TypeKind == TypeKind.Enum;
                                                                                                                                                                      });

        private static bool IsStruct(SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel) => arguments.Any(_ => _.Expression.IsStruct(semanticModel));

        private static bool IsSameType(ArgumentSyntax argument, SemanticModel semanticModel, IMethodSymbol method) => method.ContainingType.Equals(argument.GetTypeSymbol(semanticModel), SymbolEqualityComparer.Default);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            // shortcut to not analyze each single invocation node
            if (node.Expression.GetName() == nameof(object.Equals))
            {
                var issue = AnalyzeEqualsInvocation(node, context.ContainingSymbol, context.SemanticModel);

                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeEqualsInvocation(InvocationExpressionSyntax node, ISymbol containingSymbol, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;

            switch (arguments.Count)
            {
                case 2:
                    return AnalyzeMethod(node, containingSymbol, semanticModel, arguments);

                case 1:

                    if (node.Parent.IsAnyKind(StrangeMarkers))
                    {
                        return AnalyzeMethod(node, containingSymbol, semanticModel, arguments);
                    }

                    var expression = arguments[0].Expression;

                    if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        return AnalyzeMethod(node, containingSymbol, semanticModel, arguments);
                    }

                    if (expression is CastExpressionSyntax cast && cast.Type.IsObject())
                    {
                        return AnalyzeMethod(node, containingSymbol, semanticModel, arguments);
                    }

                    return null;

                default:
                    return null;
            }
        }

        private Diagnostic AnalyzeMethod(InvocationExpressionSyntax node, ISymbol containingSymbol, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var symbol = node.GetSymbol(semanticModel);

            if (symbol is IMethodSymbol method)
            {
                return AnalyzeMethod(node, containingSymbol, semanticModel, arguments, method);
            }

            return null;
        }

        private Diagnostic AnalyzeMethod(InvocationExpressionSyntax node, ISymbol containingSymbol, SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol nodeSymbol)
        {
            if (IsOwnEqualsInsideOwnOperator(containingSymbol, semanticModel, arguments))
            {
                // operator on same type, so we do not report it
                return null;
            }

            if (IsEnumEqualsMethod(nodeSymbol))
            {
                if (node.Expression is MemberAccessExpressionSyntax syntax)
                {
                    return Issue(nodeSymbol.Name, syntax.Name, "Enum.Equals");
                }
            }

            if (IsStringEqualsMethod(nodeSymbol))
            {
                return null;
            }

            if (IsDynamicOrGeneric(arguments, semanticModel))
            {
                // we cannot handle dynamics or generics
                return null;
            }

            if (IsObjectEqualsStaticMethod(nodeSymbol))
            {
                if (IsStruct(arguments, semanticModel))
                {
                    // let's see who this method is that invokes Equals
                    if (containingSymbol is IMethodSymbol enclosingMethod && enclosingMethod.MethodKind == MethodKind.UserDefinedOperator)
                    {
                        return Issue(nodeSymbol.Name, node.Expression, "object.Equals", new Dictionary<string, string> { { "dummy", "dummy" } }); // marker to see that we have to handle an operator
                    }

                    return Issue(nodeSymbol.Name, node.Expression, "object.Equals");
                }

                // no struct, so no boxing
                return null;
            }

            if (IsObjectEqualsOnStructMethod(nodeSymbol))
            {
                // let's see who this method is that invokes Equals
                if (node.Expression is MemberAccessExpressionSyntax syntax)
                {
                    return Issue(nodeSymbol.Name, syntax.Name, "object.Equals");
                }

                return null;
            }

            // let's see who this method is that invokes Equals
            if (IsObjectEqualsInsideOwnOperator(containingSymbol, semanticModel, arguments))
            {
                // operator on same type, so we do not report it
                return null;
            }

            // seems specific Equals, so let's see if it is the negative one
            if (node.Parent.IsAnyKind(StrangeMarkers))
            {
                return Issue(nodeSymbol.Name, node.Expression, "Equals");
            }

            return null;
        }
    }
}