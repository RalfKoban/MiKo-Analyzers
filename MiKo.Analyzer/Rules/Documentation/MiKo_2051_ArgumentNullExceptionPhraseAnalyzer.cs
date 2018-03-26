using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2051_ArgumentNullExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2051";

        public MiKo_2051_ArgumentNullExceptionPhraseAnalyzer() : base(Id, typeof(ArgumentNullException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment)
        {
            switch (symbol)
            {
                case IMethodSymbol method: return AnalyzeException(method, method, exceptionComment);
                case IPropertySymbol property: return AnalyzeException(property, property.SetMethod, exceptionComment);
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IMethodSymbol methodSymbol, string exceptionComment)
        {
            if (methodSymbol is null) return Enumerable.Empty<Diagnostic>();
            if (exceptionComment is null) return Enumerable.Empty<Diagnostic>();

            var parameters = methodSymbol.Parameters.Where(_ => _.Type.TypeKind != TypeKind.Struct).ToList();
            if (!parameters.Any()) return Enumerable.Empty<Diagnostic>();

            // create default proposal for parameter names
            var proposal = parameters
                                   .Select(_ => string.Format(Constants.Comments.ArgumentNullExceptionStartingPhrase[0], _.Name) + Environment.NewLine)
                                   .ConcatenatedWith(Constants.Comments.ExceptionSplittingParaPhrase + Environment.NewLine);

            // remove -or- separators and split comment into parts to inspect individually
            var parts = exceptionComment.Split(Constants.Comments.ExceptionSplittingPhrase, StringSplitOptions.RemoveEmptyEntries);

            var results = from parameter in parameters
                          let parameterIndicator = string.Format(Constants.Comments.ParamRefBeginningPhrase, parameter.Name)
                          let phrases = Constants.Comments.ArgumentNullExceptionStartingPhrase.Select(_ => string.Format(_, parameter.Name)).ToArray()
                          where !parts.Where(_ => _.Contains(parameterIndicator))
                                      .Select(_ => _.Trim()) // get rid of white-spaces
                                      .Any(_ => _.EqualsAny(StringComparison.Ordinal, phrases))
                          select ReportIssue(owningSymbol, ExceptionPhrase, proposal);

            return results.ToList();
        }
    }
}