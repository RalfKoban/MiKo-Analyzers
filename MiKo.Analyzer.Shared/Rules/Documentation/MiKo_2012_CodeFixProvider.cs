using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

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

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap, Comparison);

        private static readonly Pair[] EmptyReplacementsMap =
                                                              {
                                                                  new Pair("Called to "),
                                                              };

        private static readonly string[] EmptyReplacementsMapKeys = GetTermsForQuickLookup(EmptyReplacementsMap);

        private static readonly string[] GetSetReplacementPhrases = CreateGetSetReplacementPhrases().Distinct()
                                                                                                    .Except(new[] { "Gets or sets a value ", "Gets or sets ", "Gets a value ", "Sets a value ", "gets a value ", "sets a value " })
                                                                                                    .OrderDescendingByLengthAndText();

        private static readonly Pair[] PreparationMap =
                                                        {
                                                            new Pair(Constants.Comments.SealedClassPhrase, "##SEALED##"),
                                                            new Pair(Constants.Comments.FieldIsReadOnly, "##READONLY##"),
                                                        };

        private static readonly string[] PreparationMapKeys = GetTermsForQuickLookup(PreparationMap);

        private static readonly Pair[] CleanupMap =
                                                    {
                                                        new Pair("##SEALED##", Constants.Comments.SealedClassPhrase),
                                                        new Pair("##READONLY##", Constants.Comments.FieldIsReadOnly),
                                                    };

        private static readonly string[] CleanupMapKeys = GetTermsForQuickLookup(CleanupMap);

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

            return GetUpdatedSyntax(comment, FirstWordAdjustment.StartLowerCase);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax comment, in FirstWordAdjustment startAdjustment)
        {
            var preparedComment = Comment(comment, PreparationMapKeys, PreparationMap);
            var updatedComment = Comment(preparedComment, ReplacementMapKeys, ReplacementMap, startAdjustment);
            var cleanedComment = Comment(updatedComment, CleanupMapKeys, CleanupMap);

            return cleanedComment;
        }

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
                return Comment(comment, EmptyReplacementsMapKeys, EmptyReplacementsMap, FirstWordAdjustment.StartUpperCase | FirstWordAdjustment.MakeThirdPersonSingular | FirstWordAdjustment.KeepSingleLeadingSpace);
            }

            if (text.StartsWith("Called"))
            {
                return Comment(comment, trimmedText.Replace("Called", "Gets called"));
            }

            if (comment.GetEnclosing(Declarations) is MemberDeclarationSyntax member)
            {
                return GetUpdatedSyntax(comment, textSyntax, member, text);
            }

            return comment;
        }

        private static ReadOnlySpan<char> GetUpdatedSyntax(ref XmlElementSyntax comment, XmlTextSyntax textSyntax, in ReadOnlySpan<char> text)
        {
            if (text.ContainsAny(ReplacementMapKeys, Comparison))
            {
                var adjustment = FirstWordAdjustment.StartUpperCase | FirstWordAdjustment.KeepSingleLeadingSpace;

                if (text.StartsWithAny(ReplacementMapKeys, Comparison))
                {
                    adjustment |= FirstWordAdjustment.MakeThirdPersonSingular;
                }

                comment = GetUpdatedSyntax(comment, adjustment);

                if (comment.Content.First() is XmlTextSyntax t && ReferenceEquals(t, textSyntax) is false)
                {
                    return t.GetTextTrimmed().AsSpan();
                }
            }

            return text;
        }

        private static SyntaxNode GetUpdatedSyntax(XmlElementSyntax comment, XmlTextSyntax textSyntax, MemberDeclarationSyntax member, ReadOnlySpan<char> text)
        {
            var name = member.GetName();

            if (name.Contains(Constants.Names.Command, Comparison) && MiKo_2038_CodeFixProvider.CanFix(text))
            {
                return MiKo_2038_CodeFixProvider.GetUpdatedSyntax(comment);
            }

            if (name.Contains(Constants.Names.Factory, Comparison) && MiKo_2060_CodeFixProvider.CanFix(text))
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

            switch (member)
            {
                case PropertyDeclarationSyntax property:
                {
                    text = GetUpdatedSyntax(ref comment, textSyntax, text);

                    return GetUpdatedProperty(comment, property, text);
                }

                case BaseTypeDeclarationSyntax _:
                {
                    text = GetUpdatedSyntax(ref comment, textSyntax, text);

                    if (text.StartsWithAny(Constants.Comments.AAnThePhraseWithSpaces))
                    {
                        comment = CommentStartingWith(comment, "Represents ");
                    }

                    return comment;
                }

                default:
                {
                    GetUpdatedSyntax(ref comment, textSyntax, text);

                    return comment;
                }
            }
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

                if (text.StartsWithAny(DefaultPhrases, Comparison))
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
            builder.ReplaceWithProbe("indicating describes ", "indicating ");
            builder.ReplaceWithProbe("indicating describe ", "indicating ");
            builder.ReplaceWithProbe("indicating specifies ", "indicating ");
            builder.ReplaceWithProbe("indicating specify ", "indicating ");
            builder.ReplaceWithProbe("indicating indicates ", "indicating ");
            builder.ReplaceWithProbe("indicating indicate ", "indicating ");
            builder.ReplaceWithProbe("indicating returns ", "indicating ");
            builder.ReplaceWithProbe("indicating return ", "indicating ");
            builder.ReplaceWithProbe("indicating indicating", "indicating");
            builder.ReplaceWithProbe("indicating for ", "indicating whether the ");
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
            builder.ReplaceWithProbe("whether whether", "whether");
            builder.ReplaceWithProbe("whether set to true then", "whether");
            builder.ReplaceWithProbe("whether set to true, then", "whether");
            builder.ReplaceWithProbe("whether set to True then", "whether");
            builder.ReplaceWithProbe("whether set to True, then", "whether");
            builder.ReplaceWithProbe("whether set to TRUE then", "whether");
            builder.ReplaceWithProbe("whether set to TRUE, then", "whether");
            builder.ReplaceWithProbe("ets get ", "ets ");
            builder.ReplaceWithProbe("ets set ", "ets ");
            builder.ReplaceWithProbe("gets returns", "gets");
            builder.ReplaceWithProbe("Gets returns", "Gets");
            builder.ReplaceWithProbe("sets returns", "sets");
            builder.ReplaceWithProbe("Sets returns", "Sets");
            builder.ReplaceWithProbe(" the the ", " the ");
            builder.ReplaceWithProbe(" the an ", " an ");
            builder.ReplaceWithProbe(" the a ", " a ");
            builder.ReplaceWithProbe(" to the ", " the ");
            builder.ReplaceWithProbe(" to an ", " an ");
            builder.ReplaceWithProbe(" to a ", " a ");
            builder.ReplaceWithProbe(" true if ", " whether ");
            builder.ReplaceWithProbe(" tRUE if ", " whether ");
            builder.ReplaceWithProbe(" true when ", " whether ");
            builder.ReplaceWithProbe(" tRUE when ", " whether ");
            builder.ReplaceWithProbe(" when set to true then ", " whether ");
            builder.ReplaceWithProbe(" when set to true, then ", " whether ");
            builder.ReplaceWithProbe(" when set to True then ", " whether ");
            builder.ReplaceWithProbe(" when set to True, then ", " whether ");
            builder.ReplaceWithProbe(" when set to TRUE then ", " whether ");
            builder.ReplaceWithProbe(" when set to TRUE, then ", " whether ");
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
                return boolean ? Constants.Comments.BooleanPropertyGetterStartingPhrase : Constants.Comments.PropertyGetterStartingPhrase;
            }

            var accessors = accessorList.Accessors;

            switch (accessors.Count)
            {
                case 1:
                {
                    switch (accessors[0].Kind())
                    {
                        case SyntaxKind.GetAccessorDeclaration:
                            return boolean ? Constants.Comments.BooleanPropertyGetterStartingPhrase : Constants.Comments.PropertyGetterStartingPhrase;

                        case SyntaxKind.SetAccessorDeclaration:
                            return boolean ? Constants.Comments.BooleanPropertySetterStartingPhrase : Constants.Comments.PropertySetterStartingPhrase;

                        default:
                            return string.Empty;
                    }
                }

                case 2:
                    return boolean ? Constants.Comments.BooleanPropertyGetterSetterStartingPhrase : Constants.Comments.PropertyGetterSetterStartingPhrase;

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
            yield return new Pair("Is used to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new Pair("Is used for creating ", Constants.Comments.FactorySummaryPhrase);

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
            yield return new Pair("Extension interface for ", "Enhances ");
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
            yield return new Pair("Interface that can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("Interface that can optionally be implemented by ", "Enhances ");
            yield return new Pair("Interface that can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("Interface which can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("Interface which can optionally be implemented by ", "Enhances ");
            yield return new Pair("Interface which can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("A interface can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("A interface can optionally be implemented by ", "Enhances ");
            yield return new Pair("A interface can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("An interface can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("An interface can optionally be implemented by ", "Enhances ");
            yield return new Pair("An interface can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("The interface can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("The interface can optionally be implemented by ", "Enhances ");
            yield return new Pair("The interface can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("This interface can be implemented optionally by implementations of ", "Enhances ");
            yield return new Pair("This interface can optionally be implemented by ", "Enhances ");
            yield return new Pair("This interface can optionally added to classes implementing ", "Enhances ");
            yield return new Pair("Extension interface for ", "Enhances ");

            yield return new Pair("Implementations of this class allow to ");
            yield return new Pair("Implementations of this class allows to ");
            yield return new Pair("Implementations of this class can be used to ");
            yield return new Pair("Implementations of this interface allow to ");
            yield return new Pair("Implementations of this interface allows to ");
            yield return new Pair("Implementations of this interface can be used to ");
            yield return new Pair("Implementations of this class can ");
            yield return new Pair("Implementations of this interface can ");

            yield return new Pair("Implementers of this class allow to ");
            yield return new Pair("Implementers of this class allows to ");
            yield return new Pair("Implementers of this class can be used to ");
            yield return new Pair("Implementers of this interface allow to ");
            yield return new Pair("Implementers of this interface allows to ");
            yield return new Pair("Implementers of this interface can be used to ");
            yield return new Pair("Implementers of this class can ");
            yield return new Pair("Implementers of this interface can ");

            yield return new Pair("Implementer of this class allow to ");
            yield return new Pair("Implementer of this class allows to ");
            yield return new Pair("Implementer of this class can be used to ");
            yield return new Pair("Implementer of this interface allow to ");
            yield return new Pair("Implementer of this interface allows to ");
            yield return new Pair("Implementer of this interface can be used to ");
            yield return new Pair("Implementer of this class can ");
            yield return new Pair("Implementer of this interface can ");

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

            yield return new Pair("Use a instance of the class to ");
            yield return new Pair("Use an instance of the class to ");
            yield return new Pair("Use the instance of the class to ");
            yield return new Pair("Use a instance of this class to ");
            yield return new Pair("Use an instance of this class to ");
            yield return new Pair("Use the instance of this class to ");
            yield return new Pair("Use instances of the class to ");
            yield return new Pair("Use instances of this class to ");
            yield return new Pair("Use the instances of the class to ");
            yield return new Pair("Use the instances of this class to ");
            yield return new Pair("Use the class to ");
            yield return new Pair("Use this class to ");
            yield return new Pair("Use a this class to "); // typo
            yield return new Pair("Use an this class to "); // typo
            yield return new Pair("Use the this class to "); // typo
            yield return new Pair("Use a the class to "); // typo
            yield return new Pair("Use an the class to "); // typo
            yield return new Pair("Use the the class to "); // typo

            yield return new Pair("This Method should return", "Returns"); // typo in real-life scenario
            yield return new Pair("This method should return", "Returns");
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
            yield return new Pair("This Property "); // typo in real-life scenario
            yield return new Pair("This property ");
            yield return new Pair("This Property will "); // typo in real-life scenario
            yield return new Pair("This property will ");
            yield return new Pair("This Event "); // typo in real-life scenario
            yield return new Pair("This event ");
            yield return new Pair("This Event will "); // typo in real-life scenario
            yield return new Pair("This event will ");
            yield return new Pair("This Field "); // typo in real-life scenario
            yield return new Pair("This field ");
            yield return new Pair("This Field will "); // typo in real-life scenario
            yield return new Pair("This field will ");
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

            yield return new Pair("Helper inferface for", "Supports");
            yield return new Pair("Helper interface for", "Supports");
            yield return new Pair("Helper Inferface for", "Supports");
            yield return new Pair("Helper Interface for", "Supports");
            yield return new Pair("A helper inferface for", "Supports");
            yield return new Pair("A helper interface for", "Supports");
            yield return new Pair("A helper Inferface for", "Supports");
            yield return new Pair("A helper inferface for", "Supports");
            yield return new Pair("A helper Interface for", "Supports");
            yield return new Pair("A helper interface for", "Supports");
            yield return new Pair("An helper Inferface for", "Supports");
            yield return new Pair("An helper inferface for", "Supports");
            yield return new Pair("An helper Interface for", "Supports");
            yield return new Pair("An helper interface for", "Supports");
            yield return new Pair("The helper Inferface for", "Supports");
            yield return new Pair("The helper inferface for", "Supports");
            yield return new Pair("The helper Interface for", "Supports");
            yield return new Pair("The helper interface for", "Supports");

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
                           "Implementations of this class",
                           "Implementations of this interface",
                           "Implementers of this class",
                           "Implementers of this interface",
                           "Implementer of this class",
                           "Implementer of this interface",
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

                yield return new Pair(" used to ", " to ");
                yield return new Pair(" able to ", " to ");
                yield return new Pair(" capable to ", " to ");
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
            var continuations = new[]
                                    {
                                        "flag ", "a flag ", "the flag ",
                                        "Flag ", "a Flag ", "the Flag ",
                                        "value ", "a value ", "the value ",
                                        "information ", "a information ", "an information ", "the information ",
                                    };

            var gets = new[] { "Get", "get", "Gets", "gets", "GET", "GETS" };
            var sets = new[] { "Set", "set", "Sets", "sets", "SET", "SETS" };

            var conjunctions = new[] { "/", " and ", " And ", " AND ", " or ", " Or ", " OR " };

            var starts = new HashSet<string>();

            foreach (var conjunction in conjunctions)
            {
                foreach (var getter in gets)
                {
                    foreach (var setter in sets)
                    {
                        starts.Add(getter + conjunction + setter + " ");
                        starts.Add(setter + conjunction + getter + " ");
                    }
                }
            }

            foreach (var start in starts)
            {
                yield return start;

                foreach (var continuation in continuations)
                {
                    var phrase = start + continuation;

                    yield return phrase;
                    yield return phrase + "about ";
                }
            }

            foreach (var continuation in continuations)
            {
                foreach (var getter in gets)
                {
                    var phrase = getter + " " + continuation;

                    yield return phrase;
                    yield return phrase + "about ";
                }

                foreach (var setter in sets)
                {
                    var phrase = setter + " " + continuation;

                    yield return phrase;
                    yield return phrase + "about ";
                }
            }
        }

//// ncrunch: rdi default
    }
}