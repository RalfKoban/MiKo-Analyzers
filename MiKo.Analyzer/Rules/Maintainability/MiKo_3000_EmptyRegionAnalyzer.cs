using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3000_EmptyRegionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3000";

        public MiKo_3000_EmptyRegionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeRegionDirectiveTrivia, SyntaxKind.RegionDirectiveTrivia);
        }

        private static bool HasSomethingInsideRegion(DirectiveTriviaSyntax regionNode)
        {
            // contains #region  and #endregion directive trivia syntaxes
            var relatedDirectives = regionNode.GetRelatedDirectives();
            if (relatedDirectives.Count != 2)
            {
                return false;
            }

            var regionDirective = relatedDirectives[0];
            var endregionDirective = relatedDirectives[1];

            var regionAncestors = regionDirective.AncestorsAndSelf();
            var endregionAncestors = endregionDirective.AncestorsAndSelf();

            return regionAncestors.Except(endregionAncestors).Except(relatedDirectives).Any();
        }

        private void AnalyzeRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            var regionNode = (RegionDirectiveTriviaSyntax)context.Node;

            var somethingInside = HasSomethingInsideRegion(regionNode);

            if (somethingInside is false)
            {
                var issue = Issue(regionNode);
                context.ReportDiagnostic(issue);
            }
        }
    }
}