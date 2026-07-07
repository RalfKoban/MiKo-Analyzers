using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

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
                                                                  "Copy C-tor",
                                                                  "Copy Constructor",
                                                                  "Copy Ctor",
                                                                  "Copy D-tor", // typo
                                                                  "Copy c-tor",
                                                                  "Copy constructor",
                                                                  "Copy ctor",
                                                                  "Copy d-tor", // typo
                                                                  "Default C-tor",
                                                                  "Default Constructor",
                                                                  "Default Ctor",
                                                                  "Default D-tor", // typo
                                                                  "Default Dtor", // typo
                                                                  "Default c-tor",
                                                                  "Default constructor",
                                                                  "Default ctor",
                                                                  "Default d-tor", // typo
                                                                  "Default dtor", // typo
                                                                  "copy C-tor",
                                                                  "copy Constructor",
                                                                  "copy Ctor",
                                                                  "copy D-tor", // typo
                                                                  "copy c-tor",
                                                                  "copy constructor",
                                                                  "copy ctor",
                                                                  "copy d-tor", // typo
                                                                  "default C-tor",
                                                                  "default Constructor",
                                                                  "default Ctor",
                                                                  "default D-tor", // typo
                                                                  "default Dtor", // typo
                                                                  "default c-tor",
                                                                  "default constructor",
                                                                  "default ctor",
                                                                  "default d-tor", // typo
                                                                  "default dtor", // typo
                                                                  "Constructor",
                                                                  "constructor",
                                                                  "Ctor",
                                                                  "ctor",
                                                                  "Dtor", // typo
                                                                  "dtor", // typo
                                                                  "C-tor",
                                                                  "c-tor",
                                                                  "D-tor", // typo
                                                                  "d-tor", // typo
                                                              };

        private static readonly ReplacementMap CallbackReplacements = new ReplacementMap(
                                                                                     "MiKo_2019_Replace",
                                                                                     new[]
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
                                                                                         }.ToArray(_ => new Pair(_, "Gets called")),
                                                                                     _ => GetTermsForQuickLookup(_));

        private static readonly ReplacementMap CallbackReplacementsWithLy = new ReplacementMap(
                                                                                           "MiKo_2019_Ly",
                                                                                           new[]
                                                                                               {
                                                                                                   new Pair(Constants.Comments.AsynchronouslyStartingPhrase + "called", Constants.Comments.AsynchronouslyStartingPhrase + "runs"),
                                                                                                   new Pair(Constants.Comments.AsynchronouslyStartingPhrase + "invoked", Constants.Comments.AsynchronouslyStartingPhrase + "runs"),
                                                                                                   new Pair(Constants.Comments.RecursivelyStartingPhrase + "called", Constants.Comments.RecursivelyStartingPhrase + "runs"),
                                                                                                   new Pair(Constants.Comments.RecursivelyStartingPhrase + "invoked", Constants.Comments.RecursivelyStartingPhrase + "runs"),
                                                                                               },
                                                                                           _ => GetTermsForQuickLookup(_));

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

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
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
                                                 .Append(Constants.Space)
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
                    // currently we cannot fix that, except for properties
                    if (summary.GetEnclosing<PropertyDeclarationSyntax>() is null)
                    {
                        return summary;
                    }
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

                if (startText.StartsWithAny(CallbackReplacements.Keys))
                {
                    return Comment(summary, CallbackReplacements);
                }

                var updatedSyntax = MiKo_2012_CodeFixProvider.GetUpdatedSyntax(summary);

                if (ReferenceEquals(summary, updatedSyntax) is false)
                {
                    return updatedSyntax;
                }

                if (text.StartsWith(Constants.Comments.AsynchronouslyStartingPhrase) || text.StartsWith(Constants.Comments.RecursivelyStartingPhrase))
                {
                    var updatedSummary = Comment(summary, CallbackReplacementsWithLy);

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