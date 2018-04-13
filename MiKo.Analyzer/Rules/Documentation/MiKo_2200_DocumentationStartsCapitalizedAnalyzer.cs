using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2200_DocumentationStartsCapitalizedAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2200";

        private static readonly HashSet<string> XmlTags = typeof(Constants.XmlTag).GetFields(BindingFlags.NonPublic | BindingFlags.Static).Select(_ => _.GetRawConstantValue().ToString()).ToHashSet();

        public MiKo_2200_DocumentationStartsCapitalizedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            foreach (var xmlTag in XmlTags)
            {
                foreach (var _ in GetCommentElements(commentXml, xmlTag).Nodes()
                                                                        .Select(_ => _.ToString().TrimStart())
                                                                        .Where(_ => _.Length > 0)
                                                                        .Select(_ => _[0])
                                                                        .Where(_ => !_.IsUpperCase() && _ != '<'))
                {
                    yield return ReportIssue(symbol, xmlTag);
                }
            }
        }
    }
}