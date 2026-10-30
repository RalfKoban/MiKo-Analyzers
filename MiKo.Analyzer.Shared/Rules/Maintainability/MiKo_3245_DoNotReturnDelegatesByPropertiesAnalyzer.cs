using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3245";

        public MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol, Compilation compilation) => base.ShallAnalyze(symbol, compilation) && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            if (symbol.GetReturnType().IsDelegate())
            {
                var location = GetLocation(symbol);

                return new[] { Issue(symbol.Name, location) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static Location GetLocation(IPropertySymbol symbol)
        {
            if (symbol.IsIndexer)
            {
                var syntax = symbol.GetSyntax<IndexerDeclarationSyntax>();

                return Location.Create(syntax.SyntaxTree, TextSpan.FromBounds(syntax.Type.SpanStart, syntax.ParameterList.Span.End));
            }
            else
            {
                var syntax = symbol.GetSyntax<PropertyDeclarationSyntax>();

                return Location.Create(syntax.SyntaxTree, TextSpan.FromBounds(syntax.Type.SpanStart, syntax.Identifier.Span.End));
            }
        }
    }
}