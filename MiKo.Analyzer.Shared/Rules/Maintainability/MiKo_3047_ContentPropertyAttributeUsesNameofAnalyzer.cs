using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3047_ContentPropertyAttributeUsesNameofAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3047";

        public MiKo_3047_ContentPropertyAttributeUsesNameofAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        }

        private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            var node = (AttributeSyntax)context.Node;

            var attributeName = node.Name.ToString();

            switch (attributeName)
            {
                case "System.Windows.Markup.ContentProperty":
                case "System.Windows.Markup.ContentPropertyAttribute":
                case "Windows.Markup.ContentProperty":
                case "Windows.Markup.ContentPropertyAttribute":
                case "Markup.ContentProperty":
                case "Markup.ContentPropertyAttribute":
                case "ContentProperty":
                case "ContentPropertyAttribute":
                {
                    var argumentList = node.ArgumentList;

                    if (argumentList != null)
                    {
                        var issues = argumentList.Arguments
                                                 .Select(_ => _.Expression)
                                                 .Where(_ => _?.IsKind(SyntaxKind.StringLiteralExpression) is true)
                                                 .Select(_ => Issue(attributeName, _));

                        ReportDiagnostics(context, issues);
                    }

                    break;
                }
            }
        }
    }
}