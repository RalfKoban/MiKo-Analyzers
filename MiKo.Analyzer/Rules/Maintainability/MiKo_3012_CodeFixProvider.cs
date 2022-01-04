﻿using System;
using System.ComponentModel;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3012_CodeFixProvider)), Shared]
    public sealed class MiKo_3012_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer.Id;

        protected override string Title => Resources.MiKo_3012_CodeFixTitle;

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = FindUsedParameter(syntax);

            // there might be multiple parameters, so we have to find out which parameter is meant
            if (parameter != null)
            {
                switch (syntax.Type.GetNameOnlyPart())
                {
                    case nameof(ArgumentOutOfRangeException):
                        return GetUpdatedArgumentListForArgumentOutOfRangeException(syntax.ArgumentList, parameter);

                    case nameof(InvalidEnumArgumentException):
                        return GetUpdatedArgumentListForInvalidEnumArgumentException(parameter);
                }
            }

            return syntax.ArgumentList;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForArgumentOutOfRangeException(ArgumentListSyntax originalArguments, ParameterSyntax parameter)
        {
            var arguments = originalArguments.Arguments;

            switch (arguments.Count)
            {
                case 0: // missing message, so add a TODO
                case 1: // it's either the parameter or the message instead of the parameter
                case 2: // message and parameter (might be switched)
                    return ArgumentList(ParamName(parameter), Argument(parameter), GetUpdatedErrorMessage(arguments));
            }

            return originalArguments;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListForInvalidEnumArgumentException(ParameterSyntax parameter)
        {
            return ArgumentList(ParamName(parameter), Argument(parameter, SyntaxKind.IntKeyword), Argument(TypeOf(parameter)));
        }
    }
}