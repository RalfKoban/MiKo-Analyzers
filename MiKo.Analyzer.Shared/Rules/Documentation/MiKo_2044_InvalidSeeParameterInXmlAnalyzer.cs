using System;
using System.Collections.Generic;

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

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.Parameters.Length > 0;

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol is IMethodSymbol method)
            {
                return AnalyzeComment(comment, method);
            }

            return Array.Empty<Diagnostic>();
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IMethodSymbol method)
        {
            var names = method.Parameters.ToHashSet(_ => _.Name);

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