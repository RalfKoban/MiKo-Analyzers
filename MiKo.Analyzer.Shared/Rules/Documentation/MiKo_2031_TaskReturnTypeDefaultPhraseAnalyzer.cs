﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2031";

        public MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType) => returnType.IsTask();

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, XmlElementSyntax comment, string xmlTag)
        {
            switch (owningSymbol?.Name)
            {
                case nameof(Task.FromCanceled): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.FromCanceledTaskReturnTypeStartingPhrase);
                case nameof(Task.FromException): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.FromExceptionTaskReturnTypeStartingPhrase);
                case nameof(Task.FromResult): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.FromResultTaskReturnTypeStartingPhrase);
                case nameof(Task.ContinueWith): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.ContinueWithTaskReturnTypeStartingPhrase);
                case nameof(Task.Run): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.RunTaskReturnTypeStartingPhrase);
                case nameof(Task.WhenAll): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.WhenAllTaskReturnTypeStartingPhrase);
                case nameof(Task.WhenAny): return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.WhenAnyTaskReturnTypeStartingPhrase);
                default: return AnalyzeDefaultReturnType(owningSymbol, returnType, comment, xmlTag);
            }
        }

        private static bool GenericTypeAccepted(ITypeSymbol returnType)
        {
            if (returnType.IsBoolean())
            {
                return false; // checked by MiKo_2032
            }

            if (returnType.IsString())
            {
                return false; // checked by MiKo_2033
            }

            if (returnType.IsEnum())
            {
                return false; // checked by MiKo_2034
            }

            if (returnType.IsEnumerable())
            {
                return false; // checked by MiKo_2035
            }

            return true;
        }

        private IEnumerable<Diagnostic> AnalyzeDefaultReturnType(ISymbol owningSymbol, ITypeSymbol returnType, XmlElementSyntax comment, string xmlTag)
        {
            if (returnType.TryGetGenericArgumentType(out var argumentType))
            {
                // we have a generic task
                return GenericTypeAccepted(argumentType)
                           ? AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, Constants.Comments.GenericTaskReturnTypeStartingPhrase)
                           : Enumerable.Empty<Diagnostic>();
            }

            return AnalyzePhrase(owningSymbol, comment, xmlTag, Constants.Comments.NonGenericTaskReturnTypePhrase);
        }
    }
}