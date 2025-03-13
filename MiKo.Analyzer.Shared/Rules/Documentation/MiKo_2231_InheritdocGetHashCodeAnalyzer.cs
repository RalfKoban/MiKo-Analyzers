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

        public MiKo_2231_InheritdocGetHashCodeAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.IsOverride && method.Parameters.IsEmpty && method.ReturnType.SpecialType == SpecialType.System_Int32 && method.Name == nameof(GetHashCode);

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
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