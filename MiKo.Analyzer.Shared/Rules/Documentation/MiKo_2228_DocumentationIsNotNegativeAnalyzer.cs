using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2228_DocumentationIsNotNegativeAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2228";

        private static readonly string[] ProblematicWords = { " not ", "cannot", "Cannot", "Can't", " cant ", " can't", " wont ", "won't", "Won't", "shouldnt", "shouldn't", "Shouldnt", "Shouldn't" };

        public MiKo_2228_DocumentationIsNotNegativeAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return Enumerable.Empty<Diagnostic>()
                             .Concat(AnalyzeXml(comment.GetSummaryXmls()))
                             .Concat(AnalyzeXml(comment.GetRemarksXmls()));
        }

        private IEnumerable<Diagnostic> AnalyzeXml(IEnumerable<XmlElementSyntax> xmls)
        {
            foreach (var xml in xmls)
            {
                var counts = 0;

                foreach (var token in xml.GetXmlTextTokens())
                {
                    if (counts > 1)
                    {
                        break;
                    }

                    var locations = GetAllLocations(token, ProblematicWords);

                    counts += locations.Count();
                }

                if (counts > 1)
                {
                    yield return Issue(xml.StartTag);
                }
            }
        }
    }
}