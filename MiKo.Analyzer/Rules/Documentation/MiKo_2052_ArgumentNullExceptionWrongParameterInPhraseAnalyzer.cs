using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2052_ArgumentNullExceptionWrongParameterInPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2052";

        public MiKo_2052_ArgumentNullExceptionWrongParameterInPhraseAnalyzer() : base(Id, typeof(ArgumentNullException))
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

            var parameters = methodSymbol.Parameters.Where(_ => _.Type.IsValueType && !_.Type.IsNullable());
            return parameters.Any()
                       ? AnalyzeException(owningSymbol, parameters, exceptionComment)
                       : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeException(ISymbol owningSymbol, IEnumerable<IParameterSymbol> parameters, string exceptionComment)
        {
            var parameterIndicators = parameters.ToDictionary(_ => _, _ => string.Format(Constants.Comments.ParamRefBeginningPhrase, _.Name));

            return parameterIndicators
                   .Where(_ => exceptionComment.Contains(_.Value))
                   .Select(_ => ReportIssue(owningSymbol, _.Key.Name, _.Value + Constants.Comments.XmlElementEndingTag))
                   .ToList();
        }
    }
}