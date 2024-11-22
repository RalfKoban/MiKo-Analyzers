﻿using System;
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

        private static readonly string[] ReplacementMapKeys = CreatePhrases().ToArray();

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.ToArray(_ => new Pair(_));

        public override string FixableDiagnosticId => "MiKo_2022";

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

                case 1 when contents[0] is XmlTextSyntax text && text.GetTextWithoutTrivia().IsNullOrEmpty():
                    return comment.ReplaceNode(text, XmlText(phrase + Constants.TODO + ".").WithLeadingXmlComment().WithTrailingXmlComment());

                default:
                    return CommentStartingWith(preparedComment, phrase);
            }
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);

//// ncrunch: rdi off

        private static HashSet<string> CreatePhrases() => new HashSet<string>
                                                              {
                                                                  "A Flag indicating ",
                                                                  "A Flag that indicates ",
                                                                  "A Flag which indicates ",
                                                                  "A flag indicating ",
                                                                  "A flag that indicates ",
                                                                  "A flag which indicates ",
                                                                  "A value indicating ",
                                                                  "A value that indicates ",
                                                                  "A value which indicates ",
                                                                  "A variable that receives ",
                                                                  "A variable which receives ",
                                                                  "After return contains a flag indicating ",
                                                                  "After return contains a flag that indicates ",
                                                                  "After return contains a flag which indicates ",
                                                                  "After return contains a value indicating ",
                                                                  "After return contains a value that indicates ",
                                                                  "After return contains a value which indicates ",
                                                                  "After return provides a flag indicating ",
                                                                  "After return provides a flag that indicates ",
                                                                  "After return provides a flag which indicates ",
                                                                  "After return provides a value indicating ",
                                                                  "After return provides a value that indicates ",
                                                                  "After return provides a value which indicates ",
                                                                  "After return receives a flag indicating ",
                                                                  "After return receives a flag that indicates ",
                                                                  "After return receives a flag which indicates ",
                                                                  "After return receives a value indicating ",
                                                                  "After return receives a value that indicates ",
                                                                  "After return receives a value which indicates ",
                                                                  "After return, contains a flag indicating ",
                                                                  "After return, contains a flag that indicates ",
                                                                  "After return, contains a flag which indicates ",
                                                                  "After return, contains a value indicating ",
                                                                  "After return, contains a value that indicates ",
                                                                  "After return, contains a value which indicates ",
                                                                  "After return, provides a flag indicating ",
                                                                  "After return, provides a flag that indicates ",
                                                                  "After return, provides a flag which indicates ",
                                                                  "After return, provides a value indicating ",
                                                                  "After return, provides a value that indicates ",
                                                                  "After return, provides a value which indicates ",
                                                                  "After return, receives a flag indicating ",
                                                                  "After return, receives a flag that indicates ",
                                                                  "After return, receives a flag which indicates ",
                                                                  "After return, receives a value indicating ",
                                                                  "After return, receives a value that indicates ",
                                                                  "After return, receives a value which indicates ",
                                                                  "After successful return contains a flag indicating ",
                                                                  "After successful return contains a flag that indicates ",
                                                                  "After successful return contains a flag which indicates ",
                                                                  "After successful return contains a value indicating ",
                                                                  "After successful return contains a value that indicates ",
                                                                  "After successful return contains a value which indicates ",
                                                                  "After successful return provides a flag indicating ",
                                                                  "After successful return provides a flag that indicates ",
                                                                  "After successful return provides a flag which indicates ",
                                                                  "After successful return provides a value indicating ",
                                                                  "After successful return provides a value that indicates ",
                                                                  "After successful return provides a value which indicates ",
                                                                  "After successful return receives a flag indicating ",
                                                                  "After successful return receives a flag that indicates ",
                                                                  "After successful return receives a flag which indicates ",
                                                                  "After successful return receives a value indicating ",
                                                                  "After successful return receives a value that indicates ",
                                                                  "After successful return receives a value which indicates ",
                                                                  "After successful return, contains a flag indicating ",
                                                                  "After successful return, contains a flag that indicates ",
                                                                  "After successful return, contains a flag which indicates ",
                                                                  "After successful return, contains a value indicating ",
                                                                  "After successful return, contains a value that indicates ",
                                                                  "After successful return, contains a value which indicates ",
                                                                  "After successful return, provides a flag indicating ",
                                                                  "After successful return, provides a flag that indicates ",
                                                                  "After successful return, provides a flag which indicates ",
                                                                  "After successful return, provides a value indicating ",
                                                                  "After successful return, provides a value that indicates ",
                                                                  "After successful return, provides a value which indicates ",
                                                                  "After successful return, receives a flag indicating ",
                                                                  "After successful return, receives a flag that indicates ",
                                                                  "After successful return, receives a flag which indicates ",
                                                                  "After successful return, receives a value indicating ",
                                                                  "After successful return, receives a value that indicates ",
                                                                  "After successful return, receives a value which indicates ",
                                                                  "After return contains ",
                                                                  "After return indicates ",
                                                                  "After return provides ",
                                                                  "After return receives ",
                                                                  "After return, contains ",
                                                                  "After return, indicates ",
                                                                  "After return, provides ",
                                                                  "After return, receives ",
                                                                  "After successful return contains ",
                                                                  "After successful return indicates ",
                                                                  "After successful return provides ",
                                                                  "After successful return receives ",
                                                                  "After successful return, contains ",
                                                                  "After successful return, indicates ",
                                                                  "After successful return, provides ",
                                                                  "After successful return, receives ",
                                                                  "Contains ",
                                                                  "Flag indicating ",
                                                                  "Flag that indicates ",
                                                                  "Flag which indicates ",
                                                                  "Indicates ",
                                                                  "Indicating ",
                                                                  "OUT - ",
                                                                  "OUT -",
                                                                  "OUT ",
                                                                  "OUT: ",
                                                                  "On return contains a flag indicating ",
                                                                  "On return contains a flag that indicates ",
                                                                  "On return contains a flag which indicates ",
                                                                  "On return contains a value indicating ",
                                                                  "On return contains a value that indicates ",
                                                                  "On return contains a value which indicates ",
                                                                  "On return provides a flag indicating ",
                                                                  "On return provides a flag that indicates ",
                                                                  "On return provides a flag which indicates ",
                                                                  "On return provides a value indicating ",
                                                                  "On return provides a value that indicates ",
                                                                  "On return provides a value which indicates ",
                                                                  "On return receives a flag indicating ",
                                                                  "On return receives a flag that indicates ",
                                                                  "On return receives a flag which indicates ",
                                                                  "On return receives a value indicating ",
                                                                  "On return receives a value that indicates ",
                                                                  "On return receives a value which indicates ",
                                                                  "On return contains ",
                                                                  "On return indicates ",
                                                                  "On return provides ",
                                                                  "On return receives ",
                                                                  "On return, contains a flag indicating ",
                                                                  "On return, contains a flag that indicates ",
                                                                  "On return, contains a flag which indicates ",
                                                                  "On return, contains a value indicating ",
                                                                  "On return, contains a value that indicates ",
                                                                  "On return, contains a value which indicates ",
                                                                  "On return, provides a flag indicating ",
                                                                  "On return, provides a flag that indicates ",
                                                                  "On return, provides a flag which indicates ",
                                                                  "On return, provides a value indicating ",
                                                                  "On return, provides a value that indicates ",
                                                                  "On return, provides a value which indicates ",
                                                                  "On return, receives a flag indicating ",
                                                                  "On return, receives a flag that indicates ",
                                                                  "On return, receives a flag which indicates ",
                                                                  "On return, receives a value indicating ",
                                                                  "On return, receives a value that indicates ",
                                                                  "On return, receives a value which indicates ",
                                                                  "On return, contains ",
                                                                  "On return, indicates ",
                                                                  "On return, provides ",
                                                                  "On return, receives ",
                                                                  "Out parameter that contains ",
                                                                  "Out parameter that provides ",
                                                                  "Out parameter that receives ",
                                                                  "Out parameter that returns ",
                                                                  "Out parameter which contains ",
                                                                  "Out parameter which provides ",
                                                                  "Out parameter which receives ",
                                                                  "Out parameter which returns ",
                                                                  "Out parameter, contains ",
                                                                  "Out parameter, provides ",
                                                                  "Out parameter, receives ",
                                                                  "Out parameter, returns ",
                                                                  "Out - ",
                                                                  "Out -",
                                                                  "Out ",
                                                                  "Out: ",
                                                                  "Provides ",
                                                                  "Receives ",
                                                                  "Return ",
                                                                  "Returned on ",
                                                                  "Returned when ",
                                                                  "Returns ",
                                                                  "Specifies ",
                                                                  "To return ",
                                                                  "Value indicating ",
                                                                  "Value that indicates ",
                                                                  "Value which indicates ",
                                                                  "When the method returns ",
                                                                  "When the method returns, contains ",
                                                                  "When the method returns, indicates ",
                                                                  "When the method returns, provides ",
                                                                  "When the method returns, receives ",
                                                                  "When the method returns, ",
                                                                  "When this method returns ",
                                                                  "When this method returns, contains ",
                                                                  "When this method returns, indicates ",
                                                                  "When this method returns, provides ",
                                                                  "When this method returns, receives ",
                                                                  "When this method returns, ",
                                                                  "Will be ",
                                                                  "Will contain ",
                                                                  "Will provide ",
                                                                  "Will receive ",
                                                                  "Will return ",
                                                                  "[OUT] parameter that contains ",
                                                                  "[OUT] parameter that provides ",
                                                                  "[OUT] parameter that receives ",
                                                                  "[OUT] parameter that returns ",
                                                                  "[Out] parameter that contains ",
                                                                  "[Out] parameter that provides ",
                                                                  "[Out] parameter that receives ",
                                                                  "[Out] parameter that returns ",
                                                                  "[out] parameter that contains ",
                                                                  "[out] parameter that provides ",
                                                                  "[out] parameter that receives ",
                                                                  "[out] parameter that returns ",
                                                                  "[OUT] parameter which contains ",
                                                                  "[OUT] parameter which provides ",
                                                                  "[OUT] parameter which receives ",
                                                                  "[OUT] parameter which returns ",
                                                                  "[Out] parameter which contains ",
                                                                  "[Out] parameter which provides ",
                                                                  "[Out] parameter which receives ",
                                                                  "[Out] parameter which returns ",
                                                                  "[out] parameter which contains ",
                                                                  "[out] parameter which provides ",
                                                                  "[out] parameter which receives ",
                                                                  "[out] parameter which returns ",
                                                                  "[OUT] parameter, contains ",
                                                                  "[OUT] parameter, provides ",
                                                                  "[OUT] parameter, receives ",
                                                                  "[OUT] parameter, returns ",
                                                                  "[Out] parameter, contains ",
                                                                  "[Out] parameter, provides ",
                                                                  "[Out] parameter, receives ",
                                                                  "[Out] parameter, returns ",
                                                                  "[out] parameter, contains ",
                                                                  "[out] parameter, provides ",
                                                                  "[out] parameter, receives ",
                                                                  "[out] parameter, returns ",
                                                                  "[OUT] ",
                                                                  "[Out] ",
                                                                  "[out] ",
                                                                  "[OUT]: ",
                                                                  "[Out]: ",
                                                                  "[out]: ",
                                                                  "flag indicating ",
                                                                  "flag that indicates ",
                                                                  "flag which indicates ",
                                                                  "out parameter that contains ",
                                                                  "out parameter that provides ",
                                                                  "out parameter that receives ",
                                                                  "out parameter that returns ",
                                                                  "out parameter which contains ",
                                                                  "out parameter which provides ",
                                                                  "out parameter which receives ",
                                                                  "out parameter which returns ",
                                                                  "out parameter, contains ",
                                                                  "out parameter, provides ",
                                                                  "out parameter, receives ",
                                                                  "out parameter, returns ",
                                                                  "out - ",
                                                                  "out -",
                                                                  "out ",
                                                                  "out: ",
                                                                  "value indicating ",
                                                                  "value that indicates ",
                                                                  "value which indicates ",
                                                                  "will contain ",
                                                                  "will provide ",
                                                                  "will receive ",
                                                                  "will return ",
                                                                  "return ",
                                                                  "returns ",
                                                              };

        //// ncrunch: rdi default
    }
}