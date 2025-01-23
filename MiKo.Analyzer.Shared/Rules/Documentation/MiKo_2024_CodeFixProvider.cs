using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2024_CodeFixProvider)), Shared]
    public sealed class MiKo_2024_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly string[] ReplacementMapKeys =
                                                              {
                                                                  "Specifies",
                                                                  "A value specifying",
                                                                  "A value that specifies",
                                                                  "A value which specifies",
                                                                  "An value specifying",
                                                                  "An value that specifies",
                                                                  "An value which specifies",
                                                                  "The value specifying",
                                                                  "The value that specifies",
                                                                  "The value which specifies",
                                                                  "One of the values which specifies",
                                                                  "One of the enumeration members which specifies",
                                                                  "One of the enumeration values which specifies",
                                                                  "Determines",
                                                                  "A value determining",
                                                                  "A value that determines",
                                                                  "A value which determines",
                                                                  "An value determining",
                                                                  "An value that determines",
                                                                  "An value which determines",
                                                                  "The value determining",
                                                                  "The value that determines",
                                                                  "The value which determines",
                                                                  "One of the values which determines",
                                                                  "One of the enumeration members which determines",
                                                                  "One of the enumeration values which determines",
                                                                  "Indicator for",
                                                                  "Value indicating",
                                                                  "Enum for",
                                                                  "Enum that indicates",
                                                                  "Enum which indicates",
                                                                  "Enum indicating",
                                                                  "enum that indicates",
                                                                  "enum which indicates",
                                                                  "enum indicating",
                                                              };

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.Select(_ => new Pair(_)).ToArray();

        public override string FixableDiagnosticId => "MiKo_2024";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue)
        {
            var phrase = GetStartingPhraseProposal(issue);

            var updatedComment = Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeLowerCase);

            return CommentStartingWith(updatedComment, phrase);
        }
    }
}