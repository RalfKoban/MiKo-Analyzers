using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2045";

        private static readonly HashSet<string> InvalidTags = new HashSet<string>
                                                                  {
                                                                      Constants.XmlTag.Param,
                                                                      Constants.XmlTag.ParamRef,
                                                                  };

        public MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.Kind == SymbolKind.Method;

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var method = (IMethodSymbol)symbol;

            if (method.Parameters.Length == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            foreach (var node in comment.GetSummaryXmls(InvalidTags))
            {
                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(node));
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}