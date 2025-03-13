using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2078";

        public MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var codeTags = comment.GetXmlSyntax(Constants.XmlTag.Code);
            var codeTagsCount = codeTags.Count;

            if (codeTagsCount > 0)
            {
                for (var index = 0; index < codeTagsCount; index++)
                {
                    var code = codeTags[index];

                    foreach (var entry in code.Content)
                    {
                        if (entry.IsXml())
                        {
                            // we have an issue
                            return new[] { Issue(entry) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}