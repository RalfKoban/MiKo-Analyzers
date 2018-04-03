using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3010_ReservedExceptionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3010";

        public MiKo_3010_ReservedExceptionsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ObjectCreationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeObjectCreation(node);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeObjectCreation(ObjectCreationExpressionSyntax node)
        {
            var type = node.Type.ToString();

            return IsForbiddenException(type)
                       ? ReportIssue(type, node.GetLocation())
                       : null;
        }

        private static bool IsForbiddenException(string exceptionType)
        {
            switch (exceptionType)
            {
                case "Exception":
                case "AccessViolationException":
                case "IndexOutOfRangeException":
                case "ExecutionEngineException":
                case "NullReferenceException":
                case "OutOfMemoryException":
                case "StackOverflowException":
                case "COMException":
                case "SEHException":
                case "InteropServices.COMException":
                case "InteropServices.SEHException":
                case "Runtime.InteropServices.COMException":
                case "Runtime.InteropServices.SEHException":
                case "System.Exception":
                case "System.AccessViolationException":
                case "System.IndexOutOfRangeException":
                case "System.ExecutionEngineException":
                case "System.NullReferenceException":
                case "System.OutOfMemoryException":
                case "System.StackOverflowException":
                case "System.Runtime.InteropServices.COMException":
                case "System.Runtime.InteropServices.SEHException":
                    return true;

                default:
                    return false;
            }
        }
    }
}