﻿using System;
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

        private static readonly string[] ProblematicWords = { "n't", " not ", "cannot", "Cannot", " cant ", " wont ", "ouldnt" };

        public MiKo_2228_DocumentationIsNotNegativeAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var summaryXmls = comment.GetSummaryXmls();
            var remarksXmls = comment.GetRemarksXmls();

            if (summaryXmls.Count == 0 && remarksXmls.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return Array.Empty<Diagnostic>()
                        .Concat(AnalyzeXml(summaryXmls))
                        .Concat(AnalyzeXml(remarksXmls))
                        .ToList();
        }

        private static bool SentenceHasIssue(XmlElementSyntax xml)
        {
            var text = xml.GetTextTrimmed();

            foreach (var sentence in text.AsSpan().SplitBy(Constants.SentenceMarkers))
            {
                var indices = new HashSet<int>();

                foreach (var problematicWord in ProblematicWords)
                {
                    indices.AddRange(sentence.Text.AllIndicesOf(problematicWord, StringComparison.Ordinal));

                    if (indices.Count > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerable<Diagnostic> AnalyzeXml(IReadOnlyList<XmlElementSyntax> xmls)
        {
            var count = xmls.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    var xml = xmls[index];

                    if (SentenceHasIssue(xml))
                    {
                        yield return Issue(xml.StartTag);
                    }
                }
            }
        }
    }
}