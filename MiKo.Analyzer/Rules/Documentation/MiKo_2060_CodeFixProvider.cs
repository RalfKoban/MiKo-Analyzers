using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2060_CodeFixProvider)), Shared]
    public sealed class MiKo_2060_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2060_FactoryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2060_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var summary = (XmlElementSyntax)syntax;

            foreach (var ancestor in summary.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ClassDeclarationSyntax _:
                        return CommentStartingWith(summary, Constants.Comments.FactorySummaryPhrase);

                    case MethodDeclarationSyntax m:
                    {
                        var template = Constants.Comments.FactoryCreateMethodSummaryStartingPhraseTemplate;
                        var returnType = m.ReturnType;

                        if (returnType is GenericNameSyntax g && g.TypeArgumentList.Arguments.Count == 1)
                        {
                            template = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhraseTemplate;
                            returnType = g.TypeArgumentList.Arguments[0];
                        }

                        var parts = string.Format(template, '|').Split('|');

                        return CommentStartingWith(summary, parts[0], SeeCref(returnType), parts[1]);
                    }
                }
            }

            return summary;
        }
    }
}