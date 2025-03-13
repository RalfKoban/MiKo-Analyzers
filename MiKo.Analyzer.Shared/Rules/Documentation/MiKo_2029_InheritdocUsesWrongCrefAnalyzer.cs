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

        public MiKo_2029_InheritdocUsesWrongCrefAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
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