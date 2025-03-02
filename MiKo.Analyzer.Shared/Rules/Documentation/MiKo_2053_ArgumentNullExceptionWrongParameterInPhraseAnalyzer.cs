﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer : ArgumentExceptionPhraseAnalyzer
    {
        public const string Id = "MiKo_2053";

        public MiKo_2053_ArgumentNullExceptionWrongParameterInPhraseAnalyzer() : base(Id, typeof(ArgumentNullException), false)
        {
        }

        protected override IReadOnlyCollection<IParameterSymbol> GetMatchingParameters(ImmutableArray<IParameterSymbol> parameterSymbols) => parameterSymbols.Where(_ => _.Type.IsValueType && _.Type.IsNullable() is false).ToList();

        protected override IReadOnlyList<Diagnostic> AnalyzeException(ISymbol owningSymbol, IReadOnlyCollection<IParameterSymbol> parameters, XmlElementSyntax exceptionComment)
        {
            List<Diagnostic> issues = null;

            // get rid of the para tags as we are not interested into them
            var comment = exceptionComment.GetTextTrimmed();

            var parameterIndicators = parameters.ToDictionary<IParameterSymbol, IParameterSymbol, string>(_ => _, _ => Constants.Comments.ParamRefBeginningPhrase.FormatWith(_.Name), SymbolEqualityComparer.Default);

            foreach (var indicator in parameterIndicators)
            {
                if (comment.Contains(indicator.Value))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(owningSymbol, indicator.Key.Type.Name, indicator.Value + Constants.Comments.XmlElementEndingTag));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}