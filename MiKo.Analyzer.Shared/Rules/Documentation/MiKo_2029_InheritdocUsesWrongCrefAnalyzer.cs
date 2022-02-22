using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            var name = symbol.FullyQualifiedName();

            var crefs = Enumerable.Empty<XmlCrefAttributeSyntax>()
                                  .Concat(comment.GetXmlSyntax(Constants.XmlTag.Inheritdoc).SelectMany(_ => _.GetAttributes<XmlCrefAttributeSyntax>()))
                                  .Concat(comment.GetEmptyXmlSyntax(Constants.XmlTag.Inheritdoc).SelectMany(_ => _.GetAttributes<XmlCrefAttributeSyntax>()))
                                  .Select(_ => _.Cref);

            foreach (var cref in crefs)
            {
                if (cref.ToString() == name)
                {
                    yield return Issue(symbol.Name, cref);
                }
            }
        }
    }
}