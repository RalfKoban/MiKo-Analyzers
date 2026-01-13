using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2021_CodeFixProvider)), Shared]
    public sealed class MiKo_2021_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("Reference to a "),
                                                            new Pair("Reference to an "),
                                                            new Pair("Reference to the "),
                                                            new Pair("Determines the "),
                                                            new Pair("Determines to ", "value to "),
                                                            new Pair("Either a "),
                                                            new Pair("Either an "),
                                                            new Pair("Either the "),
                                                        };

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap);

        public override string FixableDiagnosticId => "MiKo_2021";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            var preparedComment = PrepareComment(comment);

            return CommentStartingWith(preparedComment, "The ");
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);
    }
}