using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2044";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.See,
                                                               Constants.XmlTag.SeeAlso,
                                                           };

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                if (context.ContainingSymbol is IMethodSymbol method)
                {
                    var issues = AnalyzeComment(comment, method);

                    if (issues.Count > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IMethodSymbol method)
        {
            var names = method.Parameters.ToHashSet(_ => _.Name);

            if (names.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            foreach (var node in comment.AllDescendantNodes())
            {
                if (node.IsXml())
                {
                    var tag = node.GetXmlTagName();

                    if (Tags.Contains(tag))
                    {
                        var cref = node.GetCref();

                        if (cref is null)
                        {
                            continue;
                        }

                        var name = cref.GetCrefType().GetName();

                        if (names.Contains(name))
                        {
                            if (results is null)
                            {
                                results = new List<Diagnostic>(1);
                            }

                            results.Add(Issue(node, node.GetText()));
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}