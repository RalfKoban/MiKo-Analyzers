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

        public MiKo_2231_InheritdocGetHashCodeAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride && symbol.Parameters.IsEmpty && symbol.ReturnType.SpecialType == SpecialType.System_Int32 && symbol.Name == nameof(GetHashCode);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var tagNames = comment.Content.ToHashSet(_ => _.GetXmlTagName());

            if (tagNames.Contains(Constants.XmlTag.Inheritdoc))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(comment) };
        }
    }
}