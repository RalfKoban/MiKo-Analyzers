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

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            List<Diagnostic> results = null;

            foreach (var nodeOrToken in comment.AllDescendantNodesAndTokens())
            {
                if (nodeOrToken.IsNode)
                {
                    switch (nodeOrToken.AsNode())
                    {
                        case XmlEmptyElementSyntax emptyElement when emptyElement.SlashGreaterThanToken.IsMissing:
                        {
                            if (results is null)
                            {
                                results = new List<Diagnostic>(1);
                            }

                            results.Add(Issue(emptyElement.LessThanToken));

                            break;
                        }

                        case XmlElementSyntax element:
                        {
                            var name = element.StartTag.GetName();

                            if (name.IsNullOrWhiteSpace())
                            {
                                if (results is null)
                                {
                                    results = new List<Diagnostic>(1);
                                }

                                results.Add(Issue(element.StartTag));
                            }

                            break;
                        }
                    }
                }
                else if (nodeOrToken.IsToken)
                {
                    var token = nodeOrToken.AsToken();

                    if (token.IsKind(SyntaxKind.XmlEntityLiteralToken))
                    {
                        var text = token.Text.AsCachedBuilder().Without(XmlEntities).ToStringAndRelease();

                        if (text.Contains('&'))
                        {
                            if (results is null)
                            {
                                results = new List<Diagnostic>(1);
                            }

                            results.Add(Issue(token));
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}