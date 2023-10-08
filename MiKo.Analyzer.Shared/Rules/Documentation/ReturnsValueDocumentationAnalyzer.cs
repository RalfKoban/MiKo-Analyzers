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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeComment(symbol, comment, commentXml);
        }

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected IEnumerable<Diagnostic> AnalyzeComment(IPropertySymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml)
        {
            var returnType = symbol.GetReturnType();

            if (returnType != null)
            {
                return AnalyzeReturnType(symbol, returnType, comment, commentXml);
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;

            if (method.ReturnsVoid)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return AnalyzeReturnType(method, method.ReturnType, comment, commentXml);
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, string[] phrase)
        {
            if (commentXml.StartsWithAny(phrase, StringComparison.Ordinal) is false)
            {
                foreach (var node in comment.GetXmlSyntax(xmlTag))
                {
                    yield return Issue(symbol.Name, node.GetContentsLocation(), xmlTag, phrase[0]);
                }
            }
        }

        protected IEnumerable<Diagnostic> AnalyzePhrase(ISymbol symbol, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag, params string[] phrase)
        {
            if (phrase.None(_ => _.Equals(commentXml, StringComparison.Ordinal)))
            {
                foreach (var node in comment.GetXmlSyntax(xmlTag))
                {
                    yield return Issue(symbol.Name, node.GetContentsLocation(), xmlTag, phrase[0]);
                }
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag) => Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml)
        {
            if (ShallAnalyzeReturnType(returnType))
            {
                var foundIssues = false;

                foreach (var returnComment in CommentExtensions.GetComments(commentXml, XmlTag.Returns).Where(_ => _ != null))
                {
                    foreach (var finding in AnalyzeReturnType(owningSymbol, returnType, comment, returnComment, XmlTag.Returns))
                    {
                        foundIssues = true;

                        yield return finding;
                    }
                }

                if (foundIssues is false)
                {
                    foreach (var valueComment in CommentExtensions.GetComments(commentXml, XmlTag.Value).Where(_ => _ != null))
                    {
                        foreach (var finding in AnalyzeReturnType(owningSymbol, returnType, comment, valueComment, XmlTag.Value))
                        {
                            yield return finding;
                        }
                    }
                }
            }
        }
    }
}