using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                                  {
                                                                      Constants.XmlTag.See,
                                                                      Constants.XmlTag.See.ToUpperInvariant(),
                                                                      Constants.XmlTag.SeeAlso,
                                                                      Constants.XmlTag.SeeAlso.ToUpperInvariant(),
                                                                  };

        public MiKo_2020_InheritdocSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var xmlTag in comment.GetSummaryXmls(Tags))
            {
                var cref = xmlTag.GetCref();

                if (cref != null && HasIssue(symbol, compilation, cref))
                {
                    yield return Issue(xmlTag);
                }
            }
        }

        private static bool HasIssue(ISymbol symbol, Compilation compilation, XmlCrefAttributeSyntax cref)
        {
            if (symbol.IsOverride)
            {
                return true;
            }

            switch (symbol)
            {
                case IPropertySymbol _:
                case IEventSymbol _:
                case IMethodSymbol _:
                {
                    return symbol.IsInterfaceImplementation();
                }

                case ITypeSymbol typeSymbol:
                {
                    var type = cref.GetCrefType();

                    return type?.GetSymbol(compilation) is ITypeSymbol linked && typeSymbol.IsRelated(linked);
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}