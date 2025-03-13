﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2051_ExceptionTagDefaultPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2051";

        public MiKo_2051_ExceptionTagDefaultPhraseAnalyzer() : base(Id, typeof(Exception))
        {
        }

        protected override IEnumerable<XmlElementSyntax> GetExceptionComments(DocumentationCommentTriviaSyntax documentation) => documentation.GetExceptionXmls();

        protected override IReadOnlyList<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment)
        {
            // get rid of the para tags as we are not interested into them
            var comment = exceptionComment.GetTextTrimmed();

            if (comment.StartsWithAny(Constants.Comments.ExceptionForbiddenStartingPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { Issue(symbol.Name, exceptionComment.GetContentsLocation()) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}