using System.Collections.Generic;
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

            var argument = arguments[0];
            return HasIssue(node, argument.Expression, semanticModel)
                       ? new[] { ReportIssue(node.Type.ToString(), argument.GetLocation()) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool HasIssue(SyntaxNode node, ExpressionSyntax argumentExpression, SemanticModel semanticModel)
        {
            if (argumentExpression is InvocationExpressionSyntax i && i.Expression is IdentifierNameSyntax sa && sa.Identifier.ValueText == "nameof")
            {
                return NameofHasIssue(node, i.ArgumentList.Arguments, semanticModel);
            }

            if (argumentExpression is IdentifierNameSyntax s && IdentifierIsParameter(node, s.Identifier.ValueText, semanticModel))
            {
                // it's a parameter, so don't report an issue
                return false;
            }

            return true; // report to use nameof instead
        }

        private static bool NameofHasIssue(SyntaxNode node, SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            // it might happen that the code is currently being written so there might not yet exist a specific property name
            var a = arguments.Select(_ => _.Expression).OfType<IdentifierNameSyntax>().FirstOrDefault();
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

            return true; // report to use nameof instead
        }

        private static bool IdentifierIsParameter(SyntaxNode node, string propertyName, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);
            if (method is null)
                return false;

            return method.Parameters.Any(_ => _.Name == propertyName);
        }
    }
}