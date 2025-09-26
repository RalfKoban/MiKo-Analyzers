using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2015_CodeFixProvider)), Shared]
    public sealed class MiKo_2015_CodeFixProvider : XmlTextDocumentationCodeFixProvider
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

        public override string FixableDiagnosticId => "MiKo_2015";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var text = syntax.GetTextTrimmed();

            // inspect comment for 'event' or exception
            var map = text.Contains("xception") ? ExceptionReplacementMap : EventReplacementMap;

            return GetUpdatedSyntax(syntax, issue, map);
        }
    }
}