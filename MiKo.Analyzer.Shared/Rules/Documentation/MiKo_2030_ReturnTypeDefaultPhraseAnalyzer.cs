﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2030_ReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2030";

        public MiKo_2030_ReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            if (returnType.IsBoolean())
            {
                return false; // checked by MiKo_2032
            }

            if (returnType.IsString())
            {
                return false; // checked by MiKo_2033
            }

            if (returnType.IsEnum())
            {
                return false; // checked by MiKo_2034
            }

            if (returnType.IsEnumerable())
            {
                return false; // checked by MiKo_2035
            }

            if (returnType.IsTask())
            {
                return false; // checked by MiKo_2031, MiKo_2032, MiKo_2033
            }

            return true;
        }

        protected override Diagnostic[] AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            return AnalyzeStartingPhrase(owningSymbol, comment, commentXml, xmlTag, Constants.Comments.ReturnTypeStartingPhrase);
        }
    }
}