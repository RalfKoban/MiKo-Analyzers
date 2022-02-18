using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2041_InvalidXmlInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2041";

        internal static readonly HashSet<string> InvalidSummaryCrefXmlTags = new HashSet<string>
                                                                                 {
                                                                                     Constants.XmlTag.Example,
                                                                                     Constants.XmlTag.Exception,
                                                                                     Constants.XmlTag.Include,
                                                                                     Constants.XmlTag.Inheritdoc,
                                                                                     Constants.XmlTag.Overloads,
                                                                                     Constants.XmlTag.Param,
                                                                                     Constants.XmlTag.ParamRef,
                                                                                     Constants.XmlTag.Permission,
                                                                                     Constants.XmlTag.Remarks,
                                                                                     Constants.XmlTag.Returns,
                                                                                     Constants.XmlTag.SeeAlso,
                                                                                     Constants.XmlTag.Summary,
                                                                                     Constants.XmlTag.TypeParam,
                                                                                     Constants.XmlTag.TypeParamRef,
                                                                                     Constants.XmlTag.Value,
                                                                                 };

        public MiKo_2041_InvalidXmlInSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static IEnumerable<XmlNodeSyntax> GetIssues(DocumentationCommentTriviaSyntax documentation) => documentation != null
                                                                                                                    ? documentation.GetSummaryXmls(InvalidSummaryCrefXmlTags)
                                                                                                                    : Enumerable.Empty<XmlNodeSyntax>();

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var node in GetIssues(comment))
            {
                var name = GetName(node);

                yield return Issue(name, node);
            }
        }

        private static string GetName(SyntaxNode node)
        {
            switch (node)
            {
                case XmlElementSyntax e: return e.GetName();
                case XmlEmptyElementSyntax ee: return ee.GetName();
                default: return string.Empty;
            }
        }
    }
}