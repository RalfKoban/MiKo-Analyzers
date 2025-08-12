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

        private static readonly string[] Phrases = { " to seek.", " to locate." };

        public MiKo_2074_ContainsParameterDefaultPhraseAnalyzer() : base(Id) => IgnoreEmptyParameters = false;

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.Parameters.Length > 0 && method.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.ContainingSymbol is IMethodSymbol method && method.Parameters.IndexOf(parameter) is 0;

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameterComment.GetTextTrimmed().EndsWithAny(Phrases))
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrases[0], CreatePhraseProposal(Phrases[0])) };
        }
    }
}