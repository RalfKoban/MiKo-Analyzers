﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        private static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
        private static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };

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

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries)
        {
            if (summaries.Any(IsSeeCrefLink) && HasIssue(symbol, compilation))
            {
                yield return Issue(symbol);
            }
        }

        private static bool IsSeeCrefLink(string summary) => summary.StartsWithAny(SeeStartingPhrase) && summary.EndsWithAny(SeeEndingPhrase);

        private static bool HasIssue(ISymbol symbol, Compilation compilation)
        {
            if (symbol.IsOverride)
            {
                return true;
            }

            var commentTriviaSyntax = symbol.GetDocumentationCommentTriviaSyntax();
            var xmlTag = commentTriviaSyntax.GetSummaryXmls(Tags).FirstOrDefault();
            if (xmlTag is null)
            {
                return false;
            }

            switch (symbol)
            {
                case IMethodSymbol methodSymbol:
                {
                    return methodSymbol.IsInterfaceImplementation();
                }

                case ITypeSymbol typeSymbol:
                {
                    var linkedTypeSyntax = xmlTag.GetCref().GetCrefType();
                    var linkedTypeSymbol = linkedTypeSyntax?.GetSymbol(compilation) as ITypeSymbol;

                    return linkedTypeSymbol != null && typeSymbol.IsRelated(linkedTypeSymbol);
                }

                default:
                {
                    return true;
                }
            }
        }
    }
}