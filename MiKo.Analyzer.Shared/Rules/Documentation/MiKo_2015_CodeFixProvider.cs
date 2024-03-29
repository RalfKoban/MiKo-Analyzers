using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2015_CodeFixProvider)), Shared]
    public sealed class MiKo_2015_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> EventReplacementMap = new Dictionary<string, string>
                                                                                     {
                                                                                         { "fired", "raised" },
                                                                                         { "fires", "raises" },
                                                                                         { "firing", "raising" },
                                                                                         { "fire", "raise" },
                                                                                         { "Fired", "Raised" },
                                                                                         { "Fires", "Raises" },
                                                                                         { "Firing", "Raising" },
                                                                                         { "Fire", "Raise" },
                                                                                     };

        private static readonly Dictionary<string, string> ExceptionReplacementMap = new Dictionary<string, string>
                                                                                         {
                                                                                             { "fired", "thrown" },
                                                                                             { "fires", "throws" },
                                                                                             { "firing", "throwing" },
                                                                                             { "fire", "throw" },
                                                                                             { "Fired", "Thrown" },
                                                                                             { "Fires", "Throws" },
                                                                                             { "Firing", "Throwing" },
                                                                                             { "Fire", "Throw" },
                                                                                         };

        public override string FixableDiagnosticId => "MiKo_2015";

        protected override string Title => Resources.MiKo_2015_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var map = GetMap(syntax);

            return Comment(syntax, map.Keys, map);
        }

        private static Dictionary<string, string> GetMap(SyntaxNode syntax)
        {
            var txt = syntax.ToString();

            // inspect comment for 'event' or exception
            return txt.Contains("xception") ? ExceptionReplacementMap : EventReplacementMap;
        }
    }
}