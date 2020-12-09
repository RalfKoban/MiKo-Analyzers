using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2015_CodeFixProvider)), Shared]
    public sealed class MiKo_2015_CodeFixProvider : DocumentationCodeFixProvider
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

        public override string FixableDiagnosticId => MiKo_2015_FireMethodsAnalyzer.Id;

        protected override string Title => Resources.MiKo_2015_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var map = GetMap(syntax);

            return Comment(comment, map.Keys, map);
        }

        private static Dictionary<string, string> GetMap(SyntaxNode syntax)
        {
            var txt = syntax.ToString();

            // inspect comment for 'event' or exception
            return txt.Contains("xception") ? ExceptionReplacementMap : EventReplacementMap;
        }
    }
}