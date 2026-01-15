using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2236_CodeFixProvider)), Shared]
    public sealed class MiKo_2236_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("e.g.", "for example"),
                                                            new Pair("i.e.", "for example"),
                                                            new Pair("p.ex.", "for example"),
                                                            new Pair("e. g.", "for example"),
                                                            new Pair("i. e.", "for example"),
                                                            new Pair("p. ex.", "for example"),
                                                            new Pair("eg.", "for example"),

                                                            // upper-case terms
                                                            new Pair("E.g.", "For example"),
                                                            new Pair("I.e.", "For example"),
                                                            new Pair("P.ex.", "For example"),
                                                            new Pair("E. g.", "For example"),
                                                            new Pair("I. e.", "For example"),
                                                            new Pair("P. ex.", "For example"),
                                                            new Pair("Eg.", "For example"),
                                                        };

        public override string FixableDiagnosticId => "MiKo_2236";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);
    }
}