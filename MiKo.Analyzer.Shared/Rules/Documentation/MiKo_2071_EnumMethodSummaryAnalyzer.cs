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

        private static readonly string[] BooleanPhrases = new[] { " indicating ", " indicates ", " indicate " }.SelectMany(_ => ContinuationPhrases, string.Concat).ToArray();

        private static readonly int MinimumPhraseLength = BooleanPhrases.Min(_ => _.Length);

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
                if (token.ValueText.Length < MinimumPhraseLength)
                {
                    continue;
                }

                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(token, BooleanPhrases, StringComparison.Ordinal, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(locations[index]);
                    }
                }
            }
        }
    }
}