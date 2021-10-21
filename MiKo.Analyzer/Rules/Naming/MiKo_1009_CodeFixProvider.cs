﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1009_CodeFixProvider)), Shared]
    public sealed class MiKo_1009_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1009_EventHandlerLocalVariableAnalyzer.Id;

        protected override string Title => Resources.MiKo_1009_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1009_EventHandlerLocalVariableAnalyzer.FindBetterName();
    }
}