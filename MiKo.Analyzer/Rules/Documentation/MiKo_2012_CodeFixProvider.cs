using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2012_CodeFixProvider)), Shared]
    public sealed class MiKo_2012_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] Verbs =
            {
                "Allows ",
                "Creates ",
                "Enhances ",
                "Extends ",
                "Generates ",
                "Manipulates ",
                "Offers ",
                "Provides ",
                "Represents ",
            };

        private static readonly string[] StartingTerms =
            {
                "Class that ",
                "Class which ",
                "Factory class that ",
                "Factory class which ",
                "Factory method that ",
                "Factory method which ",
                "Helper class that ",
                "Helper class which ",
                "Interface that ",
                "Interface which ",
                "The class implementing this interface ",
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

            var inheritDoc = comment.Content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Inheritdoc);
            if (inheritDoc != null)
            {
                // special case: its an inherit documentation, so mark it so
                return inheritDoc;
            }

            // maybe it is a docu that should be an inherit documentation instead
            if (comment.Content.FirstOrDefault() is XmlTextSyntax t)
            {
                var text = t.WithoutXmlCommentExterior();

                if (text.StartsWithAny(DefaultPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    return Inheritdoc();
                }
            }

            return Comment(comment, ReplacementMap.Keys, ReplacementMap);
        }

        private static IEnumerable<KeyValuePair<string, string>> CreateReplacementMapEntries()
        {
            foreach (var start in StartingTerms)
            {
                foreach (var verb in Verbs)
                {
                    var term = start + verb.ToLowerCaseAt(0);

                    yield return new KeyValuePair<string, string>(term, verb);
                }
            }

            yield return new KeyValuePair<string, string>("Class that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Class that will represent ", "Represents ");
            yield return new KeyValuePair<string, string>("Class to provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Class which serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Class which will represent ", "Represents ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces will provide ", "Provides ");
            yield return new KeyValuePair<string, string>("Classes implementing the interfaces, will provide ", "Provides ");
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
            yield return new KeyValuePair<string, string>("Helper class to manipulate ", "Manipulates ");
            yield return new KeyValuePair<string, string>("Helper method to generate ", "Generates ");
            yield return new KeyValuePair<string, string>("Implementation of ", "Provides a ");
            yield return new KeyValuePair<string, string>("Interface that serves ", "Provides ");
            yield return new KeyValuePair<string, string>("Interface which serves ", "Provides ");
            yield return new KeyValuePair<string, string>("The class offers ", "Provides ");
            yield return new KeyValuePair<string, string>("The interface offers ", "Provides ");
        }
    }
}