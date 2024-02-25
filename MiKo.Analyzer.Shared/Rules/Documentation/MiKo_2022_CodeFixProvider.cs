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
                                                                                    { "To return ", string.Empty },
                                                                                    { "[out] parameter that returns ", string.Empty },
                                                                                    { "[Out] parameter that returns ", string.Empty },
                                                                                    { "[OUT] parameter that returns ", string.Empty },
                                                                                    { "[out] ", string.Empty },
                                                                                    { "[Out] ", string.Empty },
                                                                                    { "[OUT] ", string.Empty },
                                                                                    { "[out]: ", string.Empty },
                                                                                    { "[Out]: ", string.Empty },
                                                                                    { "[OUT]: ", string.Empty },
                                                                                    { "Contains ", string.Empty },
                                                                                    { "flag indicating ", string.Empty },
                                                                                    { "Flag indicating ", string.Empty },
                                                                                    { "flag that indicates ", string.Empty },
                                                                                    { "Flag that indicates ", string.Empty },
                                                                                    { "flag which indicates ", string.Empty },
                                                                                    { "Flag which indicates ", string.Empty },
                                                                                    { "Indicates ", string.Empty },
                                                                                    { "Indicating ", string.Empty },
                                                                                    { "On return, contains ", string.Empty },
                                                                                    { "On return contains ", string.Empty },
                                                                                    { "out - ", string.Empty },
                                                                                    { "out -", string.Empty },
                                                                                    { "Out - ", string.Empty },
                                                                                    { "Out -", string.Empty },
                                                                                    { "OUT - ", string.Empty },
                                                                                    { "OUT -", string.Empty },
                                                                                    { "out: ", string.Empty },
                                                                                    { "Out: ", string.Empty },
                                                                                    { "OUT: ", string.Empty },
                                                                                    { "out parameter that returns ", string.Empty },
                                                                                    { "Out parameter that returns ", string.Empty },
                                                                                    { "OUT ", string.Empty },
                                                                                    { "out ", string.Empty },
                                                                                    { "Provides ", string.Empty },
                                                                                    { "Returned on ", string.Empty },
                                                                                    { "Returned when ", string.Empty },
                                                                                    { "Return ", string.Empty },
                                                                                    { "Returns ", string.Empty },
                                                                                    { "return ", string.Empty },
                                                                                    { "returns ", string.Empty },
                                                                                    { "Specifies ", string.Empty },
                                                                                    { "Will be ", string.Empty },
                                                                                    { "Will contain ", string.Empty },
                                                                                    { "When the method returns ", string.Empty },
                                                                                    { "When the method returns, contains ", string.Empty },
                                                                                    { "When the method returns, indicates ", string.Empty },
                                                                                    { "When the method returns, ", string.Empty },
                                                                                    { "When this method returns ", string.Empty },
                                                                                    { "When this method returns, contains ", string.Empty },
                                                                                    { "When this method returns, indicates ", string.Empty },
                                                                                    { "When this method returns, ", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => "MiKo_2022";

        protected override string Title => Resources.MiKo_2022_CodeFixTitle;

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