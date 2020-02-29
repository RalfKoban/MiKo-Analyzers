﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3033_PropertyChangeEventArgsCtorUsingNameofAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3033";

        private static readonly HashSet<string> Mappings = new HashSet<string>
                                                               {
                                                                   nameof(PropertyChangedEventArgs),
                                                                   TypeNames.PropertyChangedEventArgs,

                                                                   nameof(PropertyChangingEventArgs),
                                                                   TypeNames.PropertyChangingEventArgs,
                                                               };

        public MiKo_3033_PropertyChangeEventArgsCtorUsingNameofAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.Contains(node.Type.ToString());

        //// TODO: Check for 'ObservableHelper.CreateArgs' and 'ObservableHelper.GetPropertyName'

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                yield break;
            }

            var arguments = argumentList.Arguments;
            if (arguments.Count != 1)
            {
                yield break;
            }

            var argument = arguments[0];
            if (HasIssue(node, argument.Expression, semanticModel))
            {
                yield return Issue(node.Type.ToString(), argument);
            }
        }

        private static bool HasIssue(SyntaxNode node, ExpressionSyntax argumentExpression, SemanticModel semanticModel)
        {
            switch (argumentExpression)
            {
                case InvocationExpressionSyntax i when i.Expression is IdentifierNameSyntax sa && sa.GetName() == "nameof":
                    return NameofHasIssue(node, i.ArgumentList.Arguments, semanticModel);

                case IdentifierNameSyntax s when IdentifierIsParameter(node, s.GetName(), semanticModel):
                    return false; // it's a parameter, so don't report an issue

                default:
                    return true; // report to use nameof instead
            }
        }

        private static bool NameofHasIssue(SyntaxNode node, SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            // it might happen that the code is currently being written so there might not yet exist a specific property name
            var a = arguments.Select(_ => _.Expression).OfType<IdentifierNameSyntax>().FirstOrDefault();
            var propertyName = a.GetName();
            if (propertyName is null)
            {
                return false;
            }

            var symbol = node.GetEnclosingSymbol(semanticModel);
            var containingType = symbol?.ContainingType;
            if (containingType is null)
            {
                return false;
            }

            // verify that nameof uses a property if the type
            var propertySymbols = containingType.GetMembersIncludingInherited<IPropertySymbol>();
            if (propertySymbols.Any(_ => _.Name == propertyName))
            {
                return false;
            }

            return true; // report to use nameof instead
        }

        private static bool IdentifierIsParameter(SyntaxNode node, string propertyName, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);
            if (method is null)
            {
                return false;
            }

            return method.Parameters.Any(_ => _.Name == propertyName);
        }
    }
}