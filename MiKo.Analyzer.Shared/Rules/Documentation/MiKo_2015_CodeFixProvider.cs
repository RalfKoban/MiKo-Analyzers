using System.Collections.Generic;
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
        private static readonly KeyValuePair<string, string>[] EventReplacementMap =
                                                                                     {
                                                                                         new KeyValuePair<string, string>("fired", "raised"),
                                                                                         new KeyValuePair<string, string>("fires", "raises"),
                                                                                         new KeyValuePair<string, string>("firing", "raising"),
                                                                                         new KeyValuePair<string, string>("fire", "raise"),
                                                                                         new KeyValuePair<string, string>("Fired", "Raised"),
                                                                                         new KeyValuePair<string, string>("Fires", "Raises"),
                                                                                         new KeyValuePair<string, string>("Firing", "Raising"),
                                                                                         new KeyValuePair<string, string>("Fire", "Raise"),
                                                                                     };

        private static readonly string[] EventReplacementMapKeys = EventReplacementMap.Select(_ => _.Key).ToArray();

        private static readonly KeyValuePair<string, string>[] ExceptionReplacementMap =
                                                                                         {
                                                                                             new KeyValuePair<string, string>("fired", "thrown"),
                                                                                             new KeyValuePair<string, string>("fires", "throws"),
                                                                                             new KeyValuePair<string, string>("firing", "throwing"),
                                                                                             new KeyValuePair<string, string>("fire", "throw"),
                                                                                             new KeyValuePair<string, string>("Fired", "Thrown"),
                                                                                             new KeyValuePair<string, string>("Fires", "Throws"),
                                                                                             new KeyValuePair<string, string>("Firing", "Throwing"),
                                                                                             new KeyValuePair<string, string>("Fire", "Throw"),
                                                                                         };

        private static readonly string[] ExceptionReplacementMapKeys = ExceptionReplacementMap.Select(_ => _.Key).ToArray();

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