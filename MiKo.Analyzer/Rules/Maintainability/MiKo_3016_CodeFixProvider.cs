using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3016_CodeFixProvider)), Shared]
    public sealed class MiKo_3016_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzer.Id;

        protected override string Title => Resources.MiKo_3016_CodeFixTitle;

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = syntax.GetUsedParameter();
            var exceptionName = parameter is null
                                    ? nameof(InvalidOperationException)
                                    : nameof(ArgumentException);

            return SyntaxFactory.ParseTypeName(exceptionName);
        }

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argumentList = syntax.ArgumentList;
            if (argumentList is null)
            {
                return null;
            }

            var parameter = syntax.GetUsedParameter();
            if (parameter != null)
            {
                return ArgumentList(GetUpdatedErrorMessage(argumentList.Arguments), ParamName(parameter));
            }

            return ArgumentList(GetUpdatedErrorMessage(argumentList.Arguments));
        }
    }
}