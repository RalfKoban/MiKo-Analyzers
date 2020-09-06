using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2001_CodeFixProvider)), Shared]
    public sealed class MiKo_2001_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = MiKo_2001_EventSummaryAnalyzer.Phrase;

        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "This event is fired ", Phrase },
                                                                                    { "This event is raised ", Phrase },
                                                                                    { "This event occurs ", Phrase },
                                                                                    { "Event is fired ", Phrase },
                                                                                    { "Event is raised ", Phrase },
                                                                                    { "Event occurs ", Phrase },
                                                                                    { "Is fired ", Phrase },
                                                                                    { "Is raised ", Phrase },
                                                                                    { "Fired ", Phrase },
                                                                                    { "Raised ", Phrase },
                                                                                    { "Indicates that ", Phrase + "when " },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2001_EventSummaryAnalyzer.Id;

        protected override string Title => "Start comment with '" + Phrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var fixedComment = Comment((XmlElementSyntax)syntax, ReplacementMap.Select(_ => _.Key).ToList(), ReplacementMap);
            return fixedComment;
        }
    }
}