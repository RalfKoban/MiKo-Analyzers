using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6030";

        private static readonly SyntaxKind[] Initializers = { SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression, SyntaxKind.ObjectInitializerExpression };

        public MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer() : base(Id)
        {
        }

        internal static LinePosition GetStartPosition(InitializerExpressionSyntax initializer)
        {
            switch (initializer.Parent)
            {
                case ArrayCreationExpressionSyntax a: return GetStartPosition(a.Type);
                case ObjectCreationExpressionSyntax o: return GetStartPosition(o.Type);
                case ImplicitArrayCreationExpressionSyntax ia: return GetStartPosition(ia.CloseBracketToken);

                // consider reduced array initializers
                case EqualsValueClauseSyntax e:
                {
                    var position = e.EqualsToken.GetLocation().GetLineSpan().EndLinePosition;

                    return new LinePosition(position.Line, position.Character + 1);
                }

                default:
                {
                    return GetStartPosition(initializer.Parent);
                }
            }
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Initializers);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InitializerExpressionSyntax initializer)
            {
                var openBraceToken = initializer.OpenBraceToken;

                var typePosition = GetStartPosition(initializer);
                var openBracePosition = GetStartPosition(openBraceToken);

                if (typePosition.Line != openBracePosition.Line)
                {
                    if (typePosition.Character != openBracePosition.Character)
                    {
                        ReportDiagnostics(context, Issue(openBraceToken));
                    }
                }
            }
        }
    }
}