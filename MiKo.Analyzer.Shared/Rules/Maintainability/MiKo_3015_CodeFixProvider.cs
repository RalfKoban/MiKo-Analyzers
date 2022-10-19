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
            var arguments = argumentList.Arguments;

            var errorMessage = arguments.Count == 3
                                   ? GetUpdatedErrorMessage(arguments.RemoveAt(1)) // actual argument seems to be part of the exception, so we have to ignore it when trying to find the error message
                                   : GetUpdatedErrorMessage(argumentList);

            return ArgumentList(errorMessage);
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue) => root.WithoutUsing("System.ComponentModel"); // remove unused "using System.ComponentModel;"
    }
}