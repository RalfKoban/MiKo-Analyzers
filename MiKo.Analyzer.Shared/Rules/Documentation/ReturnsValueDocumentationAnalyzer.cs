﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, symbol.GetDocumentationCommentXml());

        protected virtual bool ShallAnalyzeReturnType(ITypeSymbol returnType) => true;

        protected IEnumerable<Diagnostic> AnalyzeComment(IPropertySymbol symbol, string commentXml)
        {
            var returnType = symbol.GetReturnType();

            return returnType != null
                       ? AnalyzeReturnType(symbol, returnType, commentXml)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var method = (IMethodSymbol)symbol;

            if (method.ReturnsVoid)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return AnalyzeReturnType(method, method.ReturnType, commentXml);
        }

        protected IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, string comment, string xmlTag, string[] phrase) => comment.StartsWithAny(phrase, StringComparison.Ordinal)
                                                                                                                                       ? Enumerable.Empty<Diagnostic>()
                                                                                                                                       : new[] { Issue(symbol, xmlTag, phrase[0]) };

        protected IEnumerable<Diagnostic> AnalyzePhrase(ISymbol symbol, string comment, string xmlTag, params string[] phrase) => phrase.Any(_ => _.Equals(comment, StringComparison.Ordinal))
                                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                                    : new[] { Issue(symbol, xmlTag, phrase[0]) };

        protected virtual IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag) => Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string commentXml)
        {
            if (ShallAnalyzeReturnType(returnType))
            {
                var foundIssues = false;

                foreach (var comment in CommentExtensions.GetComments(commentXml, XmlTag.Returns).Where(_ => _ != null))
                {
                    foreach (var finding in AnalyzeReturnType(owningSymbol, returnType, comment, XmlTag.Returns))
                    {
                        foundIssues = true;

                        yield return finding;
                    }
                }

                if (foundIssues is false)
                {
                    foreach (var comment in CommentExtensions.GetComments(commentXml, XmlTag.Value).Where(_ => _ != null))
                    {
                        foreach (var finding in AnalyzeReturnType(owningSymbol, returnType, comment, XmlTag.Value))
                        {
                            yield return finding;
                        }
                    }
                }
            }
        }
    }
}