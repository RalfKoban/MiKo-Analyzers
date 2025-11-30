using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnsVoid is false;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType() != null;

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected Diagnostic[] AnalyzePhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, string phrase) => AnalyzePhrase(symbol, comment, commentXml, xmlTag, new[] { phrase });

        protected Diagnostic[] AnalyzePhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, in ReadOnlySpan<string> phrases)
        {
            if (phrases.Any(_ => _.Equals(commentXml, StringComparison.Ordinal)))
            {
                return Array.Empty<Diagnostic>();
            }

            var nodes = comment.GetXmlSyntax(xmlTag);

            if (nodes.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var symbolName = symbol.Name;
            var text = phrases[0];

            if (nodes.Count is 1)
            {
                return new[] { Issue(symbolName, nodes[0].GetContentsLocation(), xmlTag, text) };
            }

            return nodes.Select(_ => Issue(symbolName, _.GetContentsLocation(), xmlTag, text))
                        .ToArray();
        }

        protected Diagnostic[] AnalyzeStartingPhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, in ReadOnlySpan<string> phrases)
        {
            if (commentXml.StartsWithAny(phrases))
            {
                return Array.Empty<Diagnostic>();
            }

            var nodes = comment.GetXmlSyntax(xmlTag);

            if (nodes.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var symbolName = symbol.Name;
            var text = phrases[0];

            if (nodes.Count is 1)
            {
                if (StartsWithList(nodes[0]))
                {
                    // ignore list
                    return Array.Empty<Diagnostic>();
                }

                return new[] { Issue(symbolName, nodes[0].GetContentsLocation(), xmlTag, text) };
            }

            return nodes.Where(_ => StartsWithList(_) is false) // ignore list
                        .Select(_ => Issue(symbolName, _.GetContentsLocation(), xmlTag, text))
                        .ToArray();
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            switch (symbol)
            {
                case IMethodSymbol method when ShallAnalyze(method):
                    return AnalyzeComment(comment, method);

                case IPropertySymbol property when ShallAnalyze(property):
                    return AnalyzeComment(comment, property);

                default:
                    return Array.Empty<Diagnostic>();
            }
        }

        protected virtual Diagnostic[] AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag) => Array.Empty<Diagnostic>();

        private static bool StartsWithList(XmlElementSyntax node)
        {
            var contents = node.Content;

            if (contents.Count >= 2)
            {
                return contents[1].GetXmlTagName() is Constants.XmlTag.List && contents[0] is XmlTextSyntax t && t.GetTextTrimmed().IsNullOrEmpty();
            }

            if (contents.Count is 1)
            {
                return contents[0].GetXmlTagName() is Constants.XmlTag.List;
            }

            return false;
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IMethodSymbol symbol)
        {
            return AnalyzeReturnType(symbol, symbol.ReturnType, comment);
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IPropertySymbol symbol)
        {
            return AnalyzeReturnType(symbol, symbol.GetReturnType(), comment);
        }

        private IReadOnlyList<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment)
        {
            if (ShallAnalyzeReturnType(returnType))
            {
                var commentXml = owningSymbol.GetDocumentationCommentXml();

                List<Diagnostic> issues = null;

                Analyze(owningSymbol, returnType, comment, commentXml, Constants.XmlTag.Returns, ref issues);

                if (issues is null)
                {
                    Analyze(owningSymbol, returnType, comment, commentXml, Constants.XmlTag.Value, ref issues);
                }

                if (issues != null)
                {
                    return issues;
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private void Analyze(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string tagName, ref List<Diagnostic> issues)
        {
            foreach (var xml in CommentExtensions.GetComments(commentXml, tagName).WhereNotNull())
            {
                var findings = AnalyzeReturnType(owningSymbol, returnType, comment, xml, tagName);
                var findingsLength = findings.Length;

                if (findingsLength > 0)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(findingsLength);
                    }

                    issues.AddRange(findings);
                }
            }
        }
    }
}