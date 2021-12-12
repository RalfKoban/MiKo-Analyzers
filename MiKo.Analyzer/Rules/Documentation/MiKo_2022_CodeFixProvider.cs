using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2022_CodeFixProvider)), Shared]
    public sealed class MiKo_2022_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "out parameter that returns ", string.Empty },
                                                                                    { "Out parameter that returns ", string.Empty },
                                                                                    { "[out] parameter that returns ", string.Empty },
                                                                                    { "[Out] parameter that returns ", string.Empty },
                                                                                    { "Returns ", string.Empty },
                                                                                    { "Contains ", string.Empty },
                                                                                    { "Indicates ", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2022_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax Comment(Document document, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic) => comment; // TODO RKN: fix

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var preparedComment = PrepareComment(comment);

            var symbol = (IParameterSymbol)GetSymbol(document, parameter);
            var phrase = MiKo_2022_OutParamDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            return CommentStartingWith(preparedComment, phrase);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Keys, ReplacementMap);
    }
}