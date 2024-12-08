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

        public MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var code in comment.GetXmlSyntax(Constants.XmlTag.Code))
            {
                foreach (var entry in code.Content)
                {
                    if (entry.IsXml())
                    {
                        // we have an issue
                        return new[] { Issue(symbol.Name, entry) };
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}