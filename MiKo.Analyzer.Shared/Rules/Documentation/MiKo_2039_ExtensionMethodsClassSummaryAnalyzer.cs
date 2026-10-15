using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2039_ExtensionMethodsClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2039";

        public MiKo_2039_ExtensionMethodsClassSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.ContainsExtensionMethods();

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            return Analyze(comment, Constants.XmlTag.Summary, summaries);
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeContents(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> contentXmls, Lazy<string> commentXml, Lazy<string[]> contents)
        {
            return Analyze(comment, Constants.XmlTag.Content, contents);
        }

        private Diagnostic[] Analyze(DocumentationCommentTriviaSyntax comment, string tag, Lazy<string[]> texts)
        {
            var phrases = Constants.Comments.ExtensionMethodClassStartingPhrase;

            if (texts.Value.None(_ => _.StartsWithAny(phrases)))
            {
                var syntax = comment.GetXmlSyntax(tag)[0];

                return new[] { Issue(syntax, tag, phrases[0]) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}