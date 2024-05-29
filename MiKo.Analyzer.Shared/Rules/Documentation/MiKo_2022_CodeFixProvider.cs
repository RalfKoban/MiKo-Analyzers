using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2022_CodeFixProvider)), Shared]
    public sealed class MiKo_2022_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] ReplacementMapKeys = CreatePhrases().Distinct().ToArray();

        private static readonly KeyValuePair<string, string>[] ReplacementMap = ReplacementMapKeys.Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                                                                  .ToArray();

        public override string FixableDiagnosticId => "MiKo_2022";

        protected override string Title => Resources.MiKo_2022_CodeFixTitle;

//// ncrunch: rdi default

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue)
        {
            var preparedComment = PrepareComment(comment);
            var phrase = GetStartingPhraseProposal(issue);

            var contents = preparedComment.Content;

            switch (contents.Count)
            {
                case 0:
                    return CommentStartingWith(preparedComment, phrase + Constants.TODO + ".");

                case 1 when contents[0] is XmlTextSyntax text && text.GetTextWithoutTrivia().IsEmpty:
                    return comment.ReplaceNode(text, XmlText(phrase + Constants.TODO + ".").WithLeadingXmlComment().WithTrailingXmlComment());

                default:
                    return CommentStartingWith(preparedComment, phrase);
            }
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);

//// ncrunch: rdi off

        private static IEnumerable<string> CreatePhrases()
        {
            yield return "A Flag indicating ";
            yield return "A Flag that indicates ";
            yield return "A Flag which indicates ";
            yield return "A flag indicating ";
            yield return "A flag that indicates ";
            yield return "A flag which indicates ";
            yield return "A value indicating ";
            yield return "A value that indicates ";
            yield return "A value which indicates ";
            yield return "A variable that receives ";
            yield return "A variable which receives ";
            yield return "After return contains a flag indicating ";
            yield return "After return contains a flag that indicates ";
            yield return "After return contains a flag which indicates ";
            yield return "After return contains a value indicating ";
            yield return "After return contains a value that indicates ";
            yield return "After return contains a value which indicates ";
            yield return "After return provides a flag indicating ";
            yield return "After return provides a flag that indicates ";
            yield return "After return provides a flag which indicates ";
            yield return "After return provides a value indicating ";
            yield return "After return provides a value that indicates ";
            yield return "After return provides a value which indicates ";
            yield return "After return receives a flag indicating ";
            yield return "After return receives a flag that indicates ";
            yield return "After return receives a flag which indicates ";
            yield return "After return receives a value indicating ";
            yield return "After return receives a value that indicates ";
            yield return "After return receives a value which indicates ";
            yield return "After return, contains a flag indicating ";
            yield return "After return, contains a flag that indicates ";
            yield return "After return, contains a flag which indicates ";
            yield return "After return, contains a value indicating ";
            yield return "After return, contains a value that indicates ";
            yield return "After return, contains a value which indicates ";
            yield return "After return, provides a flag indicating ";
            yield return "After return, provides a flag that indicates ";
            yield return "After return, provides a flag which indicates ";
            yield return "After return, provides a value indicating ";
            yield return "After return, provides a value that indicates ";
            yield return "After return, provides a value which indicates ";
            yield return "After return, receives a flag indicating ";
            yield return "After return, receives a flag that indicates ";
            yield return "After return, receives a flag which indicates ";
            yield return "After return, receives a value indicating ";
            yield return "After return, receives a value that indicates ";
            yield return "After return, receives a value which indicates ";
            yield return "After successful return contains a flag indicating ";
            yield return "After successful return contains a flag that indicates ";
            yield return "After successful return contains a flag which indicates ";
            yield return "After successful return contains a value indicating ";
            yield return "After successful return contains a value that indicates ";
            yield return "After successful return contains a value which indicates ";
            yield return "After successful return provides a flag indicating ";
            yield return "After successful return provides a flag that indicates ";
            yield return "After successful return provides a flag which indicates ";
            yield return "After successful return provides a value indicating ";
            yield return "After successful return provides a value that indicates ";
            yield return "After successful return provides a value which indicates ";
            yield return "After successful return receives a flag indicating ";
            yield return "After successful return receives a flag that indicates ";
            yield return "After successful return receives a flag which indicates ";
            yield return "After successful return receives a value indicating ";
            yield return "After successful return receives a value that indicates ";
            yield return "After successful return receives a value which indicates ";
            yield return "After successful return, contains a flag indicating ";
            yield return "After successful return, contains a flag that indicates ";
            yield return "After successful return, contains a flag which indicates ";
            yield return "After successful return, contains a value indicating ";
            yield return "After successful return, contains a value that indicates ";
            yield return "After successful return, contains a value which indicates ";
            yield return "After successful return, provides a flag indicating ";
            yield return "After successful return, provides a flag that indicates ";
            yield return "After successful return, provides a flag which indicates ";
            yield return "After successful return, provides a value indicating ";
            yield return "After successful return, provides a value that indicates ";
            yield return "After successful return, provides a value which indicates ";
            yield return "After successful return, receives a flag indicating ";
            yield return "After successful return, receives a flag that indicates ";
            yield return "After successful return, receives a flag which indicates ";
            yield return "After successful return, receives a value indicating ";
            yield return "After successful return, receives a value that indicates ";
            yield return "After successful return, receives a value which indicates ";
            yield return "After return contains ";
            yield return "After return indicates ";
            yield return "After return provides ";
            yield return "After return receives ";
            yield return "After return, contains ";
            yield return "After return, indicates ";
            yield return "After return, provides ";
            yield return "After return, receives ";
            yield return "After successful return contains ";
            yield return "After successful return indicates ";
            yield return "After successful return provides ";
            yield return "After successful return receives ";
            yield return "After successful return, contains ";
            yield return "After successful return, indicates ";
            yield return "After successful return, provides ";
            yield return "After successful return, receives ";
            yield return "Contains ";
            yield return "Flag indicating ";
            yield return "Flag that indicates ";
            yield return "Flag which indicates ";
            yield return "Indicates ";
            yield return "Indicating ";
            yield return "OUT - ";
            yield return "OUT -";
            yield return "OUT ";
            yield return "OUT: ";
            yield return "On return contains a flag indicating ";
            yield return "On return contains a flag that indicates ";
            yield return "On return contains a flag which indicates ";
            yield return "On return contains a value indicating ";
            yield return "On return contains a value that indicates ";
            yield return "On return contains a value which indicates ";
            yield return "On return provides a flag indicating ";
            yield return "On return provides a flag that indicates ";
            yield return "On return provides a flag which indicates ";
            yield return "On return provides a value indicating ";
            yield return "On return provides a value that indicates ";
            yield return "On return provides a value which indicates ";
            yield return "On return receives a flag indicating ";
            yield return "On return receives a flag that indicates ";
            yield return "On return receives a flag which indicates ";
            yield return "On return receives a value indicating ";
            yield return "On return receives a value that indicates ";
            yield return "On return receives a value which indicates ";
            yield return "On return contains ";
            yield return "On return indicates ";
            yield return "On return provides ";
            yield return "On return receives ";
            yield return "On return, contains a flag indicating ";
            yield return "On return, contains a flag that indicates ";
            yield return "On return, contains a flag which indicates ";
            yield return "On return, contains a value indicating ";
            yield return "On return, contains a value that indicates ";
            yield return "On return, contains a value which indicates ";
            yield return "On return, provides a flag indicating ";
            yield return "On return, provides a flag that indicates ";
            yield return "On return, provides a flag which indicates ";
            yield return "On return, provides a value indicating ";
            yield return "On return, provides a value that indicates ";
            yield return "On return, provides a value which indicates ";
            yield return "On return, receives a flag indicating ";
            yield return "On return, receives a flag that indicates ";
            yield return "On return, receives a flag which indicates ";
            yield return "On return, receives a value indicating ";
            yield return "On return, receives a value that indicates ";
            yield return "On return, receives a value which indicates ";
            yield return "On return, contains ";
            yield return "On return, indicates ";
            yield return "On return, provides ";
            yield return "On return, receives ";
            yield return "Out parameter that contains ";
            yield return "Out parameter that provides ";
            yield return "Out parameter that receives ";
            yield return "Out parameter that returns ";
            yield return "Out parameter which contains ";
            yield return "Out parameter which provides ";
            yield return "Out parameter which receives ";
            yield return "Out parameter which returns ";
            yield return "Out parameter, contains ";
            yield return "Out parameter, provides ";
            yield return "Out parameter, receives ";
            yield return "Out parameter, returns ";
            yield return "Out - ";
            yield return "Out -";
            yield return "Out ";
            yield return "Out: ";
            yield return "Provides ";
            yield return "Receives ";
            yield return "Return ";
            yield return "Returned on ";
            yield return "Returned when ";
            yield return "Returns ";
            yield return "Specifies ";
            yield return "To return ";
            yield return "Value indicating ";
            yield return "Value that indicates ";
            yield return "Value which indicates ";
            yield return "When the method returns ";
            yield return "When the method returns, contains ";
            yield return "When the method returns, indicates ";
            yield return "When the method returns, provides ";
            yield return "When the method returns, receives ";
            yield return "When the method returns, ";
            yield return "When this method returns ";
            yield return "When this method returns, contains ";
            yield return "When this method returns, indicates ";
            yield return "When this method returns, provides ";
            yield return "When this method returns, receives ";
            yield return "When this method returns, ";
            yield return "Will be ";
            yield return "Will contain ";
            yield return "Will provide ";
            yield return "Will receive ";
            yield return "Will return ";
            yield return "[OUT] parameter that contains ";
            yield return "[OUT] parameter that provides ";
            yield return "[OUT] parameter that receives ";
            yield return "[OUT] parameter that returns ";
            yield return "[Out] parameter that contains ";
            yield return "[Out] parameter that provides ";
            yield return "[Out] parameter that receives ";
            yield return "[Out] parameter that returns ";
            yield return "[out] parameter that contains ";
            yield return "[out] parameter that provides ";
            yield return "[out] parameter that receives ";
            yield return "[out] parameter that returns ";
            yield return "[OUT] parameter which contains ";
            yield return "[OUT] parameter which provides ";
            yield return "[OUT] parameter which receives ";
            yield return "[OUT] parameter which returns ";
            yield return "[Out] parameter which contains ";
            yield return "[Out] parameter which provides ";
            yield return "[Out] parameter which receives ";
            yield return "[Out] parameter which returns ";
            yield return "[out] parameter which contains ";
            yield return "[out] parameter which provides ";
            yield return "[out] parameter which receives ";
            yield return "[out] parameter which returns ";
            yield return "[OUT] parameter, contains ";
            yield return "[OUT] parameter, provides ";
            yield return "[OUT] parameter, receives ";
            yield return "[OUT] parameter, returns ";
            yield return "[Out] parameter, contains ";
            yield return "[Out] parameter, provides ";
            yield return "[Out] parameter, receives ";
            yield return "[Out] parameter, returns ";
            yield return "[out] parameter, contains ";
            yield return "[out] parameter, provides ";
            yield return "[out] parameter, receives ";
            yield return "[out] parameter, returns ";
            yield return "[OUT] ";
            yield return "[Out] ";
            yield return "[out] ";
            yield return "[OUT]: ";
            yield return "[Out]: ";
            yield return "[out]: ";
            yield return "flag indicating ";
            yield return "flag that indicates ";
            yield return "flag which indicates ";
            yield return "out parameter that contains ";
            yield return "out parameter that provides ";
            yield return "out parameter that receives ";
            yield return "out parameter that returns ";
            yield return "out parameter which contains ";
            yield return "out parameter which provides ";
            yield return "out parameter which receives ";
            yield return "out parameter which returns ";
            yield return "out parameter, contains ";
            yield return "out parameter, provides ";
            yield return "out parameter, receives ";
            yield return "out parameter, returns ";
            yield return "out - ";
            yield return "out -";
            yield return "out ";
            yield return "out: ";
            yield return "value indicating ";
            yield return "value that indicates ";
            yield return "value which indicates ";
            yield return "will contain ";
            yield return "will provide ";
            yield return "will receive ";
            yield return "will return ";
            yield return "return ";
            yield return "returns ";
        }

        //// ncrunch: rdi default
    }
}