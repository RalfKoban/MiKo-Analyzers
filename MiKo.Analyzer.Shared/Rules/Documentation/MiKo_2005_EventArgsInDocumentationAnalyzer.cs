using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2005_EventArgsInDocumentationAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2005";

        public MiKo_2005_EventArgsInDocumentationAnalyzer() : base(Id, (SymbolKind)(-1))
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
                    case SymbolKind.Field:
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
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(Constants.Comments.EventArgsTermsWithDelimiters, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(textTokens[i], Constants.Comments.EventArgsTermsWithDelimiters, StringComparison.OrdinalIgnoreCase, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(locationsCount);
                    }

                    for (var index = 0; index < locationsCount; index++)
                    {
                        results.Add(Issue(locations[index]));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}