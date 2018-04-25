using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3010_ReservedExceptionsAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3010";

        public MiKo_3010_ReservedExceptionsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => IsForbiddenException(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => new[] { ReportIssue(node.Type.ToString(), node.GetLocation()) };

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
                case "ApplicationException":
                case "SystemException":
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
                case "System.ApplicationException":
                case "System.SystemException":
                    return true;

                default:
                    return false;
            }
        }
    }
}