using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2230_ReturnValueUsesListAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2230";

        public MiKo_2230_ReturnValueUsesListAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            foreach (var token in comment.GetXmlSyntax(xmlTag).GetXmlTextTokens())
            {
                var text = token.ValueText;

                if (text.Contains(Constants.Comments.ValueMeaningPhrase, StringComparison.Ordinal))
                {
                    yield return Issue(token);
                }
            }
        }
    }
}
