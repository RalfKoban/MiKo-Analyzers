using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3013_CodeFixProvider)), Shared]
    public sealed class MiKo_3013_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3013";

        protected override string Title => Resources.MiKo_3013_CodeFixTitle;

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => SyntaxFactory.ParseTypeName(nameof(ArgumentOutOfRangeException));

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argumentList = syntax.ArgumentList;

            // there might be multiple parameters, so we have to find out which parameter is meant
            var parameter = syntax.GetUsedParameter();

            if (parameter != null)
            {
                switch (argumentList.Arguments.Count)
                {
                    case 0: // missing message, so add a TODO
                    case 1: // it's either the parameter or the message instead of the parameter
                    case 2: // message and parameter (might be switched)
                        return ArgumentList(ParamName(parameter), Argument(parameter), GetUpdatedErrorMessage(argumentList));
                }
            }

            // it might be a local variable inside a switch, so we have to find out which one
            if (syntax.GetEnclosing<SwitchStatementSyntax>()?.Expression is IdentifierNameSyntax identifier)
            {
                return ArgumentList(ParamName(identifier), Argument(identifier), GetUpdatedErrorMessage(argumentList));
            }

            return argumentList;
        }
    }
}