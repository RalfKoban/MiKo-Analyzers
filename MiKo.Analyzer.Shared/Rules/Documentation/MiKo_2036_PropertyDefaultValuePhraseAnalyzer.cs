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

        private const string BooleanKey = "IsBoolean";

        public MiKo_2036_PropertyDefaultValuePhraseAnalyzer() : base(Id)
        {
        }

        internal static bool IsBooleanIssue(Diagnostic diagnostic) => diagnostic.Properties.ContainsKey(BooleanKey);

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

            var properties = new Dictionary<string, string>();

            string proposedEndingPhrase;
            IEnumerable<string> endingPhrases;

            if (returnType.IsBoolean())
            {
                properties.Add(BooleanKey, string.Empty);

                proposedEndingPhrase = Constants.Comments.DefaultLangwordPhrase.FormatWith("...");

                endingPhrases = Constants.Comments.DefaultBooleanLangwordPhrases;
            }
            else
            {
                proposedEndingPhrase = Constants.Comments.DefaultCrefPhrase.FormatWith("...");

                endingPhrases = returnType.GetFields()
                                          .SelectMany(_ => Constants.Comments.DefaultCrefPhrases, (symbol, phrase) => phrase.FormatWith(symbol));
            }

            if (commentXml.EndsWithAny(endingPhrases, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(owningSymbol, xmlTag, proposedEndingPhrase, Constants.Comments.NoDefaultPhrase, properties) };
        }
    }
}