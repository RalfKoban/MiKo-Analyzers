﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1090_CodeFixProvider)), Shared]
    public sealed class MiKo_1090_CodeFixProvider : ParameterNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1090";

        protected override string Title => Resources.MiKo_1090_CodeFixTitle;

        protected override string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => MiKo_1090_ParametersWrongSuffixedAnalyzer.FindBetterName(symbol);
    }
}