﻿using System;
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
        private static readonly string[] Verbs =
            {
                "Adopt",
                "Allow",
                "Create",
                "Describe",
                "Enhance",
                "Extend",
                "Generate",
                "Initialize",
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

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2012_MeaninglessSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2012_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
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
                var text = t.GetComment();

                if (text.StartsWith("Interaction logic for", StringComparison.Ordinal))
                {
                    // seems like this is the default comment for WPF controls
                    return Comment(comment, XmlText("Represents a TODO"));
                }

                if (text.StartsWithAny(Constants.Comments.FieldStartingPhrase, StringComparison.Ordinal))
                {
                    var property = syntax.Ancestors().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                    if (property != null)
                    {
                        var startingPhrase = GetPropertyStartingPhrase(property.AccessorList);

                        return CommentStartingWith(comment, startingPhrase);
                    }
                }
            }

            var updatedComment = Comment(comment, ReplacementMap.Keys, ReplacementMap);

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

            // maybe it is a docu that should be an inherit documentation instead
            if (content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.GetComment();

                if (text.IsNullOrWhiteSpace() && content.Count > 1 && IsSeeCref(content[1]))
                {
                    return Inheritdoc();
                }

                if (text.StartsWithAny(DefaultPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    if (content.Count > 1 && IsSeeCref(content[1]))
                    {
                        return Inheritdoc(GetCref(content[1]));
                    }

                    return Inheritdoc();
                }
            }

            return null;
        }

        private static string GetPropertyStartingPhrase(AccessorListSyntax accessorList)
        {
            if (accessorList == null)
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

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var entries = CreateReplacementMapEntries().OrderBy(_ => _.Key[0]).ToList(); // sort by first character

            var result = new Dictionary<string, string>(entries.Count);

            foreach (var entry in entries)
            {
                if (result.ContainsKey(entry.Key) is false)
                {
                    result[entry.Key] = entry.Value;
                }
            }

            return result;
        }

        private static IEnumerable<KeyValuePair<string, string>> CreateReplacementMapEntries()
        {
            // event arguments
            yield return new KeyValuePair<string, string>("Event argument for ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event argument that is used in the ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event argument that provides information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event argument which is used in the ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event argument which provides information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event arguments for ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event arguments that provide information ", Constants.Comments.EventArgsSummaryStartingPhrase);
            yield return new KeyValuePair<string, string>("Event arguments which provide information ", Constants.Comments.EventArgsSummaryStartingPhrase);

            // events
            yield return new KeyValuePair<string, string>("Event is fired ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event that is published ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event that is published, ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event which is published ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event which is published, ", "Occurs ");

            // factories
            yield return new KeyValuePair<string, string>("Factory for ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Factory class creating ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Factory class to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Factory class that creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Factory class which creates ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Interface for factories creating ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Interface for factories to create ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Interface for factories that create ", Constants.Comments.FactorySummaryPhrase);
            yield return new KeyValuePair<string, string>("Interface for factories which create ", Constants.Comments.FactorySummaryPhrase);

            yield return new KeyValuePair<string, string>("Class that serves as ", "Represents a ");
            yield return new KeyValuePair<string, string>("Class that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Class which serves as ", "Represents a ");
            yield return new KeyValuePair<string, string>("Class which serves ", "Provides ");

            yield return new KeyValuePair<string, string>("Command ", "Represents a command ");
            yield return new KeyValuePair<string, string>("Contain ", "Provides ");
            yield return new KeyValuePair<string, string>("Contains ", "Provides ");
            yield return new KeyValuePair<string, string>("Every class that implements the interface can ", "Allows to ");
            yield return new KeyValuePair<string, string>("Every class that implements this interface can ", "Allows to ");
            yield return new KeyValuePair<string, string>("Extension of ", "Extends the ");
            yield return new KeyValuePair<string, string>("Interface for the ", "Represents a ");
            yield return new KeyValuePair<string, string>("Interface that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Interface which serves ", "Provides ");
            yield return new KeyValuePair<string, string>("The class offers ", "Provides ");
            yield return new KeyValuePair<string, string>("The interface offers ", "Provides ");
            yield return new KeyValuePair<string, string>("This class offers ", "Provides ");
            yield return new KeyValuePair<string, string>("This interface offers ", "Provides ");

            foreach (var phrase in CreatePhrases())
            {
                yield return phrase;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> CreatePhrases()
        {
            var beginnings = new[]
                                 {
                                     "Class",
                                     "Classes implementing the interfaces",
                                     "Classes implementing the interfaces,",
                                     "Factory method",
                                     "Helper class",
                                     "Helper method",
                                     "Interface for classes",
                                     "Interface for elements",
                                     "Interface for items",
                                     "Interface for objects",
                                     "Interface for view models",
                                     "Interface for workflows",
                                     "Interface for work flows",
                                     "Interface for",
                                     "Interface",
                                     "The class implementing the interface",
                                     "The class implementing the interface,",
                                     "The class implementing this interface",
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
                        yield return new KeyValuePair<string, string>($"{start} {middle} {verb} ", fix);
                        yield return new KeyValuePair<string, string>($"{start} {middle} {thirdPersonVerb} ", fix);
                    }

                    yield return new KeyValuePair<string, string>($"{start} to {noun} ", fix);
                    yield return new KeyValuePair<string, string>($"{start} to {verb} ", fix);

                    yield return new KeyValuePair<string, string>($"{start} {verb} ", fix);
                    yield return new KeyValuePair<string, string>($"{start} {thirdPersonVerb} ", fix);
                    yield return new KeyValuePair<string, string>($"{start} {gerundVerb} ", fix);
                }
            }
        }
    }
}