using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                // ignore empty comments
            }
            else
            {
                var comment = commentXml.Without(Constants.Markers.Symbols);

                var name = symbol.FullyQualifiedName();

                if (comment.Contains($"{Constants.Comments.XmlElementStartingTag}{Constants.XmlTag.Inheritdoc} cref=\"{name}\"")
                 || comment.Contains($"{Constants.Comments.XmlElementStartingTag}{Constants.XmlTag.Inheritdoc} cref='{name}'"))
                {
                    yield return Issue(symbol);
                }
            }
        }
    }
}