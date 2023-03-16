﻿using System.Collections.Generic;

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

            var type = cref.GetCrefType();
            var linkedSymbol = type?.GetSymbol(compilation);

            switch (symbol)
            {
                case IPropertySymbol _:
                {
                    return linkedSymbol is IPropertySymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case IEventSymbol _:
                {
                    return linkedSymbol is IEventSymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case IMethodSymbol _:
                {
                    return linkedSymbol is IMethodSymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case ITypeSymbol typeSymbol:
                {
                    return linkedSymbol is ITypeSymbol linked && typeSymbol.IsRelated(linked);
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}