using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MappingType = System.Func<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax, Microsoft.CodeAnalysis.IMethodSymbol, Microsoft.CodeAnalysis.SemanticModel, Microsoft.CodeAnalysis.Location>;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3011_ArgumentExceptionsParamNameAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3011";

        private static readonly IReadOnlyDictionary<string, MappingType> Mappings = new Dictionary<string, MappingType>
                                                                                        {
                                                                                            // ArgumentException
                                                                                            { nameof(ArgumentException), InspectArgumentException },
                                                                                            { TypeNames.ArgumentException, InspectArgumentException },

                                                                                            // ArgumentNullException
                                                                                            { nameof(ArgumentNullException), InspectArgumentNullException },
                                                                                            { TypeNames.ArgumentNullException, InspectArgumentNullException },

                                                                                            // ArgumentOutOfRangeException
                                                                                            { nameof(ArgumentOutOfRangeException), InspectArgumentOutOfRangeException },
                                                                                            { TypeNames.ArgumentOutOfRangeException, InspectArgumentOutOfRangeException },

                                                                                            // InvalidEnumArgumentException
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

            var location = inspector(node.ArgumentList, method, semanticModel);
            if (location != Location.None)
            {
                var parameters = GetParameterNames(node, method);
                var issue = Issue(type, location, parameters);
                return new[] { issue };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static Location InspectArgumentException(ArgumentListSyntax syntax, IMethodSymbol method, SemanticModel semanticModel)
        {
            var arguments = syntax.Arguments;
            switch (arguments.Count)
            {
                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[0], semanticModel))
                    {
                        return InspectArgument(arguments[1], method, semanticModel);
                    }

                    // it is not, so we have to report it anyway
                    break;

                case 3:
                    return InspectArgument(arguments[1], method, semanticModel);
            }

            return GetLocation(syntax);
        }

        private static Location InspectArgumentNullException(ArgumentListSyntax syntax, IMethodSymbol method, SemanticModel semanticModel)
        {
            var arguments = syntax.Arguments;
            switch (arguments.Count)
            {
                case 1:
                    return InspectArgument(arguments[0], method, semanticModel);

                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[1], semanticModel))
                    {
                        return InspectArgument(arguments[0], method, semanticModel);
                    }

                    // it is not, so we have to report it anyway
                    break;
            }

            return GetLocation(syntax);
        }

        private static Location InspectArgumentOutOfRangeException(ArgumentListSyntax syntax, IMethodSymbol method, SemanticModel semanticModel)
        {
            var arguments = syntax.Arguments;
            switch (arguments.Count)
            {
                case 2:
                    // find out if it's the message parameter ctor
                    if (IsStringParameter(arguments[1], semanticModel))
                    {
                        return InspectArgument(arguments[0], method, semanticModel);
                    }

                    // it is not, so we have to report it anyway
                    break;

                case 1:
                case 3:
                    return InspectArgument(arguments[0], method, semanticModel);
            }

            return GetLocation(syntax);
        }

        private static Location InspectInvalidEnumArgumentException(ArgumentListSyntax syntax, IMethodSymbol method, SemanticModel semanticModel)
        {
            var arguments = syntax.Arguments;
            switch (arguments.Count)
            {
                case 3:
                    return InspectArgument(arguments[0], method, semanticModel);
            }

            return GetLocation(syntax);
        }

        private static Location InspectArgument(ArgumentSyntax argument, IMethodSymbol method, SemanticModel semanticModel)
        {
            if (IsStringParameter(argument, semanticModel) && ParameterIsReferenced(argument, method))
            {
                return Location.None;
            }

            // no string, so no paramName; hence we have to report it anyway
            return argument.GetLocation();
        }

        private static bool IsStringParameter(ArgumentSyntax argument, SemanticModel semanticModel) => argument.Expression.IsString(semanticModel);

        private static bool ParameterIsReferenced(ArgumentSyntax argument, IMethodSymbol method)
        {
            var argumentName = argument.ToString();

            return method.Parameters.Select(_ => _.Name).Any(_ => argumentName == _.SurroundedWithDoubleQuote() || argumentName == AsNameof(_));
        }

        private static Location GetLocation(ArgumentListSyntax syntax) => Location.Create(syntax.SyntaxTree, syntax.Arguments.Span);

        private static string GetParameterNames(SyntaxNode node, IMethodSymbol method)
        {
            var parameters = method.Parameters.Select(_ => AsNameof(_.Name)).HumanizedConcatenated();

            var ifStatement = node.GetEnclosing<IfStatementSyntax>();
            if (ifStatement != null)
            {
                var identifiers = ifStatement.Condition.DescendantNodes().OfType<IdentifierNameSyntax>().Select(_ => _.GetName()).ToHashSet();
                var parameter = method.Parameters.FirstOrDefault(_ => identifiers.Contains(_.Name));
                if (parameter != null)
                {
                    parameters = parameter.Name.SurroundedWithApostrophe();
                }
            }

            return parameters;
        }

        private static string AsNameof(string value) => string.Concat("nameof(", value, ")");
    }
}