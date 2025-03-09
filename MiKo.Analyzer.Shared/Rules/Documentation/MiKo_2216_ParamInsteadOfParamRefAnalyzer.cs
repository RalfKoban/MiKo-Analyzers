using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2216_ParamInsteadOfParamRefAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2216";

        public MiKo_2216_ParamInsteadOfParamRefAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var element in comment.DescendantNodes())
            {
                if (element.Parent is XmlElementSyntax && element.IsXmlTag(Constants.XmlTag.Param))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(element));
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}