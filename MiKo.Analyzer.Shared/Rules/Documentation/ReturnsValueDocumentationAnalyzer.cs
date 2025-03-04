using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using static MiKoSolutions.Analyzers.Constants;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var issues = AnalyzeComment(comment, context.ContainingSymbol);

                if (issues.Count > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnsVoid is false;

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType() != null;

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected Diagnostic[] AnalyzeStartingPhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, string[] phrase)
        {
            if (commentXml.StartsWithAny(phrase, StringComparison.Ordinal) is false)
            {
                var nodes = comment.GetXmlSyntax(xmlTag);

                if (nodes.Count > 0)
                {
                    var symbolName = symbol.Name;
                    var text = phrase[0];

                    return nodes.Select(_ => Issue(symbolName, _.GetContentsLocation(), xmlTag, text)).ToArray();
                }
            }

            return Array.Empty<Diagnostic>();
        }

        protected Diagnostic[] AnalyzePhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, params string[] phrase)
        {
            if (phrase.None(_ => _.Equals(commentXml, StringComparison.Ordinal)))
            {
                var nodes = comment.GetXmlSyntax(xmlTag);

                if (nodes.Count > 0)
                {
                    var symbolName = symbol.Name;
                    var text = phrase[0];

                    return nodes.Select(_ => Issue(symbolName, _.GetContentsLocation(), xmlTag, text)).ToArray();
                }
            }

            return Array.Empty<Diagnostic>();
        }

        protected virtual Diagnostic[] AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag) => Array.Empty<Diagnostic>();

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
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

                foreach (var returnComment in CommentExtensions.GetComments(commentXml, XmlTag.Returns).WhereNotNull())
                {
                    var findings = AnalyzeReturnType(owningSymbol, returnType, comment, returnComment, XmlTag.Returns);
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

                if (issues is null)
                {
                    foreach (var valueComment in CommentExtensions.GetComments(commentXml, XmlTag.Value).WhereNotNull())
                    {
                        var findings = AnalyzeReturnType(owningSymbol, returnType, comment, valueComment, XmlTag.Value);
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

                if (issues != null)
                {
                    return issues;
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}