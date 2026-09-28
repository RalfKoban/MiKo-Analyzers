using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2243_CRefXmlTagUsesPrefixAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2243";

        public MiKo_2243_CRefXmlTagUsesPrefixAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeXmlTextAttribute, SyntaxKind.XmlTextAttribute);

        private void AnalyzeXmlTextAttribute(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is XmlTextAttributeSyntax attribute && attribute.GetName() is Constants.XmlTag.Attribute.Cref)
            {
                var text = attribute.GetTextWithoutTrivia();

                var delimiterIndex = text.IndexOf(':') + 1;

                if (delimiterIndex > 0)
                {
                    var proposal = CreateReplacementProposal(text, text.Substring(delimiterIndex));

                    ReportDiagnostics(context, Issue(attribute, proposal));
                }
            }
        }
    }
}