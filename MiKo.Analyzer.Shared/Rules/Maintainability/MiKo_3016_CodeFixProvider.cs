﻿using System;
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
        public override string FixableDiagnosticId => "MiKo_3016";

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = syntax.GetUsedParameter();
            var exceptionName = parameter is null
                                ? nameof(InvalidOperationException)
                                : nameof(ArgumentException);

            return exceptionName.AsTypeSyntax();
        }

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var parameter = syntax.GetUsedParameter();
            var errorMessage = GetUpdatedErrorMessage(syntax.ArgumentList);

            return parameter != null
                   ? ArgumentList(errorMessage, ParamName(parameter))
                   : ArgumentList(errorMessage);
        }
    }
}