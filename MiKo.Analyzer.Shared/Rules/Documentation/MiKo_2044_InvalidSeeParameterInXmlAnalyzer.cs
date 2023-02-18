using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2044";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;

            if (method.Parameters.Length > 0)
            {
                var commentWithoutSymbols = commentXml.Without(Constants.Markers.Symbols);

                foreach (var parameter in method.Parameters)
                {
                    var seePhrase = string.Concat("<see cref=\"", parameter.Name, "\"");

                    if (commentWithoutSymbols.Contains(seePhrase, Comparison))
                    {
                        yield return Issue(parameter, seePhrase + Constants.Comments.XmlElementEndingTag);
                    }

                    var seeAlsoPhrase = string.Concat("<seealso cref=\"", parameter.Name, "\"");

                    if (commentWithoutSymbols.Contains(seeAlsoPhrase, Comparison))
                    {
                        yield return Issue(parameter, seeAlsoPhrase + Constants.Comments.XmlElementEndingTag);
                    }
                }
            }
        }
    }
}