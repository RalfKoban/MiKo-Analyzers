using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2036_PropertyDefaultValuePhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2036";

        public MiKo_2036_PropertyDefaultValuePhraseAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Property);

        protected override bool ShallAnalyze(IPropertySymbol symbol)
        {
            var returnType = symbol.GetReturnType();

            if (returnType is null)
            {
                return false;
            }

            if (returnType.IsBoolean() || returnType.IsEnum())
            {
                return base.ShallAnalyze(symbol);
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            if (commentXml.EndsWith(Constants.Comments.NoDefaultPhrase, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var isBoolean = returnType.IsBoolean();

            var endingPhrases = isBoolean
                                ? Constants.Comments.DefaultBooleanLangwordPhrases
                                : returnType.GetFields()
                                            .SelectMany(_ => Constants.Comments.DefaultCrefPhrases, (symbol, phrase) => phrase.FormatWith(symbol))
                                            .ToArray();

            if (commentXml.EndsWithAny(endingPhrases, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var properties = isBoolean
                             ? new[] { new Pair(Constants.AnalyzerCodeFixSharedData.IsBoolean) }
                             : Array.Empty<Pair>();

            var proposedEndingPhrase = isBoolean ? Constants.Comments.DefaultLangwordPhrase : Constants.Comments.DefaultCrefPhrase;

            return new[] { Issue(owningSymbol, xmlTag, proposedEndingPhrase.FormatWith("..."), Constants.Comments.NoDefaultPhrase, properties) };
        }
    }
}