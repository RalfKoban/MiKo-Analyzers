using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MappingType = System.Func<Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>, Microsoft.CodeAnalysis.IMethodSymbol, Microsoft.CodeAnalysis.SemanticModel, MiKoSolutions.Analyzers.Rules.Maintainability.MiKo_3011_ArgumentExceptionsParamNameAnalyzer.InspectationResult>;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3011_ArgumentExceptionsParamNameAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3011";

        private static readonly IReadOnlyDictionary<string, MappingType> Mappings = new Dictionary<string, MappingType>
                                                                                        {
                                                                                            { nameof(ArgumentException), InspectArgumentException },
                                                                                            { TypeNames.ArgumentException, InspectArgumentException },

                                                                                            { nameof(ArgumentNullException), InspectArgumentNullException },
                                                                                            { TypeNames.ArgumentNullException, InspectArgumentNullException },

                                                                                            { nameof(ArgumentOutOfRangeException), InspectArgumentOutOfRangeException },
                                                                                            { TypeNames.ArgumentOutOfRangeException, InspectArgumentOutOfRangeException },

                                                                                            { nameof(InvalidEnumArgumentException), InspectInvalidEnumArgumentException },
                                                                                            { TypeNames.InvalidEnumArgumentException, InspectInvalidEnumArgumentException },
                                                                                        };

        public MiKo_3011_ArgumentExceptionsParamNameAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.ContainsKey(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);
            if (method is null || method.Parameters.Length == 0)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var type = node.Type.ToString();
            var inspector = Mappings[type];

            if (inspector(node.ArgumentList.Arguments, method, semanticModel) == InspectationResult.Report)
            {
                var issue = ReportIssue(type, node.GetLocation(), method.Parameters.Select(_ => string.Concat("nameof(", _.Name, ")")).HumanizedConcatenated());
                return new []{ issue };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static InspectationResult InspectArgumentException(SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol method, SemanticModel semanticModel)
        {
            switch (arguments.Count)
            {
                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[0], semanticModel))
                        return InspectArgument(arguments[1], method, semanticModel);

                    // it is not, so we have to report it anyway
                    break;

                case 3:
                    return InspectArgument(arguments[1], method, semanticModel);
            }

            return InspectationResult.Report;
        }

        private static InspectationResult InspectArgumentNullException(SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol method, SemanticModel semanticModel)
        {
            switch (arguments.Count)
            {
                case 1:
                    return InspectArgument(arguments[0], method, semanticModel);

                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[1], semanticModel))
                        return InspectArgument(arguments[0], method, semanticModel);

                    // it is not, so we have to report it anyway
                    break;
            }

            return InspectationResult.Report;
        }

        private static InspectationResult InspectArgumentOutOfRangeException(SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol method, SemanticModel semanticModel)
        {
            switch (arguments.Count)
            {
                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[1], semanticModel))
                        return InspectArgument(arguments[0], method, semanticModel);

                    // it is not, so we have to report it anyway
                    break;

                case 1:
                case 3:
                    return InspectArgument(arguments[0], method, semanticModel);
            }

            return InspectationResult.Report;
        }

        private static InspectationResult InspectInvalidEnumArgumentException(SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol method, SemanticModel semanticModel)
        {
            switch (arguments.Count)
            {
                case 3:
                    return InspectArgument(arguments[0], method, semanticModel);
            }

            return InspectationResult.Report;
        }

        private static InspectationResult InspectArgument(ArgumentSyntax argument, IMethodSymbol method, SemanticModel semanticModel)
        {
            if (IsStringParameter(argument, semanticModel) && ParameterIsReferenced(argument, method))
                return InspectationResult.None;

            // no string, so no paramName; hence we have to report it anyway
            return InspectationResult.Report;
        }

        private static bool IsStringParameter(ArgumentSyntax argument, SemanticModel semanticModel) => argument.Expression.IsString(semanticModel);

        private static bool ParameterIsReferenced(ArgumentSyntax argument, IMethodSymbol method)
        {
            var argumentName = argument.ToString();
            return method.Parameters.Select(_ => _.Name).Any(_ => argumentName == string.Concat("\"", _, "\"") || argumentName == string.Concat("nameof(", _, ")"));
        }

        internal enum InspectationResult : ushort
        {
            None = 0,
            Report = 1,
        }
    }
}