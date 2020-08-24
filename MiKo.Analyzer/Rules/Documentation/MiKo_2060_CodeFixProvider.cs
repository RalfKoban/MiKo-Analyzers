using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2060_CodeFixProvider)), Shared]
    public sealed class MiKo_2060_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2060_FactoryAnalyzer.Id;

        protected override string Title => "Apply default comment to factory";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var summary = (XmlElementSyntax)syntax;

            foreach (var ancestor in summary.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ClassDeclarationSyntax _:
                        return StartCommentWith(summary, Constants.Comments.FactorySummaryPhrase);

                    case MethodDeclarationSyntax m:
                    {
                        var template = Constants.Comments.FactoryCreateMethodSummaryStartingPhraseTemplate;
                        var returnValue = m.ReturnType;

                        if (returnValue is GenericNameSyntax g)
                        {
                            template = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhraseTemplate;
                            returnValue = SyntaxFactory.ParseTypeName(g.Identifier.ValueText);
                        }

                        var parts = string.Format(template, '|').Split('|');

                        return StartCommentWith(summary, parts[0], SeeCref(returnValue), parts[1]);
                    }
                }
            }

            return summary;
        }
    }
}