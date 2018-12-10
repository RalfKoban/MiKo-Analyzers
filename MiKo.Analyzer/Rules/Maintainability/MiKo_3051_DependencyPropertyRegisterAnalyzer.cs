using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3051_DependencyPropertyRegisterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3051";

        private const string DependencyPropertyRegisterInvocation = "DependencyProperty.Register";

        public MiKo_3051_DependencyPropertyRegisterAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol)
        {
            var arguments = GetRegisterArguments(symbol);
            if (arguments.Count < 3)
                return Enumerable.Empty<Diagnostic>();

            // public static System.Windows.DependencyProperty Register(string name, Type propertyType, Type ownerType);
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

        private static SeparatedSyntaxList<ArgumentSyntax> GetRegisterArguments(ISymbol symbol) => symbol.DeclaringSyntaxReferences
                                                                                                         .Select(_ => _.GetSyntax())
                                                                                                         .Select(_ => _.GetEnclosing<FieldDeclarationSyntax>())
                                                                                                         .SelectMany(_ => _.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Where(__ => __.ToString() == DependencyPropertyRegisterInvocation))
                                                                                                         .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                                         .Select(_ => _.ArgumentList.Arguments)
                                                                                                         .FirstOrDefault(_ => _.Count > 0);
    }
}