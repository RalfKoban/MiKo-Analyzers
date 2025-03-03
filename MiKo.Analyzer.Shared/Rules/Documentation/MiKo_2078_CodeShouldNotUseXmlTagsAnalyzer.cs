using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2078";

        public MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                switch (context.ContainingSymbol?.Kind)
                {
                    case SymbolKind.NamedType:
                    case SymbolKind.Method:
                    case SymbolKind.Property:
                    case SymbolKind.Event:
                    {
                        var issues = AnalyzeComment(comment);

                        if (issues.Count > 0)
                        {
                            ReportDiagnostics(context, issues);
                        }

                        break;
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            var codeTags = comment.GetXmlSyntax(Constants.XmlTag.Code);
            var codeTagsCount = codeTags.Count;

            if (codeTagsCount > 0)
            {
                for (var index = 0; index < codeTagsCount; index++)
                {
                    var code = codeTags[index];

                    foreach (var entry in code.Content)
                    {
                        if (entry.IsXml())
                        {
                            // we have an issue
                            return new[] { Issue(entry) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}