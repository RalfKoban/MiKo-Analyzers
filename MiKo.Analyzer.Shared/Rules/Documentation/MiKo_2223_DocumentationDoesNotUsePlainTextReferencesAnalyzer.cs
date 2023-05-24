using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2223";

        private static readonly HashSet<string> IgnoreTags = new HashSet<string>
                                                                 {
                                                                     Constants.XmlTag.Code,
                                                                     Constants.XmlTag.C,
                                                                     Constants.XmlTag.See,
                                                                     Constants.XmlTag.SeeAlso,
                                                                 };

        public MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(comment);

        private static bool TryFindCompoundWord(string text, ref int startIndex, ref int endIndex)
        {
            // jump over white-spaces at the beginning
            for (; startIndex < text.Length; startIndex++)
            {
                var c = text[startIndex];

                if (c.IsWhiteSpace() is false)
                {
                    // no white-space, so break
                    break;
                }
            }

            endIndex = startIndex;

            var foundUpperCaseLetters = 0;

            // now find next white-space and count upper cases in between
            for (; endIndex < text.Length; endIndex++)
            {
                var c = text[endIndex];

                if (c.IsUpperCaseLetter())
                {
                    // we found an upper case
                    foundUpperCaseLetters++;

                    continue;
                }

                if (c.IsWhiteSpace())
                {
                    // we found another white-space, so we are finished with the current word, so break
                    break;
                }
            }

            // we found an compound word
            return foundUpperCaseLetters > 1;
        }

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens(_ => IgnoreTags.Contains(_.GetName()) is false))
            {
                var text = token.ValueText;

                if (text.Length < 3)
                {
                    // ignore small texts as they do not contain compound words
                    continue;
                }

                foreach (var diagnostic in AnalyzeComment(token, text))
                {
                    yield return diagnostic;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeComment(SyntaxToken token, string text)
        {
            var textLength = text.Length - 1;

            var start = 0;
            var end = -1;

            // loop over complete text
            while (end < textLength)
            {
                if (TryFindCompoundWord(text, ref start, ref end))
                {
                    // we found a compound word, so report that
                    var location = CreateLocation(token, token.SpanStart + start, token.SpanStart + end);

                    yield return Issue(location);
                }

                // jump over word
                start = end;
            }
        }
    }
}