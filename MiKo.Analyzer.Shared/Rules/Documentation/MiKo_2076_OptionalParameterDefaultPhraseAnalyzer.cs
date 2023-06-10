using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2076_OptionalParameterDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2076";

        public MiKo_2076_OptionalParameterDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Any(_ => _.IsOptional) && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            const string Phrase = Constants.Comments.DefaultStartingPhrase;

            if (parameter.IsOptional && parameterComment.GetTextTrimmed().Contains(Phrase, StringComparison.Ordinal) is false)
            {
                yield return Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrase);
            }
        }
    }
}