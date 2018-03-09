using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2033";

        public MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            switch (returnType.Name)
            {
                // ReSharper disable once RedundantNameQualifier
                case nameof(System.String): return true;
                case nameof(System.Threading.Tasks.Task):
                    {
                        if (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length == 1)
                        {
                            // we have a generic task
                            // ReSharper disable once RedundantNameQualifier
                            return namedType.TypeArguments[0].Name == nameof(System.String);
                        }

                        return false;
                    }

                default: return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, string comment, string xmlTag)
        {
            // ReSharper disable once RedundantNameQualifier
            var isString = returnType.Name == nameof(System.String);

            var startingPhrases = isString ? Constants.Comments.StringReturnTypeStartingPhrase : Constants.Comments.StringTaskReturnTypeStartingPhrase;

            return AnalyzeStartingPhrase(owningSymbol, comment, xmlTag, startingPhrases);
        }
    }
}