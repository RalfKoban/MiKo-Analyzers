using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2012_CodeFixProvider)), Shared]
    public sealed class MiKo_2012_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off
        private static readonly string[] Verbs =
                                                 {
                                                     "Adopt",
                                                     "Allow",
                                                     "Build",
                                                     "Construct",
                                                     "Create",
                                                     "Describe",
                                                     "Detect",
                                                     "Enhance",
                                                     "Extend",
                                                     "Generate",
                                                     "Initialize",
                                                     "Handle",
                                                     "Manipulate",
                                                     "Offer",
                                                     "Perform",
                                                     "Provide",
                                                     "Process",
                                                     "Represent",
                                                     "Store",
                                                     "Wrap",
                                                 };

        private static readonly Dictionary<string, string> ThirdPersonVerbs = Verbs.ToDictionary(_ => _, Verbalizer.MakeThirdPersonSingularVerb);

        private static readonly Dictionary<string, string> GerundVerbs = Verbs.ToDictionary(_ => _, Verbalizer.MakeGerundVerb);

        private static readonly string[] DefaultPhrases =
                                                          {
                                                              "A default impl",
                                                              "A default-impl",
                                                              "A impl ",
                                                              "An impl ",
                                                              "A implementation ",
                                                              "An implementation ",
                                                              "Default impl",
                                                              "Default-impl",
                                                              "Impl ",
                                                              "Implementation ",
                                                              "The default impl",
                                                              "The default-impl",
                                                              "The imp ",
                                                              "The implementation ",
                                                          };

        private static readonly string[] UsedToPhrases =
                                                         {
                                                             "Attribute that ",
                                                             "A class to ",
                                                             "A class used to ",
                                                             "A class that is used to ",
                                                             "A class which is used to ",
                                                             "An class to ",
                                                             "An class used to ",
                                                             "An class that is used to ",
                                                             "An class which is used to ",
                                                             "Class to ",
                                                             "Class used to ",
                                                             "Class that is used to ",
                                                             "Class which is used to ",
                                                             "The class used to ",
                                                             "The class is used to ",
                                                             "The class that is used to ",
                                                             "This class is used to ",
                                                             "A interface to ",
                                                             "An interface to ",
                                                             "A interface used to ",
                                                             "An interface used to ",
                                                             "A interface that is used to ",
                                                             "A interface which is used to ",
                                                             "An interface that is used to ",
                                                             "An interface which is used to ",
                                                             "Interface to ",
                                                             "Interface used to ",
                                                             "Interface that is used to ",
                                                             "Interface which is used to ",
                                                             "The interface used to ",
                                                             "The interface is used to ",
                                                             "The interface that is used to ",
                                                             "The interface which is used to ",
                                                             "This interface is used to ",
                                                             "Used to ",
                                                         };

        private static readonly string[] ComponentPhrases = CreateComponentPhrases().ToArray();

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key.Trim());

        private static readonly ISet<SyntaxKind> Declarations = new HashSet<SyntaxKind>
                                                                    {
                                                                        SyntaxKind.ClassDeclaration,
                                                                        SyntaxKind.StructDeclaration,
                                                                        SyntaxKind.RecordDeclaration,
                                                                        SyntaxKind.InterfaceDeclaration,
                                                                        SyntaxKind.MethodDeclaration,
                                                                        SyntaxKind.PropertyDeclaration,
                                                                        SyntaxKind.EventDeclaration,
                                                                        SyntaxKind.EventFieldDeclaration,
                                                                        SyntaxKind.FieldDeclaration,
                                                                    };
//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2012";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            var content = comment.Content;

            var inheritdoc = GetUpdatedSyntaxWithInheritdoc(content);

            if (inheritdoc != null)
            {
                return inheritdoc;
            }

            if (content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.GetTextWithoutTrivia();

                if (text.StartsWith("Interaction logic for", StringComparison.Ordinal))
                {
                    // seems like this is the default comment for WPF controls
                    return Comment(comment, XmlText("Represents a TODO"));
                }

                if (syntax.GetEnclosing(Declarations) is MemberDeclarationSyntax member)
                {
                    var name = member.GetName();

                    if (name.Contains("Command", StringComparison.OrdinalIgnoreCase))
                    {
                        if (text.StartsWithAny(MiKo_2038_CodeFixProvider.CommandStartingPhrases, StringComparison.OrdinalIgnoreCase))
                        {
                            return MiKo_2038_CodeFixProvider.GetUpdatedSyntax(comment);
                        }
                    }
                    else if (name.Contains("Factory", StringComparison.OrdinalIgnoreCase))
                    {
                        var mappedData = MiKo_2060_CodeFixProvider.MappedData.Value;

                        if (text.StartsWithAny(mappedData.InstancesReplacementMapKeys, StringComparison.OrdinalIgnoreCase)
                         || text.StartsWithAny(mappedData.TypeReplacementMapKeysA, StringComparison.OrdinalIgnoreCase)
                         || text.StartsWithAny(mappedData.TypeReplacementMapKeysCD, StringComparison.OrdinalIgnoreCase)
                         || text.StartsWithAny(mappedData.TypeReplacementMapKeysT, StringComparison.OrdinalIgnoreCase)
                         || text.StartsWithAny(mappedData.TypeReplacementMapKeysOthers, StringComparison.OrdinalIgnoreCase))
                        {
                            return MiKo_2060_CodeFixProvider.GetUpdatedSyntax(comment);
                        }
                    }
                }

                if (text.StartsWithAny(ReplacementMapKeys, StringComparison.Ordinal))
                {
                    return Comment(comment, ReplacementMapKeys, ReplacementMap);
                }

                foreach (var phrase in ComponentPhrases.Concat(UsedToPhrases))
                {
                    if (text.StartsWith(phrase, StringComparison.Ordinal))
                    {
                        var remainingText = text.Slice(phrase.Length);

                        var firstWord = remainingText.FirstWord();
                        var index = remainingText.IndexOf(firstWord);
                        var replacementForFirstWord = Verbalizer.MakeThirdPersonSingularVerb(firstWord.ToString()).ToUpperCaseAt(0);

                        var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText.Slice(index + firstWord.Length));

                        return Comment(comment, replacedText, content.RemoveAt(0));
                    }
                }

                if (text.StartsWithAny(Constants.Comments.AAnThePhraseWithSpaces, StringComparison.Ordinal))
                {
                    var ancestor = syntax.FirstAncestorOrSelf<MemberDeclarationSyntax>();

                    switch (ancestor)
                    {
                        case PropertyDeclarationSyntax property:
                        {
                            var startingPhrase = GetPropertyStartingPhrase(property.AccessorList);

                            return CommentStartingWith(comment, startingPhrase);
                        }

                        case BaseTypeDeclarationSyntax _:
                            return CommentStartingWith(comment, "Represents ");
                    }
                }
            }

            var updatedComment = Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeLowerCase);

            return updatedComment;
        }

        private static XmlEmptyElementSyntax GetUpdatedSyntaxWithInheritdoc(SyntaxList<XmlNodeSyntax> content)
        {
            var inheritdoc = content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Inheritdoc);

            if (inheritdoc != null)
            {
                // special case: its an inherit documentation, so mark it so
                return inheritdoc;
            }

            // maybe it is a documentation that should be an inherit documentation instead
            if (content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.GetTextWithoutTrivia();

                if (text.IsNullOrWhiteSpace() && content.Count > 1 && IsSeeCref(content[1]))
                {
                    return Inheritdoc();
                }

                if (text.StartsWithAny(DefaultPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    if (content.Count > 1 && IsSeeCref(content[1]))
                    {
                        return Inheritdoc(GetSeeCref(content[1]));
                    }

                    return Inheritdoc();
                }
            }

            return null;
        }

        private static string GetPropertyStartingPhrase(AccessorListSyntax accessorList)
        {
            if (accessorList is null)
            {
                return string.Empty;
            }

            var accessors = accessorList.Accessors;

            switch (accessors.Count)
            {
                case 1:
                {
                    switch (accessors[0].Kind())
                    {
                        case SyntaxKind.GetAccessorDeclaration:
                            return "Gets ";

                        case SyntaxKind.SetAccessorDeclaration:
                            return "Sets ";

                        default:
                            return string.Empty;
                    }
                }

                case 2:
                    return "Gets or sets ";

                default:
                    return string.Empty;
            }
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var entries = CreateReplacementMapEntries().ToArray(_ => _.Key, AscendingStringComparer.Default); // sort by first character

            var result = new Dictionary<string, string>(entries.Length);

            foreach (var entry in entries)
            {
                if (result.ContainsKey(entry.Key) is false)
                {
                    result[entry.Key] = entry.Value;
                }
            }

            return result.ToArray(_ => new Pair(_.Key, _.Value));
        }

        private static IEnumerable<Pair> CreateReplacementMapEntries()
        {
            // event arguments
            yield return new Pair("Event argument for ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event argument that is used in the ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event argument that provides information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event argument which is used in the ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event argument which provides information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event arguments for ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event arguments that provide information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new Pair("Event arguments which provide information ", Constants.Comments.EventArgsSummaryStartingPhrase);

            // events
            yield return new Pair("Event is fired ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair("Event that is published ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair("Event that is published, ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair("Event which is published ", Constants.Comments.EventSummaryStartingPhrase);
            yield return new Pair("Event which is published, ", Constants.Comments.EventSummaryStartingPhrase);

            // event handler
            yield return new Pair("EventHandler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase);
            yield return new Pair("EventHandler for ", Constants.Comments.EventHandlerSummaryStartingPhrase);
            yield return new Pair("Event handler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase);
            yield return new Pair("Event handler for ", Constants.Comments.EventHandlerSummaryStartingPhrase);
            yield return new Pair("Handler for the ", Constants.Comments.EventHandlerSummaryStartingPhrase);
            yield return new Pair("Handler for ", Constants.Comments.EventHandlerSummaryStartingPhrase);

            // factories
            yield return new Pair("Factory for ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class building ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class constructing ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class creating ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class to build ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class to construct ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class that builds ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class that constructs ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class that creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class which builds ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class which constructs ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Factory class which creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface for factories creating ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface for factories to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface for factories that create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface for factories which create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface of a factory creating ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface of a factory that creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Interface of a factory which creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Used to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Used for creating ", Constants.Comments.FactorySummaryPhrase);

            yield return new Pair("Class that serves as ", "Represents a ");
            yield return new Pair("Class that serves ", "Provides ");
            yield return new Pair("Class which serves as ", "Represents a ");
            yield return new Pair("Class which serves ", "Provides ");

            // initialize method
            yield return new Pair("Initialize ", "Initializes ");

            yield return new Pair("Contain ", "Provides ");
            yield return new Pair("Contains ", "Provides ");
            yield return new Pair("Every class that implements the interface can ", "Allows to ");
            yield return new Pair("Every class that implements this interface can ", "Allows to ");
            yield return new Pair("Extension of ", "Extends the ");
            yield return new Pair("Interface definition for ", "Represents ");
            yield return new Pair("Interface for a ", "Represents a ");
            yield return new Pair("Interface for an ", "Represents an ");
            yield return new Pair("Interface for the ", "Represents a ");
            yield return new Pair("Interface of ", "Represents ");
            yield return new Pair("Interface that serves ", "Provides ");
            yield return new Pair("Interface which serves ", "Provides ");
            yield return new Pair("The class offers ", "Provides ");
            yield return new Pair("The interface offers ", "Provides ");
            yield return new Pair("This class offers ", "Provides ");
            yield return new Pair("This interface offers ", "Provides ");

            // view models
            yield return new Pair("ViewModel of ", "Represents the view model of ");
            yield return new Pair("View Model of ", "Represents the view model of ");
            yield return new Pair("View model of ", "Represents the view model of ");
            yield return new Pair("ViewModel for ", "Represents the view model of ");
            yield return new Pair("View Model for ", "Represents the view model of ");
            yield return new Pair("View model for ", "Represents the view model of ");
            yield return new Pair("ViewModel representing ", "Represents the view model of ");
            yield return new Pair("View Model representing ", "Represents the view model of ");
            yield return new Pair("View model representing ", "Represents the view model of ");
            yield return new Pair("ViewModel that represents ", "Represents the view model of ");
            yield return new Pair("View Model that represents ", "Represents the view model of ");
            yield return new Pair("View model that represents ", "Represents the view model of ");

            foreach (var phrase in CreatePhrases())
            {
                yield return phrase;
            }
        }

        private static IEnumerable<Pair> CreatePhrases()
        {
            var beginnings = new[]
                                 {
                                     "A class",
                                     "A interface",
                                     "An interface",
                                     "Class",
                                     "Classes implementing the interfaces",
                                     "Classes implementing the interfaces,",
                                     "Extension method",
                                     "Factory method",
                                     "Function",
                                     "Help function",
                                     "Help method",
                                     "Helper class",
                                     "Helper function",
                                     "Helper method",
                                     "Interface definition of helper",
                                     "Interface definition of a helper",
                                     "Interface definition of an helper",
                                     "Interface definition of the helper",
                                     "Interface for classes",
                                     "Interface for elements",
                                     "Interface for items",
                                     "Interface for objects",
                                     "Interface for view models",
                                     "Interface for workflows",
                                     "Interface for work flows",
                                     "Interface for",
                                     "Interface implemented to",
                                     "Interface",
                                     "Method",
                                     "The class implementing the interface",
                                     "The class implementing the interface,",
                                     "The class implementing this interface",
                                     "The class",
                                     "The interface",
                                     "This class",
                                     "This interface class",
                                     "This interface",
                                 };

            var middleParts = new[]
                                  {
                                      "that can",
                                      "that will",
                                      "that",
                                      "which can",
                                      "which will",
                                      "which",
                                      "will",
                                  };

            foreach (var word in Verbs)
            {
                var noun = word.ToUpperCaseAt(0);
                var verb = word.ToLowerCaseAt(0);

                var thirdPersonStart = ThirdPersonVerbs[word];
                var thirdPersonVerb = thirdPersonStart.ToLowerCaseAt(0);
                var gerundVerb = GerundVerbs[word].ToLowerCaseAt(0);

                var fix = thirdPersonStart + " ";

                foreach (var start in beginnings)
                {
                    foreach (var middle in middleParts)
                    {
                        var begin = string.Concat(start, " ", middle, " ");

                        yield return new Pair(string.Concat(begin, verb, " "), fix);
                        yield return new Pair(string.Concat(begin, thirdPersonVerb, " "), fix);
                    }

                    yield return new Pair(string.Concat(start, " to ", noun, " "), fix);
                    yield return new Pair(string.Concat(start, " to ", verb, " "), fix);

                    yield return new Pair(string.Concat(start, " ", verb, " "), fix);
                    yield return new Pair(string.Concat(start, " ", thirdPersonVerb, " "), fix);
                    yield return new Pair(string.Concat(start, " ", gerundVerb, " "), fix);
                }
            }
        }

        private static IEnumerable<string> CreateComponentPhrases()
        {
            var starts = new[] { "A component", "An component", "The component", "This component", "Component" };
            var middles = new[] { string.Empty, "is ", "that is ", "which is " };
            var lasts = new[] { "to", "able to", "capable to", "used to" };

            foreach (var start in starts)
            {
                foreach (var middle in middles)
                {
                    var begin = string.Concat(start, " ", middle);

                    foreach (var last in lasts)
                    {
                        yield return string.Concat(begin, last, " ");
                    }
                }
            }
        }

//// ncrunch: rdi default
    }
}