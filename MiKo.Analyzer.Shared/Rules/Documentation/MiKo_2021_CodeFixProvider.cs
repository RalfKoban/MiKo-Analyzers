using System;
using System.Composition;
using System.Linq;

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
                                                            new Pair("Determines to ", "value to "), // TODO RKN: new Pair("Determines to ", "value to "),
                                                        };

        private static readonly string[] ReplacementMapKeys = ReplacementMap.Select(_ => _.Key).ToArray();

        public override string FixableDiagnosticId => "MiKo_2021";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue)
        {
            var preparedComment = PrepareComment(comment);

            return CommentStartingWith(preparedComment, "The ");
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMapKeys, ReplacementMap);
    }
}