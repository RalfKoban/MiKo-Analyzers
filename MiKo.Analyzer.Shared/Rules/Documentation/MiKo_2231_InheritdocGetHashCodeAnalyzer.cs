using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2231_InheritdocGetHashCodeAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2231";

        public MiKo_2231_InheritdocGetHashCodeAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride && symbol.Parameters.IsEmpty && symbol.ReturnType.SpecialType == SpecialType.System_Int32 && symbol.Name == nameof(GetHashCode);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                if (context.ContainingSymbol is IMethodSymbol symbol && ShallAnalyze(symbol))
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
            var tagNames = comment.Content.ToHashSet(_ => _.GetXmlTagName());

            if (tagNames.Contains(Constants.XmlTag.Inheritdoc))
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(comment) };
        }
    }
}