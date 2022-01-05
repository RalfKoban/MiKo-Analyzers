using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3015_CodeFixProvider)), Shared]
    public sealed class MiKo_3015_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzer.Id;

        protected override string Title => Resources.MiKo_3015_CodeFixTitle;

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => SyntaxFactory.ParseTypeName(nameof(InvalidOperationException));

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argumentList = syntax.ArgumentList;
            if (argumentList is null)
            {
                return null;
            }

            var arguments = argumentList.Arguments;
            switch (arguments.Count)
            {
                case 3: // actual argument seems to be part of the exception, so we have to ignore it when trying to find the error message
                    return ArgumentList(GetUpdatedErrorMessage(arguments.RemoveAt(1)));

                // case 0: // missing message, so add a TODO
                // case 1: // it's either the parameter or the message instead of the parameter
                // case 2: // message and parameter (might be switched)
                default:
                    return ArgumentList(GetUpdatedErrorMessage(arguments));
            }
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic) => WithoutUsing(root, "System.ComponentModel"); // remove unused "using System.ComponentModel;"
    }
}