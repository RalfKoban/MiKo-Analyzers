using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2074_ContainsParameterDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2074";

        private static readonly string[] EndingPhrases = { " to seek.", " to locate.", " to seek", " to locate" };

        private static readonly string[] MiddlePhrases = { " to seek in the ", " to locate in the" };

        public MiKo_2074_ContainsParameterDefaultPhraseAnalyzer() : base(Id) => IgnoreEmptyParameters = false;

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.Parameters.Length > 0 && method.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter)
        {
            if (parameter.ContainingSymbol is IMethodSymbol method)
            {
                var expectedIndex = method.IsExtensionMethod ? 1 : 0;

                return method.Parameters.IndexOf(parameter) == expectedIndex;
            }

            return false;
        }

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            var text = parameterComment.GetTextTrimmed();

            if (text.EndsWithAny(EndingPhrases) || text.ContainsAny(MiddlePhrases))
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), EndingPhrases[0], CreatePhraseProposal(EndingPhrases[0])) };
        }
    }
}