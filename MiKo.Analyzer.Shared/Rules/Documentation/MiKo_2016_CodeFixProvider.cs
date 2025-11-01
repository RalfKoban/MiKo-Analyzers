using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2016_CodeFixProvider)), Shared]
    public sealed class MiKo_2016_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] ReplacementMapKeys =
                                                              {
                                                                  "A callback that is called ",
                                                                  "A callback which is called ",
                                                                  "A method that gets called ",
                                                                  "A method that is called ",
                                                                  "A method which gets called ",
                                                                  "A method which is called ",
                                                                  "Callback that is called ",
                                                                  "Callback which is called ",
                                                                  "Method that gets called ",
                                                                  "Method that is called ",
                                                                  "Method which gets called ",
                                                                  "Method which is called ",
                                                                  "The callback that is called ",
                                                                  "The callback which is called ",
                                                                  "The method gets called ",
                                                                  "The method is called ",
                                                                  "The method that gets called ",
                                                                  "The method that is called ",
                                                                  "The method which gets called ",
                                                                  "The method which is called ",
                                                                  "This method gets called ",
                                                                  "This method is called ",
                                                              };

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.Select(_ => new Pair(_, Constants.Comments.AsynchronouslyStartingPhrase + "invoked "))
                                                                          .OrderDescendingByLengthAndText(_ => _.Key);

        public override string FixableDiagnosticId => "MiKo_2016";

        protected override string Title => Resources.MiKo_2016_CodeFixTitle.FormatWith(Constants.Comments.AsynchronouslyStartingPhrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var preparedComment = Comment((XmlElementSyntax)syntax, ReplacementMapKeys, ReplacementMap);

            if (ReferenceEquals(syntax, preparedComment))
            {
                return CommentStartingWith(preparedComment, Constants.Comments.AsynchronouslyStartingPhrase, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeThirdPersonSingular);
            }

            return preparedComment;
        }
    }
}