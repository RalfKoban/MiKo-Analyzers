using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class DependencyPropertyRegisterMaintainabilityAnalyzer : MaintainabilityAnalyzer
    {
        internal const string Value = "ParameterValue";

        private readonly string m_invocation;

        protected DependencyPropertyRegisterMaintainabilityAnalyzer(string diagnosticId, string invocation) : base(diagnosticId, SymbolKind.Field) => m_invocation = invocation;

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(m_invocation);

            if (arguments.Count < 3)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            // public static System.Windows.DependencyProperty Register(string name, Type propertyType, Type ownerType);
            // public static System.Windows.DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, System.Windows.PropertyMetadata typeMetadata);
            var nameArgument = arguments[0];
            var propertyType = arguments[1];
            var ownerType = arguments[2];

            return Enumerable.Empty<Diagnostic>()
                             .Concat(AnalyzeParameterName(symbol, nameArgument))
                             .Concat(AnalyzeParameterPropertyType(symbol, propertyType, nameArgument))
                             .Concat(AnalyzeParameterOwningType(symbol, ownerType));
        }

        private IEnumerable<Diagnostic> AnalyzeParameterName(IFieldSymbol symbol, ArgumentSyntax nameArgument)
        {
            var expression = nameArgument.Expression;

            if (expression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var name = ((LiteralExpressionSyntax)expression).GetName();

                yield return ReportIssue(symbol, nameArgument, "'nameof(" + name + ")'");
            }
        }

        private IEnumerable<Diagnostic> AnalyzeParameterPropertyType(IFieldSymbol symbol, ArgumentSyntax propertyType, ArgumentSyntax nameArgument)
        {
            if (propertyType.Expression is TypeOfExpressionSyntax syntax)
            {
                // find property and get the type
                var name = nameArgument.GetName();

                var owningType = symbol.ContainingType;
                var property = owningType.GetProperties().FirstOrDefault(_ => _.Name == name);

                if (property is null)
                {
                    // wrong name
                    yield return ReportIssue(symbol, nameArgument, "the name of an existing property");
                }
                else
                {
                    var returnType = property.GetReturnType()?.FullyQualifiedName();
                    var syntaxType = syntax.Type.ToString();

                    if (returnType != syntaxType)
                    {
                        // it might be that the syntax type is the same but the return type is fully qualified
                        // so check again for only the name part
                        var returnTypeNameOnlyPart = returnType?.GetNameOnlyPart();
                        var syntaxTypeNameOnlyPart = syntax.Type.GetNameOnlyPart();

                        if (returnTypeNameOnlyPart != syntaxTypeNameOnlyPart)
                        {
                            yield return ReportIssue(symbol, propertyType, returnType);
                        }
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeParameterOwningType(IFieldSymbol symbol, ArgumentSyntax ownerType)
        {
            if (ownerType.Expression is TypeOfExpressionSyntax syntax)
            {
                var owningType = symbol.ContainingType;
                var containingTypeName = owningType.Name;

                if (containingTypeName != syntax.Type.ToString())
                {
                    yield return ReportIssue(symbol, ownerType, containingTypeName);
                }
            }
        }

        private Diagnostic ReportIssue(IFieldSymbol symbol, ArgumentSyntax argument, string parameter)
        {
            return Issue(symbol.Name, argument, parameter, new Dictionary<string, string> { { Value, parameter } });
        }
    }
}