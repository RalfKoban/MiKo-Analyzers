﻿using System.Collections.Generic;

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

        internal static IEnumerable<SyntaxNode> GetProblematicElements(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var element in comment.DescendantNodes())
            {
                if (element.IsXmlTag(Constants.XmlTag.Param))
                {
                    if (element.Parent is XmlElementSyntax)
                    {
                        yield return element;
                    }
                }
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            if (comment is null)
            {
                // it might be that there is no documentation comment available
                yield break;
            }

            foreach (var element in GetProblematicElements(comment))
            {
                yield return Issue(element);
            }
        }
    }
}