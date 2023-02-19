using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2214_DocumentationContainsEmptyLinesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2214";

        public MiKo_2214_DocumentationContainsEmptyLinesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var tokens in comment.DescendantNodes<XmlTextSyntax>().Select(_ => _.TextTokens))
            {
                for (var i = 0; i < tokens.Count - 1; i++)
                {
                    var currentToken = tokens[i];

                    if (currentToken.IsKind(SyntaxKind.XmlTextLiteralToken) && currentToken.ValueText.IsNullOrWhiteSpace())
                    {
                        var nextToken = tokens[i + 1];

                        if (nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            yield return Issue(symbol.Name, nextToken);
                        }
                    }
                }
            }
        }
    }
}