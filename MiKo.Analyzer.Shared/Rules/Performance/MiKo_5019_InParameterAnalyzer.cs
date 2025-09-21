using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5019_InParameterAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5019";

        private static readonly SyntaxKind[] PrefixedExpressions = { SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression };

        private static readonly SyntaxKind[] PostfixedExpressions = { SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression };

        public MiKo_5019_InParameterAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters;

            if (parameters.Length is 0)
            {
                return false;
            }

            if (parameters.Length is 1 && symbol.Name.StartsWith("Analyze", StringComparison.Ordinal) && parameters[0].Name is "context")
            {
                return false;
            }

            if (symbol.IsOverride)
            {
                return false;
            }

            if (symbol.IsAsync)
            {
                return false;
            }

            if (symbol.CanBeReferencedByName)
            {
                if (symbol.ReturnType.IsTask())
                {
                    return false;
                }

                if (symbol.IsInterfaceImplementation())
                {
                    return false;
                }
            }
            else
            {
                // seems to be a ctor or operator
                switch (symbol.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.Conversion:
                    case MethodKind.UserDefinedOperator:
                        break;

                    default:
                        return false;
                }
            }

            return symbol.GetSyntax().DescendantNodes<YieldStatementSyntax>().None();
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.Parameters.SelectMany(_ => AnalyzeParameter(_, compilation));

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation)
        {
            var type = symbol.Type;

            if (IsReadOnlyStruct(type))
            {
                if (symbol.RefKind is RefKind.None)
                {
                    if (symbol.HasAttribute("NUnit.Framework.RangeAttribute"))
                    {
                        // NUnit seems to have an issue here, so we cannot change the code
                        return Array.Empty<Diagnostic>();
                    }

                    var parameterName = symbol.Name;
                    var syntaxNode = symbol.ContainingSymbol.GetSyntax();

                    foreach (var node in syntaxNode.AllDescendantNodes())
                    {
                        switch (node)
                        {
                            case LambdaExpressionSyntax lambda when lambda.DescendantNodes<IdentifierNameSyntax>().Any(_ => _.GetName() == parameterName):
                            {
                                // cannot fix lambdas
                                return Array.Empty<Diagnostic>();
                            }

                            case AssignmentExpressionSyntax assignment when assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) && assignment.Left is IdentifierNameSyntax identifier && identifier.GetName() == parameterName:
                            {
                                // cannot fix assignments
                                return Array.Empty<Diagnostic>();
                            }

                            case PrefixUnaryExpressionSyntax prefix when prefix.IsAnyKind(PrefixedExpressions) && prefix.Operand is IdentifierNameSyntax identifier && identifier.GetName() == parameterName:
                            {
                                // cannot fix increments or decrements
                                return Array.Empty<Diagnostic>();
                            }

                            case PostfixUnaryExpressionSyntax postfix when postfix.IsAnyKind(PostfixedExpressions) && postfix.Operand is IdentifierNameSyntax identifier && identifier.GetName() == parameterName:
                            {
                                // cannot fix increments or decrements
                                return Array.Empty<Diagnostic>();
                            }
                        }
                    }

                    return new[] { Issue(symbol) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsReadOnlyStruct(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    return true;

                default:
                    switch (type.TypeKind)
                    {
                        case TypeKind.Struct when type.IsReadOnly:
                        case TypeKind.Enum when type is INamedTypeSymbol namedType && namedType.EnumUnderlyingType?.IsReadOnly is true:
                            return true;

                        default:
                            return false;
                    }
            }
        }
    }
}