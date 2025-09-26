using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2235_CodeFixProvider)), Shared]
    public sealed class MiKo_2235_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("'s going to", " will"),
                                                            new Pair("'re going to", " will"),
                                                            new Pair("is going to", "will"),
                                                            new Pair("are going to", "will"),
                                                            new Pair("going to", "will"),

                                                            // upper-case terms
                                                            new Pair("Is going to", "Will"),
                                                            new Pair("Are going to", "Will"),
                                                            new Pair("Going to", "Will"),
                                                        };

        public override string FixableDiagnosticId => "MiKo_2235";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);
    }
}