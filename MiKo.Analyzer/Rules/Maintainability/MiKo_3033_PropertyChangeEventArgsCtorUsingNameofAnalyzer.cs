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

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;
            if (arguments.Count != 1)
                return Enumerable.Empty<Diagnostic>();

            return HasIssue(node, arguments[0].Expression, semanticModel)
                       ? new[] { ReportIssue(node.Type.ToString(), node.GetLocation()) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool HasIssue(SyntaxNode node, ExpressionSyntax argumentExpression, SemanticModel semanticModel)
        {
            if (argumentExpression is InvocationExpressionSyntax i && i.Expression is IdentifierNameSyntax s && s.Identifier.ValueText == "nameof")
            {
                // it might happen that the code is currently being written so there might not yet exist a specific property name
                var a = i.ArgumentList.Arguments.Select(_ => _.Expression).OfType<IdentifierNameSyntax>().FirstOrDefault();
                var propertyName = a?.Identifier.ValueText;
                if (propertyName is null)
                    return false;

                var symbol = node.GetEnclosingSymbol(semanticModel);
                var containingType = symbol?.ContainingType;
                if (containingType is null)
                    return false;

                // verify that nameof uses a property if the type
                if (containingType.GetMembers().OfType<IPropertySymbol>().Any(_ => _.Name == propertyName))
                    return false;
            }

            return true; // use nameof instead
        }
    }
}