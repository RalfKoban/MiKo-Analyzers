using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_2204_DocumentationShallUseListAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2244_DocumentationShallUseListAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2244";

        private static readonly string[] Tags = { "ul", "ol" };

        public MiKo_2244_DocumentationShallUseListAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> issues = null;

            foreach (var tag in comment.DescendantNodes<XmlElementStartTagSyntax>(_ => _.GetName().EqualsAny(Tags, StringComparison.OrdinalIgnoreCase)))
            {
                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(tag));
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}