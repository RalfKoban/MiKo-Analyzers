using System;
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
            var parameter = syntax.GetUsedParameter();

            // there might be multiple parameters, so we have to find out which parameter is meant
            if (parameter != null)
            {
                var arguments = GetUpdatedArgumentListSyntaxForParameter(syntax, parameter);
                if (arguments != null)
                {
                    return arguments;
                }
            }

            // it might be a local variable inside a switch, so we have to find out which one
            if (syntax.GetEnclosing<SwitchStatementSyntax>()?.Expression is IdentifierNameSyntax identifier)
            {
                var arguments = GetUpdatedArgumentListSyntaxForIdentifier(syntax, identifier);
                if (arguments != null)
                {
                    return arguments;
                }
            }

            return syntax.ArgumentList;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListSyntaxForParameter(ObjectCreationExpressionSyntax syntax, ParameterSyntax parameter)
        {
            switch (syntax.Type.GetNameOnlyPart())
            {
                case nameof(ArgumentOutOfRangeException):
                {
                    var argumentList = syntax.ArgumentList;

                    switch (argumentList.Arguments.Count)
                    {
                        case 0: // missing message, so add a TODO
                        case 1: // it's either the parameter or the message instead of the parameter
                        case 2: // message and parameter (might be switched)
                            return ArgumentList(ParamName(parameter), Argument(parameter), GetUpdatedErrorMessage(argumentList));
                    }

                    break;
                }

                case nameof(InvalidEnumArgumentException):
                    return ArgumentList(ParamName(parameter), ArgumentWithCast(SyntaxKind.IntKeyword, parameter), Argument(TypeOf(parameter)));
            }

            return null;
        }

        private static ArgumentListSyntax GetUpdatedArgumentListSyntaxForIdentifier(ObjectCreationExpressionSyntax syntax, IdentifierNameSyntax identifier)
        {
            switch (syntax.Type.GetNameOnlyPart())
            {
                case nameof(ArgumentOutOfRangeException):
                    return ArgumentList(ParamName(identifier), Argument(identifier), GetUpdatedErrorMessage(syntax.ArgumentList));

                case nameof(InvalidEnumArgumentException):
                    return ArgumentList(
                                    ParamName(identifier),
                                    ArgumentWithCast(SyntaxKind.IntKeyword, identifier),
                                    Argument(Invocation(SimpleMemberAccess(identifier, nameof(GetType))))); // use .GetType() call as we are not sure which type the identifier has
            }

            return null;
        }
    }
}