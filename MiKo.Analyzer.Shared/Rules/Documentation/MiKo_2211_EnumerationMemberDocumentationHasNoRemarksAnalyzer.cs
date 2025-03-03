using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2211";

        public MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.ContainingType.IsEnum();

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                if (context.ContainingSymbol is IFieldSymbol symbol && ShallAnalyze(symbol))
                {
                    var issues = AnalyzeComment(comment);

                    if (issues.Count > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            var remarks = comment.GetRemarksXmls();

            if (remarks.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return remarks.Select(_ => Issue(_.StartTag)).ToList();
        }
    }
}