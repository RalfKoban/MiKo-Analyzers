using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

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

        private static readonly string[] UsedToPhrases = CreateUsedToPhrases().ToArray();

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key.Trim());

        private static readonly Pair[] EmptyReplacementsMap =
                                                              {
                                                                  new Pair("Called to "),
                                                              };

        private static readonly string[] EmptyReplacementsMapKeys = EmptyReplacementsMap.ToArray(_ => _.Key);

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2012";

        internal static SyntaxNode GetUpdatedSyntax(XmlElementSyntax comment, XmlTextSyntax textSyntax)
        {
            var text = textSyntax.GetTextWithoutTrivia().AsSpan();

            if (text.StartsWith("Interaction logic for", StringComparison.Ordinal))
            {
                // seems like this is the default comment for WPF controls
                return Comment(comment, XmlText("Represents a TODO"));
            }

            if (text.StartsWithAny(EmptyReplacementsMapKeys, StringComparison.Ordinal))
            {
                return Comment(comment, EmptyReplacementsMapKeys, EmptyReplacementsMap, FirstWordHandling.MakeUpperCase | FirstWordHandling.MakeThirdPersonSingular | FirstWordHandling.KeepLeadingSpace);
            }

            if (comment.GetEnclosing(Declarations) is MemberDeclarationSyntax member)
            {
                var name = member.GetName();

                if (name.Contains(Constants.Names.Command, StringComparison.OrdinalIgnoreCase))
                {
                    if (MiKo_2038_CodeFixProvider.CanFix(text))
                    {
                        return MiKo_2038_CodeFixProvider.GetUpdatedSyntax(comment);
                    }
                }
                else if (name.Contains(Constants.Names.Factory, StringComparison.OrdinalIgnoreCase))
                {
                    if (MiKo_2060_CodeFixProvider.CanFix(text))
                    {
                        return MiKo_2060_CodeFixProvider.GetUpdatedSyntax(comment);
                    }
                }
                else if (name.Contains(nameof(EventArgs)))
                {
                    return MiKo_2002_CodeFixProvider.GetUpdatedSyntax(comment);
                }

                if (text.StartsWithAny(ReplacementMapKeys, StringComparison.Ordinal))
                {
                    return Comment(comment, ReplacementMapKeys, ReplacementMap);
                }

                foreach (var phrase in UsedToPhrases)
                {
                    if (text.StartsWith(phrase, StringComparison.Ordinal))
                    {
                        var remainingText = text.Slice(phrase.Length);

                        if (member is PropertyDeclarationSyntax property)
                        {
                            return GetUpdatedProperty(comment, property, remainingText);
                        }

                        var firstWord = remainingText.FirstWord();
                        var index = remainingText.IndexOf(firstWord);
                        var replacementForFirstWord = Verbalizer.MakeThirdPersonSingularVerb(firstWord.ToUpperCaseAt(0));

                        var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText.Slice(index + firstWord.Length));

                        return Comment(comment, replacedText, comment.Content.RemoveAt(0));
                    }
                }

                if (text.StartsWithAny(Constants.Comments.AAnThePhraseWithSpaces, StringComparison.Ordinal))
                {
                    switch (member)
                    {
                        case PropertyDeclarationSyntax property:
                            return GetUpdatedProperty(comment, property, text);

                        case BaseTypeDeclarationSyntax _:
                            return CommentStartingWith(comment, "Represents ");
                    }
                }
            }

            return comment;
        }

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
                return GetUpdatedSyntax(comment, t);
            }

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeLowerCase);
        }

        private static XmlEmptyElementSyntax GetUpdatedSyntaxWithInheritdoc(in SyntaxList<XmlNodeSyntax> content)
        {
            var inheritdoc = content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Inheritdoc);

            if (inheritdoc != null)
            {
                // special case: it's an inherit documentation, so mark it so
                return inheritdoc;
            }

            // maybe it is a documentation that should be an inherit documentation instead
            if (content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.GetTextWithoutTrivia();

                if (text.IsNullOrWhiteSpace() && content.Count > 1 && content[1].IsSeeCref())
                {
                    return Inheritdoc();
                }

                if (text.StartsWithAny(DefaultPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    if (content.Count > 1 && content[1].IsSeeCref())
                    {
                        return Inheritdoc(GetSeeCref(content[1]));
                    }

                    return Inheritdoc();
                }
            }

            return null;
        }

        private static XmlElementSyntax GetUpdatedProperty(XmlElementSyntax comment, PropertyDeclarationSyntax property, in ReadOnlySpan<char> remainingText)
        {
            var startingPhrase = GetPropertyStartingPhrase(property);

            var builder = StringBuilderCache.Acquire(startingPhrase.Length + remainingText.Length)
                                            .Append(startingPhrase)
                                            .Append(remainingText.ToLowerCaseAt(0))
                                            .ReplaceWithProbe("Gets or sets a value indicating get or set ", "Gets or sets a value indicating ")
                                            .ReplaceWithProbe("Gets or sets a value indicating get ", "Gets or sets a value indicating ")
                                            .ReplaceWithProbe("Gets or sets a value indicating set ", "Gets or sets a value indicating ")
                                            .ReplaceWithProbe("Gets or sets get or set ", "Gets or sets ")
                                            .ReplaceWithProbe("Gets or sets get ", "Gets or sets ")
                                            .ReplaceWithProbe("Gets or sets set ", "Gets or sets ")
                                            .ReplaceWithProbe("Gets a value indicating get ", "Gets a value indicating ")
                                            .ReplaceWithProbe("Gets get ", "Gets ")
                                            .ReplaceWithProbe("Sets a value indicating set ", "Sets a value indicating ")
                                            .ReplaceWithProbe("Sets set ", "Sets ")
                                            .ReplaceWithProbe("value indicating the value indicating", "value indicating the");

            var replacedFixedText = builder.ToStringAndRelease();

            return Comment(comment, replacedFixedText, comment.Content.RemoveAt(0));
        }

        private static string GetPropertyStartingPhrase(PropertyDeclarationSyntax property)
        {
            var accessorList = property.AccessorList;

            if (accessorList is null)
            {
                return string.Empty;
            }

            var boolean = property.Type.IsBoolean();

            var accessors = accessorList.Accessors;

            switch (accessors.Count)
            {
                case 1:
                {
                    switch (accessors[0].Kind())
                    {
                        case SyntaxKind.GetAccessorDeclaration:
                            return boolean ? "Gets a value indicating " : "Gets ";

                        case SyntaxKind.SetAccessorDeclaration:
                            return boolean ? "Sets a value indicating " : "Sets ";

                        default:
                            return string.Empty;
                    }
                }

                case 2:
                    return boolean ? "Gets or sets a value indicating " : "Gets or sets ";

                default:
                    return string.Empty;
            }
        }

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var verbs = new[]
                            {
                                "Adopt",
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

            var thirdPersonVerbs = verbs.ToDictionary(_ => _, Verbalizer.MakeThirdPersonSingularVerb);
            var gerundVerbs = verbs.ToDictionary(_ => _, Verbalizer.MakeGerundVerb);

            return CreateReplacementMap(verbs, thirdPersonVerbs, gerundVerbs);
        }

        private static Pair[] CreateReplacementMap(string[] verbs, Dictionary<string, string> thirdPersonVerbs, Dictionary<string, string> gerundVerbs)
        {
            var entries = CreateReplacementMapEntries(verbs, thirdPersonVerbs, gerundVerbs).ToArray(_ => _.Key, AscendingStringComparer.Default); // sort by first character
            var entriesLength = entries.Length;

            var result = new Dictionary<string, string>(entriesLength);

            for (var index = 0; index < entriesLength; index++)
            {
                var entry = entries[index];

                if (result.ContainsKey(entry.Key) is false)
                {
                    result[entry.Key] = entry.Value;
                }
            }

            return result.ToArray(_ => new Pair(_.Key, _.Value));
        }

        private static IEnumerable<Pair> CreateReplacementMapEntries(string[] verbs, Dictionary<string, string> thirdPersonVerbs, Dictionary<string, string> gerundVerbs)
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

            foreach (var phrase in CreatePhrases(verbs, thirdPersonVerbs, gerundVerbs))
            {
                yield return phrase;
            }
        }

        private static IEnumerable<Pair> CreatePhrases(string[] verbs, Dictionary<string, string> thirdPersonVerbs, Dictionary<string, string> gerundVerbs)
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

            var verbsLength = verbs.Length;
            var beginningsLength = beginnings.Length;
            var middlePartsLength = middleParts.Length;

            for (var verbIndex = 0; verbIndex < verbsLength; verbIndex++)
            {
                var word = verbs[verbIndex];
                var noun = word.ToUpperCaseAt(0);
                var verb = word.ToLowerCaseAt(0);

                var thirdPersonStart = thirdPersonVerbs[word];
                var thirdPersonVerb = thirdPersonStart.ToLowerCaseAt(0);
                var gerundVerb = gerundVerbs[word].ToLowerCaseAt(0);

                var fix = thirdPersonStart + " ";

                for (var beginningIndex = 0; beginningIndex < beginningsLength; beginningIndex++)
                {
                    var start = beginnings[beginningIndex];

                    for (var middleIndex = 0; middleIndex < middlePartsLength; middleIndex++)
                    {
                        var middle = middleParts[middleIndex];
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

        private static IEnumerable<string> CreateUsedToPhrases()
        {
            string[] subjects =
                                {
                                    "A class", "An class", "The class", "This class", "Class",
                                    "A base class", "An base class", "The base class", "This base class", "Base class",
                                    "A abstract base class", "An abstract base class", "The abstract base class", "This abstract base class", "Abstract base class",
                                    "A interface", "An interface", "The interface", "This interface", "Interface",
                                    "A component", "An component", "The component", "This component", "Component",
                                    "A attribute", "An attribute", "The attribute", "This attribute", "Attribute",
                                };
            string[] conjunctions = { "that", "which" };
            string[] conditionals = { "is", "can be", "could be", "may be", "might be", "shall be", "should be", "will be", "would be" };

            foreach (var subject in subjects)
            {
                yield return string.Concat(subject, " to ");
                yield return string.Concat(subject, " allows to ");
                yield return string.Concat(subject, " used to ");
                yield return string.Concat(subject, " able to ");
                yield return string.Concat(subject, " capable to ");

                foreach (var conditional in conditionals)
                {
                    yield return string.Concat(subject, " ", conditional, " used to ");
                    yield return string.Concat(subject, " ", conditional, " able to ");
                    yield return string.Concat(subject, " ", conditional, " capable to ");
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var conjunction in conjunctions)
                {
                    var beginning = string.Concat(subject, " ", conjunction, " ");

                    yield return string.Concat(beginning, "allows to ");

                    foreach (var conditional in conditionals)
                    {
                        yield return string.Concat(beginning, conditional, " used to ");
                        yield return string.Concat(beginning, conditional, " able to ");
                        yield return string.Concat(beginning, conditional, " capable to ");
                    }
                }
            }

            yield return "Used to ";
            yield return "Able to ";
            yield return "Capable to ";
        }

//// ncrunch: rdi default
    }
}