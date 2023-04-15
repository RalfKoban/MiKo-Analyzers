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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in comment.DescendantNodes())
            {
                var cref = node.GetCref(Constants.XmlTag.Inheritdoc);
                var type = cref.GetCrefType();

                if (type?.GetSymbol(compilation) is ISymbol linked && symbol.Equals(linked, SymbolEqualityComparer.Default))
                {
                    yield return Issue(cref);
                }
            }
        }
    }
}