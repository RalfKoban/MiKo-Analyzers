using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2032";

        public MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeReturnType(ITypeSymbol returnType)
        {
            switch (returnType.Name)
            {
                // ReSharper disable once RedundantNameQualifier
                case nameof(System.Boolean): return true;
                case nameof(System.Threading.Tasks.Task):
                    {
                        if (returnType is INamedTypeSymbol namedType && namedType.TypeArguments.Length == 1)
                        {
                            // we have a generic task
                            // ReSharper disable once RedundantNameQualifier
                            return namedType.TypeArguments[0].Name == nameof(System.Boolean);
                        }

                        return false;
                    }

                default: return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(IMethodSymbol method, string comment, string xmlTag)
        {
            // ReSharper disable once RedundantNameQualifier
            if (method.ReturnType.Name == nameof(System.Boolean))
            {
                var startingPhrases = Constants.Comments.BooleanReturnTypeStartingPhrase;
                var endingPhrases = Constants.Comments.BooleanReturnTypeEndingPhrase;
                return comment.StartsWithAny(StringComparison.Ordinal, startingPhrases) && comment.EndsWithAny(StringComparison.Ordinal, endingPhrases)
                        ? Enumerable.Empty<Diagnostic>()
                        : new[] { ReportIssue(method, method.Name, xmlTag, startingPhrases[0], endingPhrases[0]) };
            }

            // TODO: analyze Task<bool>
            return Enumerable.Empty<Diagnostic>();
        }
    }
}