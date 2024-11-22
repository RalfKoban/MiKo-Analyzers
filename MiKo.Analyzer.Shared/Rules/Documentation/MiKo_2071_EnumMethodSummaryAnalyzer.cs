using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2071_EnumMethodSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2071";

        private static readonly string[] ContinuationPhrases = { "whether ", "if " };

        private static readonly string[] BooleanPhrases = new[] { " indicating ", " indicates ", " indicate " }.SelectMany(term1 => ContinuationPhrases, string.Concat)
                                                                                                               .ToArray();

        public MiKo_2071_EnumMethodSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsEnum() && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType()?.IsEnum() is true && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, BooleanPhrases, StringComparison.Ordinal, Offset, Offset))
                {
                    yield return Issue(location);
                }
            }
        }
    }
}