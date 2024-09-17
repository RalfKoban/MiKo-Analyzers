using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2015_CodeFixProvider)), Shared]
    public sealed class MiKo_2015_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Pair[] EventReplacementMap =
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

        private static readonly string[] EventReplacementMapKeys = EventReplacementMap.ToArray(_ => _.Key);

        private static readonly Pair[] ExceptionReplacementMap =
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

        private static readonly string[] ExceptionReplacementMapKeys = ExceptionReplacementMap.ToArray(_ => _.Key);

        public override string FixableDiagnosticId => "MiKo_2015";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var text = syntax.ToString();

            // inspect comment for 'event' or exception
            return text.Contains("xception")
                   ? Comment(syntax, ExceptionReplacementMapKeys, ExceptionReplacementMap)
                   : Comment(syntax, EventReplacementMapKeys, EventReplacementMap);
        }
    }
}