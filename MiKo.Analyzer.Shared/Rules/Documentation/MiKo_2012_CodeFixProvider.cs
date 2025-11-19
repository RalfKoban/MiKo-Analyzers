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
        private const FirstWordAdjustment StartAdjustment = FirstWordAdjustment.StartUpperCase | FirstWordAdjustment.MakeThirdPersonSingular | FirstWordAdjustment.KeepSingleLeadingSpace;

        private static readonly string[] DefaultPhrases =
                                                          {
                                                              "A default impl",
                                                              "A default-impl",
                                                              "A impl ",
                                                              "A implementation ",
                                                              "An default impl", // typo
                                                              "An default-impl", // typo
                                                              "An implementation ",
                                                              "An impl ",
                                                              "Default impl",
                                                              "Default-impl",
                                                              "Implementation ",
                                                              "Impl ",
                                                              "The default impl",
                                                              "The default-impl",
                                                              "The implementation ",
                                                              "The impl ",
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

        private static readonly string[] GetSetReplacementPhrases = CreateGetSetReplacementPhrases().Except(new[] { "Gets or sets a value ", "Gets or sets " })
                                                                                                    .OrderDescendingByLengthAndText();

        //// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2012";

        internal static SyntaxNode GetUpdatedSyntax(XmlElementSyntax comment)
        {
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

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordAdjustment.StartLowerCase);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax((XmlElementSyntax)syntax);

        private static SyntaxNode GetUpdatedSyntax(XmlElementSyntax comment, XmlTextSyntax textSyntax)
        {
            var trimmedText = textSyntax.GetTextTrimmed();
            var text = trimmedText.AsSpan();

            if (text.StartsWith("Interaction logic for"))
            {
                // seems like this is the default comment for WPF controls
                return Comment(comment, XmlText("Represents a TODO"));
            }

            if (text.StartsWithAny(EmptyReplacementsMapKeys))
            {
                return Comment(comment, EmptyReplacementsMapKeys, EmptyReplacementsMap, StartAdjustment);
            }

            if (text.StartsWith("Called"))
            {
                return Comment(comment, trimmedText.Replace("Called", "Gets called"));
            }

            if (comment.GetEnclosing(Declarations) is MemberDeclarationSyntax member)
            {
                var name = member.GetName();

                if (name.Contains(Constants.Names.Command, StringComparison.OrdinalIgnoreCase) && MiKo_2038_CodeFixProvider.CanFix(text))
                {
                    return MiKo_2038_CodeFixProvider.GetUpdatedSyntax(comment);
                }

                if (name.Contains(Constants.Names.Factory, StringComparison.OrdinalIgnoreCase) && MiKo_2060_CodeFixProvider.CanFix(text))
                {
                    return MiKo_2060_CodeFixProvider.GetUpdatedSyntax(comment);
                }

                if (name.Contains(nameof(EventArgs)))
                {
                    return MiKo_2002_CodeFixProvider.GetUpdatedSyntax(comment);
                }

                if (MiKo_2039_CodeFixProvider.CanFix(text))
                {
                    return MiKo_2039_CodeFixProvider.GetUpdatedSyntax(comment);
                }

                foreach (var phrase in UsedToPhrases)
                {
                    if (text.StartsWith(phrase))
                    {
                        var remainingText = text.Slice(phrase.Length);

                        if (member is PropertyDeclarationSyntax p)
                        {
                            return GetUpdatedProperty(comment, p, remainingText);
                        }

                        var firstWord = remainingText.FirstWord();
                        var index = remainingText.IndexOf(firstWord);
                        var replacementForFirstWord = Verbalizer.MakeThirdPersonSingularVerb(firstWord.ToUpperCaseAt(0));

                        var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText.Slice(index + firstWord.Length));

                        return Comment(comment, replacedText, comment.Content.RemoveAt(0));
                    }
                }

                if (text.StartsWithAny(ReplacementMapKeys))
                {
                    return Comment(comment, ReplacementMapKeys, ReplacementMap, StartAdjustment);
                }

                switch (member)
                {
                    case PropertyDeclarationSyntax property:
                        return GetUpdatedProperty(comment, property, text);

                    case BaseTypeDeclarationSyntax _ when text.StartsWithAny(Constants.Comments.AAnThePhraseWithSpaces):
                        return CommentStartingWith(comment, "Represents ");
                }
            }

            return comment;
        }

        private static XmlEmptyElementSyntax GetUpdatedSyntaxWithInheritdoc(in SyntaxList<XmlNodeSyntax> content)
        {
            var inheritdoc = content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Inheritdoc);

            if (inheritdoc != null)
            {
                // special case: it's an inherit documentation, so mark it so
                return inheritdoc;
            }

            // maybe it is a documentation that should be an inherit documentation instead
            if (content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.GetTextTrimmed();

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
                                            .ReplaceAllWithProbe(GetSetReplacementPhrases, " ");

            builder.ReplaceWithProbe("  ", " ");
            builder.ReplaceWithProbe("indicating get or set ", "indicating ");
            builder.ReplaceWithProbe("indicating describes ", "indicating ");
            builder.ReplaceWithProbe("indicating describe ", "indicating ");
            builder.ReplaceWithProbe("indicating specifies ", "indicating ");
            builder.ReplaceWithProbe("indicating specify ", "indicating ");
            builder.ReplaceWithProbe("indicating indicates ", "indicating ");
            builder.ReplaceWithProbe("indicating indicate ", "indicating ");
            builder.ReplaceWithProbe("bool indicating", "value indicating");
            builder.ReplaceWithProbe("bool that indicates", "value indicating");
            builder.ReplaceWithProbe("bool which indicates", "value indicating");
            builder.ReplaceWithProbe("boolean indicating", "value indicating");
            builder.ReplaceWithProbe("boolean that indicates", "value indicating");
            builder.ReplaceWithProbe("boolean which indicates", "value indicating");
            builder.ReplaceWithProbe("value that indicates", "value indicating");
            builder.ReplaceWithProbe("value which indicates", "value indicating");
            builder.ReplaceWithProbe("value indicating whether value indicating", "value indicating");
            builder.ReplaceWithProbe("value indicating value indicating", "value indicating");
            builder.ReplaceWithProbe("the value indicat", "a value indicat");
            builder.ReplaceWithProbe(" value indicating whether a value indicating", " value indicating");
            builder.ReplaceWithProbe(" value indicating gets a value indicating", " value indicating");
            builder.ReplaceWithProbe(" value indicating sets a value indicating", " value indicating");
            builder.ReplaceWithProbe(" value indicating gets or sets a value indicating", " value indicating");
            builder.ReplaceWithProbe(" value indicating a value indicating", " value indicating");
            builder.ReplaceWithProbe("whether value indicating whether", "whether");
            builder.ReplaceWithProbe("a value indicating get ", "a value indicating ");
            builder.ReplaceWithProbe("a value indicating set ", "a value indicating ");
            builder.ReplaceWithProbe("indicating that indicates if", "indicating whether");
            builder.ReplaceWithProbe("indicating which indicates if", "indicating whether");
            builder.ReplaceWithProbe("indicating that indicates that", "indicating whether");
            builder.ReplaceWithProbe("indicating which indicates that", "indicating whether");
            builder.ReplaceWithProbe("indicating that indicates whether", "indicating whether");
            builder.ReplaceWithProbe("indicating which indicates whether", "indicating whether");
            builder.ReplaceWithProbe("indicating that", "indicating whether");
            builder.ReplaceWithProbe("indicating if ", "indicating whether ");
            builder.ReplaceWithProbe("indicating to", "indicating whether to");
            builder.ReplaceWithProbe("indicating indicating", "indicating");
            builder.ReplaceWithProbe("whether to true if to", "whether to");
            builder.ReplaceWithProbe("whether to true to", "whether to");
            builder.ReplaceWithProbe("whether to true, to", "whether to");
            builder.ReplaceWithProbe("whether describe if", "whether");
            builder.ReplaceWithProbe("whether describe that", "whether");
            builder.ReplaceWithProbe("whether describe whether", "whether");
            builder.ReplaceWithProbe("whether describes if", "whether");
            builder.ReplaceWithProbe("whether describes that", "whether");
            builder.ReplaceWithProbe("whether describes whether", "whether");
            builder.ReplaceWithProbe("whether indicate if", "whether");
            builder.ReplaceWithProbe("whether indicate that", "whether ");
            builder.ReplaceWithProbe("whether indicate whether", "whether");
            builder.ReplaceWithProbe("whether indicates if", "whether");
            builder.ReplaceWithProbe("whether indicates that", "whether ");
            builder.ReplaceWithProbe("whether indicates whether", "whether");
            builder.ReplaceWithProbe("whether specifies if", "whether");
            builder.ReplaceWithProbe("whether specifies that", "whether");
            builder.ReplaceWithProbe("whether specifies whether", "whether");
            builder.ReplaceWithProbe("whether specify if", "whether");
            builder.ReplaceWithProbe("whether specify that", "whether");
            builder.ReplaceWithProbe("whether specify whether", "whether");
            builder.ReplaceWithProbe("ets get ", "ets ");
            builder.ReplaceWithProbe("ets set ", "ets ");
            builder.ReplaceWithProbe("  ", " ");

            var replacedFixedText = builder.ToStringAndRelease();

            return Comment(comment, replacedFixedText, comment.Content.RemoveAt(0));
        }

        private static string GetPropertyStartingPhrase(PropertyDeclarationSyntax property)
        {
            var boolean = property.Type.IsBoolean();

            var accessorList = property.AccessorList;

            if (accessorList is null)
            {
                // seem to be an expression body, so it's a getter only
                return boolean ? "Gets a value indicating " : "Gets ";
            }

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

            return result.Select(_ => new Pair(_.Key, _.Value))
                         .OrderByDescending(_ => _.Key.Length)
                         .ThenBy(_ => _.Key, AscendingStringComparer.Default) // sort by first character
                         .ToArray();
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

            yield return new Pair("Class part ", "Represents the part ");
            yield return new Pair("Class Part ", "Represents the part ");
            yield return new Pair("Class that serves as ", "Represents a ");
            yield return new Pair("Class that serves ", "Provides ");
            yield return new Pair("Class which serves as ", "Represents a ");
            yield return new Pair("Class which serves ", "Provides ");

            yield return new Pair("This is an adapter for ", "Adapts ");
            yield return new Pair("This is an adapter between ", "Adapts between ");

            // initialize method
            yield return new Pair("Initialize ", "Initializes ");
            yield return new Pair("This method returns ", "Returns ");
            yield return new Pair("This method initializes ", "Initializes ");
            yield return new Pair("This method will initialize ", "Initializes ");
            yield return new Pair("This will initialize ", "Initializes ");

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
            yield return new Pair("This interface shall be implemented by classes that want to ");
            yield return new Pair("The interface shall be implemented by classes that want to ");
            yield return new Pair("This interface shall be implemented by components that want to ");
            yield return new Pair("The interface shall be implemented by components that want to ");

            yield return new Pair("This class is responsible for ");
            yield return new Pair("This interface is responsible for ");
            yield return new Pair("This method is responsible for ");
            yield return new Pair("Is responsible for ");
            yield return new Pair("Responsible for ");

            // view models
            yield return new Pair("A View model for ", "Represents the view model of ");
            yield return new Pair("A View Model for ", "Represents the view model of ");
            yield return new Pair("A View model of ", "Represents the view model of ");
            yield return new Pair("A View Model of ", "Represents the view model of ");
            yield return new Pair("A View model representing ", "Represents the view model of ");
            yield return new Pair("A View Model representing ", "Represents the view model of ");
            yield return new Pair("A view model that is needed to ");
            yield return new Pair("A View model that represents ", "Represents the view model of ");
            yield return new Pair("A View Model that represents ", "Represents the view model of ");
            yield return new Pair("A view model which is needed to ");
            yield return new Pair("A ViewModel for ", "Represents the view model of ");
            yield return new Pair("A ViewModel of ", "Represents the view model of ");
            yield return new Pair("A ViewModel representing ", "Represents the view model of ");
            yield return new Pair("A ViewModel that is needed to ");
            yield return new Pair("A ViewModel that represents ", "Represents the view model of ");
            yield return new Pair("A ViewModel which is needed to ");
            yield return new Pair("The View model for ", "Represents the view model of ");
            yield return new Pair("The View Model for ", "Represents the view model of ");
            yield return new Pair("The View model of ", "Represents the view model of ");
            yield return new Pair("The View Model of ", "Represents the view model of ");
            yield return new Pair("The View model representing ", "Represents the view model of ");
            yield return new Pair("The View Model representing ", "Represents the view model of ");
            yield return new Pair("The view model that is needed to ");
            yield return new Pair("The View model that represents ", "Represents the view model of ");
            yield return new Pair("The View Model that represents ", "Represents the view model of ");
            yield return new Pair("The view model which is needed to ");
            yield return new Pair("The ViewModel for ", "Represents the view model of ");
            yield return new Pair("The ViewModel of ", "Represents the view model of ");
            yield return new Pair("The ViewModel representing ", "Represents the view model of ");
            yield return new Pair("The ViewModel that is needed to ");
            yield return new Pair("The ViewModel that represents ", "Represents the view model of ");
            yield return new Pair("The ViewModel which is needed to ");
            yield return new Pair("This view model ");
            yield return new Pair("This view model is needed to ");
            yield return new Pair("This ViewModel is needed to ");
            yield return new Pair("View model for ", "Represents the view model of ");
            yield return new Pair("View Model for ", "Represents the view model of ");
            yield return new Pair("View model needed to ");
            yield return new Pair("View model of ", "Represents the view model of ");
            yield return new Pair("View Model of ", "Represents the view model of ");
            yield return new Pair("View model representing ", "Represents the view model of ");
            yield return new Pair("View Model representing ", "Represents the view model of ");
            yield return new Pair("View model that is needed to ");
            yield return new Pair("View model that represents ", "Represents the view model of ");
            yield return new Pair("View Model that represents ", "Represents the view model of ");
            yield return new Pair("View model which is needed to ");
            yield return new Pair("ViewModel for ", "Represents the view model of ");
            yield return new Pair("ViewModel needed to ");
            yield return new Pair("ViewModel of ", "Represents the view model of ");
            yield return new Pair("ViewModel representing ", "Represents the view model of ");
            yield return new Pair("ViewModel that is needed to ");
            yield return new Pair("ViewModel that represents ", "Represents the view model of ");
            yield return new Pair("ViewModel which is needed to ");

            yield return new Pair("Base for all ", "Represents ");

            yield return new Pair("Use this method to ");
            yield return new Pair("Use this method, to ");
            yield return new Pair("Use this Method to "); // typo in real-life scenario
            yield return new Pair("Use this Method, to "); // typo in real-life scenario
            yield return new Pair("Use this class to ");
            yield return new Pair("Use this class, to ");
            yield return new Pair("Use this Class to "); // typo in real-life scenario
            yield return new Pair("Use this Class, to "); // typo in real-life scenario

            yield return new Pair("The Method will be called", "Gets called"); // typo in real-life scenario
            yield return new Pair("The method will be called", "Gets called");
            yield return new Pair("The Method is called", "Gets called"); // typo in real-life scenario
            yield return new Pair("The method is called", "Gets called");
            yield return new Pair("This Method will be called", "Gets called"); // typo in real-life scenario
            yield return new Pair("This method will be called", "Gets called");
            yield return new Pair("This Method is called", "Gets called"); // typo in real-life scenario
            yield return new Pair("This method is called", "Gets called");
            yield return new Pair("This Method "); // typo in real-life scenario
            yield return new Pair("This method ");
            yield return new Pair("This Method will "); // typo in real-life scenario
            yield return new Pair("This method will ");
            yield return new Pair("This Class "); // typo in real-life scenario
            yield return new Pair("This class ");
            yield return new Pair("This Callback "); // typo in real-life scenario
            yield return new Pair("This Call-back "); // typo in real-life scenario
            yield return new Pair("This callback ");
            yield return new Pair("This call-back ");
            yield return new Pair("This Callback will "); // typo in real-life scenario
            yield return new Pair("This Call-back will "); // typo in real-life scenario
            yield return new Pair("This callback will ");
            yield return new Pair("This call-back will ");

            yield return new Pair("This control ");
            yield return new Pair("This control will ");
            yield return new Pair("This Control ");
            yield return new Pair("This Control will ");

            yield return new Pair("This handler ");
            yield return new Pair("This Handler ");
            yield return new Pair("This handler will ");
            yield return new Pair("This Handler will ");

            yield return new Pair("This will ");
            yield return new Pair("This ");

            foreach (var phrase in CreatePhrases(verbs, thirdPersonVerbs, gerundVerbs))
            {
                yield return phrase;
            }
        }

        private static string[] GetSubjects()
        {
            return new[]
                       {
                           "A class", "An class", "The class", "This class", "Class",
                           "A base class", "An base class", "The base class", "This base class", "Base class",
                           "A abstract base class", "An abstract base class", "The abstract base class", "This abstract base class", "Abstract base class",
                           "A interface", "An interface", "The interface", "This interface", "Interface",  "The interface class", "This interface class",
                           "A component", "An component", "The component", "This component", "Component",
                           "A attribute", "An attribute", "The attribute", "This attribute", "Attribute",
                           "Classes implementing the interface", "Classes implementing the interface,",
                           "Classes implementing the interfaces", "Classes implementing the interfaces,",
                           "Classes implementing this interface", "Classes implementing this interface,",
                           "Classes implementing this interfaces", "Classes implementing this interfaces,",
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
                           "The class implementing the interface",
                           "The class implementing the interface,",
                           "The class implementing this interface",
                           "A callback", "A Callback", "A call-back", "A Call-back",
                           "The callback", "The Callback", "The call-back", "The Call-back",
                           "This callback", "This Callback", "This call-back", "This Call-back",
                       };
        }

        private static IEnumerable<Pair> CreatePhrases(string[] verbs, Dictionary<string, string> thirdPersonVerbs, Dictionary<string, string> gerundVerbs)
        {
            // GetSubjects
            var beginnings = GetSubjects().ConcatenatedWith("Extension method", "Factory method", "Function", "Help function", "Help method", "Helper class", "Helper function", "Helper method", "Interface for", "Interface implemented to", "Method")
                                          .Distinct()
                                          .ToArray();

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
                    var beginning = beginnings[beginningIndex];

                    for (var middleIndex = 0; middleIndex < middlePartsLength; middleIndex++)
                    {
                        var middle = middleParts[middleIndex];
                        var start = string.Concat(beginning, " ", middle, " ");

                        yield return new Pair(string.Concat(start, verb, " "), fix);
                        yield return new Pair(string.Concat(start, thirdPersonVerb, " "), fix);
                    }

                    yield return new Pair(string.Concat(beginning, " to ", noun, " "), fix);
                    yield return new Pair(string.Concat(beginning, " to ", verb, " "), fix);

                    yield return new Pair(string.Concat(beginning, " ", verb, " "), fix);
                    yield return new Pair(string.Concat(beginning, " ", thirdPersonVerb, " "), fix);
                    yield return new Pair(string.Concat(beginning, " ", gerundVerb, " "), fix);
                }
            }

            const string Provides = "Provides ";

            for (var beginningIndex = 0; beginningIndex < beginningsLength; beginningIndex++)
            {
                var beginning = beginnings[beginningIndex];

                yield return new Pair(beginning + " that is used as helper for ", Provides);
                yield return new Pair(beginning + " that is used as helper class for ", Provides);
                yield return new Pair(beginning + " that is used as a helper for ", Provides);
                yield return new Pair(beginning + " that is used as a helper class for ", Provides);
                yield return new Pair(beginning + " which is used as helper for ", Provides);
                yield return new Pair(beginning + " which is used as helper class for ", Provides);
                yield return new Pair(beginning + " which is used as a helper for ", Provides);
                yield return new Pair(beginning + " which is used as a helper class for ", Provides);
                yield return new Pair(beginning + " used as helper for ", Provides);
                yield return new Pair(beginning + " used as helper class for ", Provides);
                yield return new Pair(beginning + " used as a helper for ", Provides);
                yield return new Pair(beginning + " used as a helper class for ", Provides);

                yield return new Pair(beginning + " to contain ", Provides);
                yield return new Pair(beginning + " to offer ", Provides);
                yield return new Pair(beginning + " to retrieve ", Provides);
                yield return new Pair(beginning + " to provide functionality to ");
                yield return new Pair(beginning + " to ");

                yield return new Pair(beginning + " containing ", Provides);
                yield return new Pair(beginning + " offering ", Provides);
                yield return new Pair(beginning + " retrieving ", Provides);
                yield return new Pair(beginning + " providing functionality to ");
                yield return new Pair(beginning + " representing ", "Represents ");
                yield return new Pair(beginning + " Representing ", "Represents "); // upper-case is intended as this is a real-world-scenario
                yield return new Pair(beginning + " will be called with their ", Provides + "a "); // upper-case is intended as this is a real-world-scenario

                for (var middleIndex = 0; middleIndex < middlePartsLength; middleIndex++)
                {
                    var middle = middleParts[middleIndex];
                    var start = string.Concat(beginning, " ", middle, " ");

                    yield return new Pair(start + "contains ", Provides);
                    yield return new Pair(start + "contain ", Provides);

                    yield return new Pair(start + "offers ", Provides);
                    yield return new Pair(start + "offer ", Provides);

                    yield return new Pair((beginning + " will be called with their ").Replace("will will", "will"), Provides + "a "); // upper-case is intended as this is a real-world-scenario

                    yield return new Pair(start + "provides functionality to ");

                    yield return new Pair(start);
                }

                yield return new Pair(beginning + " for checking ", "Determines ");
                yield return new Pair(beginning + " for ");
                yield return new Pair(beginning + " in charge of ");
                yield return new Pair(beginning + " specialized in ");
                yield return new Pair(beginning + " used for checking ", "Determines ");
                yield return new Pair(beginning + " used for ");
            }
        }

        private static IEnumerable<string> CreateUsedToPhrases()
        {
            var subjects = GetSubjects();
            string[] conjunctions = { "that", "which" };
            string[] conditionals = { "is", "can be", "could be", "may be", "might be", "shall be", "should be", "will be", "would be" };

            foreach (var subject in subjects)
            {
                // yield return string.Concat(subject, " to ");
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

        private static IEnumerable<string> CreateGetSetReplacementPhrases()
        {
            var continuations = new[] { "flag ", "a flag ", "the flag ", "value ", "a value ", "the value " };

            var starts = new[]
                             {
                                 "Get and Set ",
                                 "Get And Set ",
                                 "Get AND Set ",
                                 "Get or Set ",
                                 "Get Or Set ",
                                 "Get OR Set ",
                                 "get/set ",
                                 "get/Set ",
                                 "Get/set ",
                                 "Get/Set ",
                                 "Gets and Sets ",
                                 "Gets And Sets ",
                                 "Gets AND Sets ",
                                 "Gets or sets ",
                                 "Gets or Sets ",
                                 "Gets Or Sets ",
                                 "Gets OR Sets ",
                                 "gets/sets ",
                                 "gets/Sets ",
                                 "Gets/sets ",
                                 "Gets/Sets ",
                                 "set/get ",
                                 "set/Get ",
                                 "Set/get ",
                                 "Set/Get ",
                                 "sets/gets ",
                                 "sets/Gets ",
                                 "Sets/gets ",
                                 "Sets/Gets ",
                             };

            foreach (var start in starts)
            {
                yield return start;

                foreach (var continuation in continuations)
                {
                    yield return start + continuation;
                }
            }
        }

        //// ncrunch: rdi default
    }
}