using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2245_CodeFixProvider)), Shared]
    public sealed class MiKo_2245_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2245";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlTextSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is XmlTextSyntax text)
            {
                return root.ReplaceNode(syntax, GetReplacements(text));
            }

            return root;
        }

        private static List<SyntaxNode> GetReplacements(XmlTextSyntax node)
        {
            var textTokens = node.TextTokens;
            var result = new List<SyntaxNode>(textTokens.Count * 2);

            var newLineTokenJustSkipped = false;

            foreach (var textToken in textTokens)
            {
                // special handling of new lines
                if (textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    // keep new line
                    result.Add(XmlText().WithLeadingXmlComment());

                    // we do not need to inspect further
                    newLineTokenJustSkipped = true;

                    continue;
                }

                var valueText = textToken.ValueText;
                var text = valueText.AsSpan();

                // get rid of leading whitespace characters caused by '/// '
                if (newLineTokenJustSkipped)
                {
                    text = text.TrimStart();
                }

                newLineTokenJustSkipped = false;

                var numbers = valueText.GetNumbers();

                if (numbers.Length is 0)
                {
                    result.Add(XmlText(text.ToString()));

                    // we do not need to inspect further as there is no number
                    continue;
                }

                var parts = text.SplitBy(numbers.ToHashSet(_ => _).OrderDescendingByLengthAndText());

                foreach (var part in parts)
                {
                    if (part.IsNumber())
                    {
                        result.Add(C(part));
                    }
                    else
                    {
                        result.Add(XmlText(part));
                    }
                }
            }

            return result;
        }
    }
}