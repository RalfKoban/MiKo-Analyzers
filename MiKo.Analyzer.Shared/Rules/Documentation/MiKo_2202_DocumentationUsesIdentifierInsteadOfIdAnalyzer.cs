using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2202";

        internal const string Term = "id";

        internal static readonly string[] Terms = GetWithDelimiters(Term);

        public MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, comment);

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.DescendantNodes<XmlElementSyntax>(_ => CodeTags.Contains(_.GetName()) is false)
                                         .SelectMany(_ => _.ChildNodes<XmlTextSyntax>())
                                         .SelectMany(_ => _.TextTokens))
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Terms, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}