using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6032";

        public MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer() : base(Id)
        {
        }

        internal static LinePosition GetOutdentedStartPosition(ParameterListSyntax parameterList)
        {
            var startPosition = parameterList.GetStartPosition();
            var characterPosition = startPosition.Character + parameterList.OpenParenToken.Span.Length;

            var parameters = parameterList.Parameters;

            // inspect whether the first parameter is on the same line as the method name itself, in such case we cannot outdent the parameters
            if (parameters.Count > 0)
            {
                // hence we have to take the position of that very first parameter
                if (startPosition.Line != parameters[0].GetStartPosition().Line)
                {
                    characterPosition -= Constants.Indentation;
                }
            }

            return new LinePosition(startPosition.Line, characterPosition);
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ParameterList);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ParameterListSyntax parameterList && parameterList.Parameters.SeparatorCount > 0)
            {
                ReportDiagnostics(context, AnalyzeNode(parameterList));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(ParameterListSyntax parameterList)
        {
            var startPosition = GetOutdentedStartPosition(parameterList);
            var characterPosition = startPosition.Character;

            var inspectedLines = new HashSet<int> { startPosition.Line };

            foreach (var parameter in parameterList.Parameters)
            {
                var parameterPosition = parameter.GetStartPosition();

                if (inspectedLines.Add(parameterPosition.Line))
                {
                    // this is a new found line, so inspect start position
                    if (parameterPosition.Character != characterPosition)
                    {
                        yield return Issue(parameter.GetName(), parameter);
                    }
                }
                else
                {
                    // line was already known, eg. due to a leading parameter
                }
            }
        }
    }
}