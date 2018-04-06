using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MappingType = System.Func<Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>, Microsoft.CodeAnalysis.IMethodSymbol, Microsoft.CodeAnalysis.SemanticModel, MiKoSolutions.Analyzers.Rules.Maintainability.MiKo_3011_ArgumentExceptionsParamNameAnalyzer.InspectationResult>;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3011_ArgumentExceptionsParamNameAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3011";

        private static readonly ConcurrentDictionary<string, MappingType> Mappings = new ConcurrentDictionary<string, MappingType>(new Dictionary<string, MappingType>
                                                                                                                                       {
                                                                                                                                           { nameof(ArgumentException), InspectArgumentException },
                                                                                                                                           { "System." + nameof(ArgumentException), InspectArgumentException },

                                                                                                                                           { nameof(ArgumentNullException), InspectArgumentNullException },
                                                                                                                                           { "System." + nameof(ArgumentNullException), InspectArgumentNullException },

                                                                                                                                           { nameof(ArgumentOutOfRangeException), InspectArgumentOutOfRangeException },
                                                                                                                                           { "System." + nameof(ArgumentOutOfRangeException), InspectArgumentOutOfRangeException },

                                                                                                                                           { nameof(InvalidEnumArgumentException), InspectInvalidEnumArgumentException },
                                                                                                                                           { "System.ComponentModel." + nameof(InvalidEnumArgumentException), InspectInvalidEnumArgumentException },
                                                                                                                                       });

        public MiKo_3011_ArgumentExceptionsParamNameAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeObjectCreation(node, context.SemanticModel);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var type = node.Type.ToString();
            if (Mappings.TryGetValue(type, out var inspector))
            {
                var method = node.GetEnclosingMethod(semanticModel);

                if (inspector(node.ArgumentList.Arguments, method, semanticModel) == InspectationResult.Report)
                {
                    return ReportIssue(type, node.GetLocation(), method?.Parameters.Select(_ => string.Concat("nameof(", _.Name, ")")).HumanizedConcatenated());
                }
            }
            return null;
        }

        private static InspectationResult InspectArgumentException(SeparatedSyntaxList<ArgumentSyntax> arguments, IMethodSymbol method, SemanticModel semanticModel)
        {
            switch (arguments.Count)
            {
                case 2:
                    // is it the message parameter ctor?
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
                    // is it the message parameter ctor?
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
                    // is it the message parameter ctor?
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

        private static bool IsStringParameter(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(argument.Expression);
            return typeInfo.Type.SpecialType == SpecialType.System_String;
        }

        private static bool ParameterIsReferenced(ArgumentSyntax argument, IMethodSymbol method)
        {
            if (method is null)
                return false;

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