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

            List<Diagnostic> diagnostics = null;

            AnalyzeParameterName(symbol, nameArgument, ref diagnostics);
            AnalyzeParameterPropertyType(symbol, propertyType, nameArgument, ref diagnostics);
            AnalyzeParameterOwningType(symbol, ownerType, ref diagnostics);

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private void AnalyzeParameterName(IFieldSymbol symbol, ArgumentSyntax nameArgument, ref List<Diagnostic> diagnostics)
        {
            var expression = nameArgument.Expression;

            if (expression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var name = ((LiteralExpressionSyntax)expression).GetName();
                ReportIssue(symbol, nameArgument, "'nameof(" + name + ")'", ref diagnostics);
            }
        }

        private void AnalyzeParameterPropertyType(IFieldSymbol symbol, ArgumentSyntax propertyType, ArgumentSyntax nameArgument, ref List<Diagnostic> diagnostics)
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
                    ReportIssue(symbol, nameArgument, "the name of an existing property", ref diagnostics);
                }
                else
                {
                    var returnType = property.GetReturnType()?.FullyQualifiedName();
                    var syntaxType = syntax.Type.ToString();

                    // it might be that the syntax type is the same but the return type is fully qualified
                    // so check again for only the name part
                    if (returnType == syntaxType)
                    {
                        return;
                    }

                    var returnTypeNameOnlyPart = returnType?.GetNameOnlyPart();
                    var syntaxTypeNameOnlyPart = syntax.Type.GetNameOnlyPart();

                    if (returnTypeNameOnlyPart != syntaxTypeNameOnlyPart)
                    {
                        ReportIssue(symbol, propertyType, returnType, ref diagnostics);
                    }
                }
            }
        }

        private void AnalyzeParameterOwningType(IFieldSymbol symbol, ArgumentSyntax ownerType, ref List<Diagnostic> results)
        {
            if (ownerType.Expression is TypeOfExpressionSyntax syntax)
            {
                var owningType = symbol.ContainingType;
                var containingTypeName = owningType.Name;
                if (containingTypeName != syntax.Type.ToString())
                {
                    ReportIssue(symbol, ownerType, containingTypeName, ref results);
                }
            }
        }

        private void ReportIssue(IFieldSymbol symbol, ArgumentSyntax argument, string parameter, ref List<Diagnostic> results)
        {
            if (results is null)
            {
                results = new List<Diagnostic>(1);
            }

            var properties = new Dictionary<string, string>
                                 {
                                     { Value, parameter },
                                 };

            results.Add(Issue(symbol.Name, argument, parameter, properties));
        }
    }
}