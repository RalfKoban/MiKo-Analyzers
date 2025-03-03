using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2029_InheritdocUsesWrongCrefAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2029";

        public MiKo_2029_InheritdocUsesWrongCrefAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var symbol = context.ContainingSymbol;

                switch (symbol?.Kind)
                {
                    case SymbolKind.NamedType:
                    case SymbolKind.Method:
                    case SymbolKind.Property:
                    case SymbolKind.Event:
                    {
                        var issues = AnalyzeComment(comment, symbol, context.SemanticModel);

                        if (issues.Count > 0)
                        {
                            ReportDiagnostics(context, issues);
                        }

                        break;
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var node in comment.DescendantNodes())
            {
                var cref = node.GetCref(Constants.XmlTag.Inheritdoc);
                var type = cref.GetCrefType();

                if (type?.GetSymbol(semanticModel) is ISymbol linked && symbol.Equals(linked, SymbolEqualityComparer.Default))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(cref));
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}