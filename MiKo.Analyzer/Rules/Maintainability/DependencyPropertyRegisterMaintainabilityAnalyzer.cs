using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class DependencyPropertyRegisterMaintainabilityAnalyzer : MaintainabilityAnalyzer
    {
        private readonly string m_invocation;

        protected DependencyPropertyRegisterMaintainabilityAnalyzer(string diagnosticId, string invocation) : base(diagnosticId, SymbolKind.Field) => m_invocation = invocation;

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol)
        {
            var arguments = GetInvocationArguments(symbol);
            if (arguments.Count < 3)
                return Enumerable.Empty<Diagnostic>();

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
            if (nameArgument.Expression.Kind() is SyntaxKind.StringLiteralExpression)
            {
                ReportIssue(symbol, nameArgument, "nameof", ref diagnostics);
            }
        }

        private void AnalyzeParameterPropertyType(IFieldSymbol symbol, ArgumentSyntax propertyType, ArgumentSyntax nameArgument, ref List<Diagnostic> diagnostics)
        {
            if (propertyType.Expression is TypeOfExpressionSyntax syntax)
            {
                // find property and get the type
                var name = GetName(nameArgument);

                var owningType = symbol.ContainingType;
                var property = owningType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(_ => _.Name == name);
                if (property is null)
                {
                    // wrong name
                    ReportIssue(symbol, nameArgument, "an existing property", ref diagnostics);
                }
                else
                {
                    var returnType = property.GetReturnType()?.ToString();
                    if (returnType != syntax.Type.ToString())
                    {
                        ReportIssue(symbol, propertyType, returnType, ref diagnostics);
                    }
                }
            }
        }

        private void AnalyzeParameterOwningType(IFieldSymbol symbol, ArgumentSyntax ownerType, ref List<Diagnostic> diagnostics)
        {
            if (ownerType.Expression is TypeOfExpressionSyntax syntax)
            {
                var owningType = symbol.ContainingType;
                var containingTypeName = owningType.Name;
                if (containingTypeName != syntax.Type.ToString())
                {
                    ReportIssue(symbol, ownerType, containingTypeName, ref diagnostics);
                }
            }
        }

        private void ReportIssue(IFieldSymbol symbol, ArgumentSyntax argument, object parameter, ref List<Diagnostic> diagnostics)
        {
            if (diagnostics is null)
                diagnostics = new List<Diagnostic>();

            diagnostics.Add(ReportIssue(symbol.Name, argument.GetLocation(), parameter));
        }

        private static string GetName(ArgumentSyntax nameArgument)
        {
            switch (nameArgument.Expression)
            {
                // nameof
                case InvocationExpressionSyntax i: return i.ArgumentList.Arguments[0].ToString();
                case LiteralExpressionSyntax l: return l.Token.ValueText;
                default: return string.Empty;
            }
        }

        private SeparatedSyntaxList<ArgumentSyntax> GetInvocationArguments(IFieldSymbol symbol) => symbol.GetAssignmentsVia(m_invocation)
                                                                                                         .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                                         .Select(_ => _.ArgumentList.Arguments)
                                                                                                         .FirstOrDefault(_ => _.Count > 0);
    }
}