﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1005_CodeFixProvider)), Shared]
    public sealed class MiKo_1005_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1005_EventArgsLocalVariableAnalyzer.Id;

        protected override string Title => "Rename EventArgs variable";

        protected override string GetNewName(ISymbol symbol) => MiKo_1005_EventArgsLocalVariableAnalyzer.FindBetterName(symbol);
    }
}