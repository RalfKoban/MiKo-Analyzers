using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2036_PropertyDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2036";

        public MiKo_2036_PropertyDefaultPhraseAnalyzer() : base(Id)
        {
        }

        public override void Initialize(AnalysisContext context) => Initialize(context, SymbolKind.Property);

        protected override bool ShallAnalyzeProperty(IPropertySymbol symbol)
        {
            var returnType = GetReturnType(symbol);
            if (returnType == null) return false;

            return returnType.SpecialType == SpecialType.System_Boolean || returnType.IsEnum();
        }

        private static ITypeSymbol GetReturnType(IPropertySymbol symbol)
        {
            if (symbol.GetMethod != null)
                return symbol.GetMethod.ReturnType;

            if (symbol.SetMethod != null)
                return symbol.SetMethod.Parameters[0].Type;

            return null;
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            string proposedEndingPhrase;
            string[] endingPhrases;

            if (returnType.SpecialType == SpecialType.System_Boolean)
            {
                proposedEndingPhrase = string.Format(Constants.Comments.DefaultLangwordPhrase, "...");
                endingPhrases = Constants.Comments.DefaultBooleanLangwordPhrases;
            }
            else
            {
                proposedEndingPhrase = string.Format(Constants.Comments.DefaultCrefPhrase, "...");
                endingPhrases = returnType.GetMembers()
                                          .OfType<IFieldSymbol>()
                                          .SelectMany(_ => Constants.Comments.DefaultCrefPhrases, (symbol, phrase) => string.Format(phrase, symbol))
                                          .ToArray();
            }

            return comment.EndsWithAny(StringComparison.Ordinal, endingPhrases)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(owningSymbol, owningSymbol.Name, xmlTag, proposedEndingPhrase) };
        }
    }
}