using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2012_CodeFixProvider)), Shared]
    public sealed class MiKo_2012_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] Verbs =
            {
                "Allow",
                "Create",
                "Describe",
                "Enhance",
                "Extend",
                "Generate",
                "Initialize",
                "Manipulate",
                "Offer",
                "Provide",
                "Represent",
            };

        private static readonly Dictionary<string, string> PassiveVerbs = Verbs.ToDictionary(_ => _, _ => _ + "s");

        private static readonly Dictionary<string, string> GerundVerbs = Verbs.ToDictionary(_ => _, _ => (_ + "ing").Replace("eing", "ing"));

        private static readonly string[] StartingTerms =
            {
                "Class ",
                "Class to ",
                "Class that ",
                "Class which ",
                "Factory class ",
                "Factory class to ",
                "Factory class that ",
                "Factory class which ",
                "Factory method ",
                "Factory method to ",
                "Factory method that ",
                "Factory method which ",
                "Helper class to ",
                "Helper class that ",
                "Helper class which ",
                "Helper method to ",
                "Helper method that ",
                "Helper method which ",
                "Interface ",
                "Interface to ",
                "Interface that ",
                "Interface which ",
                "Interface for objects that can ",
                "The class implementing this interface ",
                "The class implementing this interface can ",
                "This class ",
                "This interface ",
                "This interface class ",
            };

        private static readonly string[] DefaultPhrases =
            {
                "A default impl",
                "A default-impl",
                "Default impl",
                "Default-impl",
                "The default impl",
                "The default-impl",
            };

        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMapEntries().ToDictionary(_ => _.Key, _ => _.Value);

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
                var text = t.WithoutXmlCommentExterior();

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
                var text = t.WithoutXmlCommentExterior();

                if (text.IsNullOrWhiteSpace() && content.Count > 1 && IsSeeCref(content[1]))
                {
                    return Inheritdoc();
                }

                if (text.StartsWithAny(DefaultPhrases, StringComparison.OrdinalIgnoreCase))
                {
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

        private static IEnumerable<KeyValuePair<string, string>> CreateReplacementMapEntries()
        {
            foreach (var phrase in StartingTerms.SelectMany(CreatePhrase))
            {
                yield return phrase;
            }

            yield return new KeyValuePair<string, string>("Class that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Class that will represent ", "Represents ");
            yield return new KeyValuePair<string, string>("Class which serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Class which will represent ", "Represents ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces will provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces, will provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Command ", "Represents a command ");
            yield return new KeyValuePair<string, string>("Contain ", "Provides ");
            yield return new KeyValuePair<string, string>("Contains ", "Provides ");
            yield return new KeyValuePair<string, string>("Event argument for ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event argument that is used in the ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event argument that provides information ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event argument which is used in the ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event argument which provides information ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event arguments for ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event arguments that provide information ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event arguments which provide information ", "Provides data for the ");
            yield return new KeyValuePair<string, string>("Event is fired ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event that is published ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event that is published, ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event which is published ", "Occurs ");
            yield return new KeyValuePair<string, string>("Event which is published, ", "Occurs ");
            yield return new KeyValuePair<string, string>("Every class that implements this interface can ", "Allows to ");
            yield return new KeyValuePair<string, string>("Extension of ", "Extends the ");
            yield return new KeyValuePair<string, string>("Factory for ", "Provides support for creating ");
            yield return new KeyValuePair<string, string>("Implementation of ", "Provides a ");
            yield return new KeyValuePair<string, string>("Interface that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Interface which serves ", "Provides ");
            yield return new KeyValuePair<string, string>("The class offers ", "Provides ");
            yield return new KeyValuePair<string, string>("The interface offers ", "Provides ");
        }

        private static IEnumerable<KeyValuePair<string, string>> CreatePhrase(string start)
        {
            foreach (var verb in Verbs)
            {
                var passiveVerb = PassiveVerbs[verb];
                var fix = passiveVerb + " ";

                yield return new KeyValuePair<string, string>(start + verb + " ", fix);
                yield return new KeyValuePair<string, string>(start + verb.ToLowerCaseAt(0) + " ", fix);
                yield return new KeyValuePair<string, string>(start + passiveVerb.ToLowerCaseAt(0) + " ", fix);
                yield return new KeyValuePair<string, string>(start + GerundVerbs[verb].ToLowerCaseAt(0) + " ", fix);
            }
        }
    }
}