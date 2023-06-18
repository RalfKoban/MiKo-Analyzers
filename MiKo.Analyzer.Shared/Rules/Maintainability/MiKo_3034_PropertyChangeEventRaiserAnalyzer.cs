using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3034_PropertyChangeEventRaiserAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3034";

        private static readonly HashSet<string> Mappings = new HashSet<string>
                                                               {
                                                                   nameof(PropertyChangedEventArgs),
                                                                   TypeNames.PropertyChangedEventArgs,

                                                                   nameof(PropertyChangingEventArgs),
                                                                   TypeNames.PropertyChangingEventArgs,
                                                               };

        public MiKo_3034_PropertyChangeEventRaiserAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;

            if (argumentList is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var arguments = argumentList.Arguments;

            if (arguments.Count != 1)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var method = node.GetEnclosingMethod(semanticModel);

            if (method is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            // no string parameters
            if (method.Parameters.All(_ => _.Type.IsString() is false))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (arguments[0].Expression is IdentifierNameSyntax s)
            {
                return AnalyzeParameter(s.GetName(), method.Parameters);
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeParameter(string propertyName, ImmutableArray<IParameterSymbol> parameters)
        {
            // TODO: RKN
            // x - get parent invocation
            // x - then check whether an PropertyChanging or PropertyChanged event gets raised
            // x - then check whether it's part of a method (body or block)

            // v - then check the arguments (note: for properties such as getter methods there might be no arguments)
            // v - at least one has to be a string -> should be provided as argument for the creation expression
            // v - that one has to have the attribute applied --> if not, report
            var parameter = parameters.FirstOrDefault(_ => _.Name == propertyName);

            if (parameter is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            foreach (var name in parameter.GetAttributeNames())
            {
                switch (name)
                {
                    case "CallerMemberName":
                    case nameof(CallerMemberNameAttribute):
                        return Enumerable.Empty<Diagnostic>();
                }
            }

            // no attribute on parameter: report issue
            return new[] { Issue(parameter) };
        }
    }
}