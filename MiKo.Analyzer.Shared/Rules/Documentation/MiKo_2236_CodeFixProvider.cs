using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2236_CodeFixProvider)), Shared]
    public sealed class MiKo_2236_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("e.g.", "for example"),
                                                            new Pair("i.e.", "for example"),
                                                            new Pair("e. g.", "for example"),
                                                            new Pair("i. e.", "for example"),
                                                            new Pair("eg.", "for example"),

                                                            // upper-case terms
                                                            new Pair("E.g.", "For example"),
                                                            new Pair("I.e.", "For example"),
                                                            new Pair("E. g.", "For example"),
                                                            new Pair("I. e.", "For example"),
                                                            new Pair("Eg.", "For example"),
                                                        };

        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key);

        public override string FixableDiagnosticId => "MiKo_2236";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue) => Comment(syntax, ReplacementMapKeys, ReplacementMap);
    }
}