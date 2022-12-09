using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2075_ActionFunctionParameterPhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2075";

        internal const string Replacement = "callback";

        internal static readonly string[] Terms = { "action", "Action", "function", "Function", "func", "Func" };

        private static readonly string[] Phrases = GetWithDelimiters(Terms);

        public MiKo_2075_ActionFunctionParameterPhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Phrases, StringComparison.Ordinal, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location, location.GetText(), Replacement);
                }
            }
        }
    }
}