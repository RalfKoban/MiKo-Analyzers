using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2015_CodeFixProvider)), Shared]
    public sealed class MiKo_2015_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private static readonly Pair[] OccurReplacementMap =
                                                             {
                                                                 new Pair("fired", "Occurs when"),
                                                                 new Pair("fires", "Occurs when"),
                                                                 new Pair("firing", "Occurs when"),
                                                                 new Pair("fire", "Occurs when"),
                                                                 new Pair("Fired", "Occurs when"),
                                                                 new Pair("Fires", "Occurs when"),
                                                                 new Pair("Firing", "Occurs when"),
                                                                 new Pair("Fire", "Occurs when"),
                                                                 new Pair("when if", "when"),
                                                                 new Pair("when when", "when"),
                                                             };

        private static readonly Pair[] RaiseReplacementMap =
                                                             {
                                                                 new Pair("fired", "raised"),
                                                                 new Pair("fires", "raises"),
                                                                 new Pair("firing", "raising"),
                                                                 new Pair("fire", "raise"),
                                                                 new Pair("Fired", "Raised"),
                                                                 new Pair("Fires", "Raises"),
                                                                 new Pair("Firing", "Raising"),
                                                                 new Pair("Fire", "Raise"),
                                                             };

        private static readonly Pair[] ThrowReplacementMap =
                                                             {
                                                                 new Pair("fired", "thrown"),
                                                                 new Pair("fires", "throws"),
                                                                 new Pair("firing", "throwing"),
                                                                 new Pair("fire", "throw"),
                                                                 new Pair("Fired", "Thrown"),
                                                                 new Pair("Fires", "Throws"),
                                                                 new Pair("Firing", "Throwing"),
                                                                 new Pair("Fire", "Throw"),
                                                             };

        public override string FixableDiagnosticId => "MiKo_2015";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var map = GetReplacementMap(syntax);

            return GetUpdatedSyntax(syntax, issue, map);
        }

        private static ReadOnlySpan<Pair> GetReplacementMap(XmlTextSyntax syntax)
        {
            var text = syntax.GetTextTrimmed();

            // inspect comment for 'exception'
            if (text.Contains("xception"))
            {
                return ThrowReplacementMap;
            }

            var commentTriviaSyntax = syntax.FirstAncestor<DocumentationCommentTriviaSyntax>();

            switch (commentTriviaSyntax.ParentTrivia.Token.Parent)
            {
                case EventDeclarationSyntax _:
                case EventFieldDeclarationSyntax _:
                    return OccurReplacementMap;

                default:
                    return RaiseReplacementMap;
            }
        }
    }
}