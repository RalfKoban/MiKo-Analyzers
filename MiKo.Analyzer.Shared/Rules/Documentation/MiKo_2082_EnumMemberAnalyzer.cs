﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2082_EnumMemberAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2082";

        private static readonly string[] StartingPhrases =
            {
                "Defines",
                "Indicates",
                "Specifies",
            };

        public MiKo_2082_EnumMemberAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment) => symbol.GetFields().SelectMany(_ => AnalyzeField(_, compilation));

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            foreach (var wrongPhrase in StartingPhrases)
            {
                if (summary.StartsWith(wrongPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    var location = GetFirstLocation(textToken, wrongPhrase, StringComparison.OrdinalIgnoreCase);

                    return Issue(symbol.Name, location, wrongPhrase);
                }
            }

            return null;
        }
    }
}