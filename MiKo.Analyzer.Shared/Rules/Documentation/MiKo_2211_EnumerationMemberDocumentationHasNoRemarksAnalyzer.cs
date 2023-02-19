using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2211";

        public MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.ContainingType.IsEnum() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var remarks in comment.GetRemarksXmls())
            {
                yield return Issue(symbol.Name, remarks.StartTag);
            }
        }
    }
}