using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6050";

        public MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer() : base(Id)
        {
        }

        internal static LinePosition GetOutdentedStartPosition(ArgumentListSyntax argumentList)
        {
            var startPosition = argumentList.GetStartPosition();
            var characterPosition = startPosition.Character + argumentList.OpenParenToken.Span.Length;

            var arguments = argumentList.Arguments;

            // inspect whether the first argument is on the same line as the method name itself, in such case we cannot outdent the arguments
            if (arguments.Count > 0)
            {
                // hence we have to take the position of that very first argument
                if (startPosition.Line != arguments[0].GetStartPosition().Line)
                {
                    characterPosition -= Constants.Indentation;
                }
            }

            return new LinePosition(startPosition.Line, characterPosition);
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ArgumentList);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentListSyntax argumentList && argumentList.Arguments.SeparatorCount > 0)
            {
                ReportDiagnostics(context, AnalyzeNode(argumentList));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(ArgumentListSyntax argumentList)
        {
            var startPosition = GetOutdentedStartPosition(argumentList);
            var characterPosition = startPosition.Character;

            var inspectedLines = new HashSet<int> { startPosition.Line };

            foreach (var argument in argumentList.Arguments)
            {
                var argumentPosition = argument.GetStartPosition();

                if (inspectedLines.Add(argumentPosition.Line))
                {
                    // this is a new found line, so inspect start position
                    if (argumentPosition.Character != characterPosition)
                    {
                        yield return Issue(argument.ToString(), argument);
                    }
                }
                else
                {
                    // line was already known, eg. due to a leading argument
                }
            }
        }
    }
}