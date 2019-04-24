using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2301_TestArrangeActAssertCommentAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2301";

        public MiKo_2301_TestArrangeActAssertCommentAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml) => Enumerable.Empty<Diagnostic>();
    }
}