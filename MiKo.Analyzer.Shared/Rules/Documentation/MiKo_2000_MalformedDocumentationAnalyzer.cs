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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var nodeOrToken in comment.AllDescendantNodesAndTokens())
            {
                if (nodeOrToken.IsNode)
                {
                    if (nodeOrToken.AsNode() is XmlEmptyElementSyntax element && element.SlashGreaterThanToken.IsMissing)
                    {
                        yield return Issue(element.LessThanToken);
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
                            yield return Issue(token);
                        }
                    }
                }
            }
        }
    }
}