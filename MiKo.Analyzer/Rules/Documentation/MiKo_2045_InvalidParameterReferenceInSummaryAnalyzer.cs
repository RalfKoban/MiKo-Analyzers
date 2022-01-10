﻿using System.Collections.Generic;
using System.Linq;

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

        public MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        internal static IEnumerable<XmlNodeSyntax> GetIssues(DocumentationCommentTriviaSyntax documentation) => documentation != null
                                                                                                                    ? documentation.GetSummaryXmls(InvalidTags)
                                                                                                                    : Enumerable.Empty<XmlNodeSyntax>();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var method = (IMethodSymbol)symbol;

            if (method.Parameters.Length != 0)
            {
                var documentation = method.GetDocumentationCommentTriviaSyntax();

                foreach (var node in GetIssues(documentation))
                {
                    yield return Issue(node);
                }
            }
        }
    }
}