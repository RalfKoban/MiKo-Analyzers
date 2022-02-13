using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ReturnsValueDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ReturnsValueDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, comment);

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected IEnumerable<Diagnostic> AnalyzeComment(IPropertySymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            var returnType = symbol.GetReturnType();

            return returnType != null
                       ? AnalyzeReturnType(symbol, returnType, comment)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;
            if (method.ReturnsVoid)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return AnalyzeReturnType(method, method.ReturnType, comment);
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, XmlElementSyntax comment, string xmlTag, string[] phrase)
        {
            // TODO RKN: fix call
            var xmlComment = comment.GetTextWithoutTrivia();

            if (xmlComment.StartsWithAny(phrase, StringComparison.Ordinal) is false)
            {
                yield return Issue(symbol, xmlTag, phrase[0]);
            }
        }

        protected IEnumerable<Diagnostic> AnalyzePhrase(ISymbol symbol, XmlElementSyntax comment, string xmlTag, params string[] phrase)
        {
            // TODO RKN: fix call
            var xmlComment = comment.GetTextWithoutTrivia();

            if (phrase.None(_ => _.Equals(xmlComment, StringComparison.Ordinal)))
            {
                yield return Issue(symbol, xmlTag, phrase[0]);
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, XmlElementSyntax comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment)
        {
            if (ShallAnalyzeReturnType(returnType) is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return TryAnalyzeReturnType(owningSymbol, returnType, comment, Constants.XmlTag.Returns)
                   ?? TryAnalyzeReturnType(owningSymbol, returnType, comment, Constants.XmlTag.Value)
                   ?? Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> TryAnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string xmlTag)
        {
            foreach (var node in comment.GetXmlSyntax(xmlTag))
            {
                var findings = AnalyzeReturnType(owningSymbol, returnType, node, xmlTag);

                foreach (var finding in findings)
                {
                    yield return finding;
                }
            }
        }
    }
}