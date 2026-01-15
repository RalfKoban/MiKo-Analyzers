using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2019_CodeFixProvider)), Shared]
    public sealed class MiKo_2019_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] RepresentsCandidates =
                                                                {
                                                                    "Additional ",
                                                                    "Addtional ", // typo
                                                                    "Complex ",
                                                                    "Health ",
                                                                    "Repository ",
                                                                    "Simple ",
                                                                };

        private static readonly string[] RepresentsTheCandidates =
                                                                   {
                                                                       "Absolute ",
                                                                       "High ",
                                                                       "Highest ",
                                                                       "Low ",
                                                                       "Lowest ",
                                                                       "Major ",
                                                                       "Max ",
                                                                       "Maximum ",
                                                                       "Medium ",
                                                                       "Middle ",
                                                                       "Min ",
                                                                       "Minimum ",
                                                                       "Minor ",
                                                                       "Optimal ",
                                                                       "Optimum ",
                                                                   };

        private static readonly string[] CurrentlyUnfixable =
                                                              {
                                                                  "Given ",
                                                                  "When ",
                                                              };

        private static readonly string[] DeterminesCandidates =
                                                                {
                                                                    "Whether",
                                                                    "If",
                                                                };

        private static readonly string[] ConstructorPhrases =
                                                              {
                                                                  "copy constructor",
                                                                  "copy Constructor",
                                                                  "Copy constructor",
                                                                  "Copy Constructor",
                                                                  "copy ctor",
                                                                  "copy Ctor",
                                                                  "Copy ctor",
                                                                  "Copy Ctor",
                                                                  "default constructor",
                                                                  "default Constructor",
                                                                  "Default constructor",
                                                                  "Default Constructor",
                                                                  "default ctor",
                                                                  "default Ctor",
                                                                  "Default ctor",
                                                                  "Default Ctor",
                                                                  "Constructor",
                                                                  "constructor",
                                                                  "Ctor",
                                                                  "ctor",
                                                              };

        private static readonly Pair[] CallbackReplacements = new[]
                                                                  {
                                                                      "A callback that is called",
                                                                      "A callback which is called",
                                                                      "A method that gets called",
                                                                      "A method that is called",
                                                                      "A method which gets called",
                                                                      "A method which is called",
                                                                      "Callback that is called",
                                                                      "Callback which is called",
                                                                      "Method that gets called",
                                                                      "Method that is called",
                                                                      "Method which gets called",
                                                                      "Method which is called",
                                                                      "The callback that is called",
                                                                      "The callback which is called",
                                                                      "The method gets called",
                                                                      "The method is called",
                                                                      "The method that gets called",
                                                                      "The method that is called",
                                                                      "The method which gets called",
                                                                      "The method which is called",
                                                                      "This method gets called",
                                                                      "This method is called",
                                                                  }.ToArray(_ => new Pair(_, "Gets called"));

        private static readonly string[] CallbackPhrases = GetTermsForQuickLookup(CallbackReplacements);

        private static readonly Pair[] CallbackReplacementsWithLy =
                                                                    {
                                                                        new Pair(Constants.Comments.AsynchronouslyStartingPhrase + "called", Constants.Comments.AsynchronouslyStartingPhrase + "runs"),
                                                                        new Pair(Constants.Comments.AsynchronouslyStartingPhrase + "invoked", Constants.Comments.AsynchronouslyStartingPhrase + "runs"),

                                                                        new Pair(Constants.Comments.RecursivelyStartingPhrase + "called", Constants.Comments.RecursivelyStartingPhrase + "runs"),
                                                                        new Pair(Constants.Comments.RecursivelyStartingPhrase + "invoked", Constants.Comments.RecursivelyStartingPhrase + "runs"),
                                                                    };

        private static readonly string[] CallbackPhrasesWithLy = GetTermsForQuickLookup(CallbackReplacementsWithLy);

        public override string FixableDiagnosticId => "MiKo_2019";

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax summary)
            {
                if (summary.FirstAncestor<MemberDeclarationSyntax>() is ConstructorDeclarationSyntax ctor)
                {
                    return GetUpdatedSyntaxForConstructor(summary, ctor);
                }

                return GetUpdatedSyntax(summary);
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            return GetUpdatedSyntax(syntax);
        }

        private static SyntaxNode GetUpdatedSyntaxForConstructor(XmlElementSyntax summary, ConstructorDeclarationSyntax constructor)
        {
            var content = summary.Content;

            if (content.Count > 0 && content[0] is XmlTextSyntax textSyntax)
            {
                var startText = textSyntax.GetTextTrimmed();

                if (startText.StartsWithAny(ConstructorPhrases))
                {
                    var start = "Initializes a new instance of the ";
                    var continuation = " class";

                    if (constructor.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    {
                        start = "Prevents a default instance of the ";
                        continuation = " class from being created";
                    }

                    var commentEnd = continuation.AsCachedBuilder()
                                                 .Append(startText)
                                                 .Without(ConstructorPhrases)
                                                 .Append(' ')
                                                 .Replace(" for ", " with ")
                                                 .TrimmedEnd()
                                                 .ToStringAndRelease();

                    var seeCref = constructor.Parent is TypeDeclarationSyntax type
                                  ? SeeCref(type.AsTypeSyntax())
                                  : SeeCref(constructor.Identifier.ValueText);

                    return Comment(summary, start, seeCref, commentEnd);
                }
            }

            return GetUpdatedSyntax(summary);
        }

        private static SyntaxNode GetUpdatedSyntax(XmlElementSyntax summary)
        {
            var content = summary.Content;

            if (content.Count > 0 && content[0] is XmlTextSyntax textSyntax)
            {
                var startText = textSyntax.GetTextTrimmed();

                if (startText.StartsWithAny(CurrentlyUnfixable))
                {
                    // currently we cannot fix that
                    return summary;
                }

                var text = startText.AsSpan();
                var firstWord = text.FirstWord();

                if (startText.StartsWithAny(RepresentsCandidates))
                {
                    var article = ArticleProvider.GetArticleFor(firstWord, FirstWordAdjustment.StartLowerCase);

                    return CommentStartingWith(summary, "Represents " + article);
                }

                if (startText.StartsWithAny(RepresentsTheCandidates))
                {
                    return CommentStartingWith(summary, "Represents the ");
                }

                if (startText.StartsWithAny(CallbackPhrases))
                {
                    return Comment(summary, CallbackPhrases, CallbackReplacements);
                }

                var updatedSyntax = MiKo_2012_CodeFixProvider.GetUpdatedSyntax(summary);

                if (ReferenceEquals(summary, updatedSyntax) is false)
                {
                    return updatedSyntax;
                }

                if (text.StartsWith(Constants.Comments.AsynchronouslyStartingPhrase) || text.StartsWith(Constants.Comments.RecursivelyStartingPhrase))
                {
                    var updatedSummary = Comment(summary, CallbackPhrasesWithLy, CallbackReplacementsWithLy);

                    if (ReferenceEquals(summary, updatedSummary) is false)
                    {
                        return updatedSummary;
                    }

                    firstWord = text.SecondWord();
                }

                // only adjust in case there is no single letter
                if (firstWord.Length > 1)
                {
                    var index = text.IndexOf(firstWord);
                    var remainingText = text.Slice(index + firstWord.Length);

                    var firstWordUpper = firstWord.ToUpperCaseAt(0);

                    var replacementForFirstWord = firstWordUpper.EqualsAny(DeterminesCandidates)
                                                  ? "Determines whether"
                                                  : Verbalizer.MakeThirdPersonSingularVerb(firstWordUpper);

                    var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText);

                    return Comment(summary, replacedText, content.RemoveAt(0));
                }
            }

            // currently we cannot fix that
            return summary;
        }
    }
}