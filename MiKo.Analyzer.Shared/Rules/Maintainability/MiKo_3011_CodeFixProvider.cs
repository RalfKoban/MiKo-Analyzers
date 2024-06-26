using System;
using System.ComponentModel;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3011_CodeFixProvider)), Shared]
    public sealed class MiKo_3011_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3011";

        protected override string Title => Resources.MiKo_3011_CodeFixTitle;

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            // there might be multiple parameters, so we have to find out which parameter is meant
            var parameter = syntax.GetUsedParameter();

            if (parameter != null)
            {
                switch (syntax.Type.GetNameOnlyPart())
                {
                    case nameof(ArgumentException):
                        return GetUpdatedArgumentListForArgumentException(syntax.ArgumentList, parameter);

                    case nameof(ArgumentNullException):
                        return GetUpdatedArgumentListForArgumentNullException(syntax.ArgumentList, parameter);

                    case nameof(ArgumentOutOfRangeException):
                        return GetUpdatedArgumentListForArgumentOutOfRangeException(syntax.ArgumentList, parameter);

                    case nameof(InvalidEnumArgumentException):
                        return GetUpdatedArgumentListForInvalidEnumArgumentException(syntax.ArgumentList, parameter);
                }
            }

            return syntax.ArgumentList;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                    return ArgumentList(ToDo(), ParamName(parameter));

                case 1: // it's only the message
                {
                    var argument = arguments[0];

                    if (argument.ToString() == parameter.GetName().SurroundedWithDoubleQuote())
                    {
                        // seems like the 'message' parameter has been misused for the parameter name
                        return ArgumentList(ToDo(), ParamName(parameter));
                    }

                    return ArgumentList(argument, ParamName(parameter));
                }

                case 2: // switched message and parameter
                    return ArgumentList(arguments[1], ParamName(parameter));

                case 3: // switched message and parameter
                    return ArgumentList(arguments[1], ParamName(parameter), arguments[2]);
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentNullException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                    return ArgumentList(ParamName(parameter), ToDo());

                case 1: // it's the message instead of the parameter
                case 2: // switched message and parameter
                    return ArgumentList(ParamName(parameter), arguments[0]);
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentOutOfRangeException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                    return ArgumentList(ParamName(parameter), Argument(parameter), ToDo());

                case 1: // it's the message instead of the parameter
                case 2: // switched message and parameter
                    return ArgumentList(ParamName(parameter), Argument(parameter), arguments[0]);
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForInvalidEnumArgumentException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing data
                case 1: // it's only the message
                case 3: // switched message and parameter
                {
                    return ArgumentList(ParamName(parameter), ArgumentWithCast(SyntaxKind.IntKeyword, parameter), Argument(TypeOf(parameter)));
                }
            }

            return originalArguments;
        }
    }
}