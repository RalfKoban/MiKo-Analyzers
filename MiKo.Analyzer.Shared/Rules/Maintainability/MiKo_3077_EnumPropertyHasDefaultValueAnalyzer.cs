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

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation) => HasIssue(symbol)
                                                                                                               ? new[] { Issue(symbol) }
                                                                                                               : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(IPropertySymbol property)
        {
            var syntax = property.GetSyntax<PropertyDeclarationSyntax>();

            if (syntax is null)
            {
                // seems that we have a record
                return false;
            }

            if (syntax.Initializer != null)
            {
                // ignore initializers
                return false;
            }

            if (property.IsReadOnly)
            {
                var expression = syntax.GetPropertyExpression();

                switch (expression)
                {
                    case InvocationExpressionSyntax _: // ignore properties that simply return a fixed result
                    case MemberAccessExpressionSyntax _: // ignore properties that simply return a calculated result
                        return false;
                }
            }

            // try to find an assignment in the ctor(s)
            return IsNotAssignedInConstructor(property, syntax);
        }

        private static bool IsNotAssignedInConstructor(IPropertySymbol symbol, PropertyDeclarationSyntax syntax)
        {
            var ctors = symbol.ContainingType.Constructors.Where(IsApplicableConstructor).ToList();

            if (ctors.None())
            {
                // no ctor, so we assume that it is not assigned
                return true;
            }

            var backingField = GetBackingField(symbol, syntax);

            foreach (var ctor in ctors)
            {
                var syntaxNode = ctor.GetSyntax();

                var unassigned = true;

                if (syntaxNode is null)
                {
                    // seems like there is no ctor in source code
                }
                else
                {
                    if (syntaxNode.ChildNodes().Any(_ => _.IsKind(SyntaxKind.ThisConstructorInitializer)))
                    {
                        // ignore ctors that invoke other ctors of same type as those would get reported as well in case of violations
                        continue;
                    }

                    foreach (var node in syntaxNode.DescendantNodes<AssignmentExpressionSyntax>(SyntaxKind.SimpleAssignmentExpression))
                    {
                        if (node.Left is IdentifierNameSyntax i)
                        {
                            var identifier = i.GetName();

                            if (identifier == symbol.Name)
                            {
                                // we found a property assignment, so we have to check for backing field assignments
                                unassigned = false;

                                break;
                            }

                            if (backingField != null && identifier == backingField.Name)
                            {
                                // we found a backing field assignment
                                unassigned = false;

                                break;
                            }
                        }
                    }
                }

                if (unassigned)
                {
                    if (backingField != null)
                    {
                        // here we have to inspect if we have a default value already set on the field
                        return backingField.GetSyntax().DescendantNodes<EqualsValueClauseSyntax>().None();
                    }

                    // we found no assignment, so report for that specific ctor (as it seems like the object can be created in a way that bypasses the default value assignment)
                    return true;
                }
            }

            return false;
        }

        private static bool IsApplicableConstructor(IMethodSymbol symbol) => symbol.IsConstructor() && symbol.IsSerializationConstructor() is false;

        private static IFieldSymbol GetBackingField(IPropertySymbol symbol, PropertyDeclarationSyntax syntax)
        {
            var name = syntax.GetIdentifierNameFromPropertyExpression();

            if (name != null)
            {
                return symbol.ContainingType.GetFields().FirstOrDefault(_ => _.Name == name);
            }

            return null;
        }
    }
}