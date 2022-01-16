using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3077_EnumPropertyHasDefaultValueAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3077";

        public MiKo_3077_EnumPropertyHasDefaultValueAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType().IsEnum();

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            if (HasIssue(symbol))
            {
                yield return Issue(symbol);
            }
        }

        private static bool HasIssue(IPropertySymbol property)
        {
            var propertySyntax = property.GetSyntax<PropertyDeclarationSyntax>();

            if (propertySyntax.Initializer != null)
            {
                // ignore initializers
                return false;
            }

            if (property.IsReadOnly)
            {
                ExpressionSyntax expression = null;

                if (propertySyntax.ExpressionBody != null)
                {
                    expression = propertySyntax.ExpressionBody.Expression;
                }
                else
                {
                    var accessorList = propertySyntax.AccessorList;
                    if (accessorList != null)
                    {
                        var getter = accessorList.Accessors[0];

                        if (getter.ExpressionBody != null)
                        {
                            expression = getter.ExpressionBody.Expression;
                        }
                        else
                        {
                            if (getter.Body?.Statements.FirstOrDefault() is ReturnStatementSyntax r)
                            {
                                expression = r.Expression;
                            }
                        }
                    }
                }

                switch (expression)
                {
                    case InvocationExpressionSyntax _: // ignore properties that simply return a fixed result
                    case MemberAccessExpressionSyntax _: // ignore properties that simply return a calculated result
                        return false;
                }
            }

            // try to find an assignment in the ctor(s)
            return IsNotAssignedInConstructor(property);
        }

        private static bool IsNotAssignedInConstructor(IPropertySymbol symbol)
        {
            var ctors = symbol.ContainingType.Constructors.Where(IsApplicableConstructor).ToList();

            if (ctors.None())
            {
                // no ctor, so we assume that it is not assigned
                return true;
            }

            var propertyName = symbol.Name;

            foreach (var ctor in ctors)
            {
                var syntaxNode = ctor.GetSyntax();

                if (syntaxNode.ChildNodes().Any(_ => _.IsKind(SyntaxKind.ThisConstructorInitializer)))
                {
                    // ignore ctors that invoke other ctors of same type as those would get reported as well in case of violations
                    continue;
                }

                var unassigned = syntaxNode.DescendantNodes()
                                           .Where(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                           .Cast<AssignmentExpressionSyntax>()
                                           .None(_ => _.Left is IdentifierNameSyntax i && i.GetName() == propertyName);

                if (unassigned)
                {
                    // we found no assignment, so report for that specific ctor (as it seems like the object can be created in a way that bypasses the default value assignment)
                    return true;
                }
            }

            return false;
        }

        private static bool IsApplicableConstructor(IMethodSymbol symbol) => symbol.IsConstructor() && symbol.IsSerializationConstructor() is false;
    }
}