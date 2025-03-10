using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2000";

        private static readonly string[] XmlEntities = { "&amp;", "&lt;", "&gt;" };

        public MiKo_2000_MalformedDocumentationAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var nodeOrToken in comment.AllDescendantNodesAndTokens())
            {
                Diagnostic issue = null;

                if (nodeOrToken.IsNode)
                {
                    issue = FindIssue(nodeOrToken.AsNode());
                }
                else if (nodeOrToken.IsToken)
                {
                    issue = FindIssue(nodeOrToken.AsToken());
                }

                if (issue is null)
                {
                    continue;
                }

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(issue);
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private Diagnostic FindIssue(SyntaxNode node)
        {
            switch (node)
            {
                case XmlEmptyElementSyntax ee when ee.SlashGreaterThanToken.IsMissing:
                {
                    return Issue(ee.LessThanToken);
                }

                case XmlElementSyntax e:
                {
                    var name = e.StartTag.GetName();

                    return name.IsNullOrWhiteSpace() ? Issue(e.StartTag) : null;
                }

                default:
                    return null;
            }
        }

        private Diagnostic FindIssue(SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlEntityLiteralToken))
            {
                var text = token.Text.AsCachedBuilder().Without(XmlEntities).ToStringAndRelease();

                if (text.Contains('&'))
                {
                    return Issue(token);
                }
            }

            return null;
        }
    }
}