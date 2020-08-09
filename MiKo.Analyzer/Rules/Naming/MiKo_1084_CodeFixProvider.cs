﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1084_CodeFixProvider)), Shared]
    public sealed class MiKo_1084_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1084_VariablesWithNumberSuffixAnalyzer.Id;

        protected override string Title => "Remove number";

        protected override string GetNewName(ISymbol symbol) => MiKo_1084_VariablesWithNumberSuffixAnalyzer.FindBetterName(symbol);
    }
}