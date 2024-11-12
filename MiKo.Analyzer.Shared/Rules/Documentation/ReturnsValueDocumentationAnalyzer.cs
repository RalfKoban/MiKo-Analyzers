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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnsVoid is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType() != null && base.ShallAnalyze(symbol);

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeReturnType(symbol, symbol.ReturnType, comment, commentXml);
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeReturnType(symbol, symbol.GetReturnType(), comment, commentXml);
        }

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

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

                foreach (var returnComment in CommentExtensions.GetComments(commentXml, XmlTag.Returns).WhereNotNull())
                {
                    foreach (var finding in AnalyzeReturnType(owningSymbol, returnType, comment, returnComment, XmlTag.Returns))
                    {
                        foundIssues = true;

                        yield return finding;
                    }
                }

                if (foundIssues is false)
                {
                    foreach (var valueComment in CommentExtensions.GetComments(commentXml, XmlTag.Value).WhereNotNull())
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