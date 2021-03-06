﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2014_CodeFixProvider)), Shared]
    public sealed class MiKo_2014_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2014_DisposeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2014_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var method = comment.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), MiKo_2014_DisposeDefaultPhraseAnalyzer.SummaryPhrase).WithTrailingXmlComment();

            var parameters = method.ParameterList.Parameters;
            if (parameters.Count == 0)
            {
                return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
            }

            var param = ParameterComment(parameters[0], MiKo_2014_DisposeDefaultPhraseAnalyzer.ParameterPhrase).WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param);
        }
    }
}