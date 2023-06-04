using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3010_ReservedExceptionsAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3010";

        private static readonly HashSet<string> ForbiddenExceptionTypes = new HashSet<string>
                                                                              {
                                                                                  nameof(Exception),
                                                                                  nameof(AccessViolationException),
                                                                                  nameof(IndexOutOfRangeException),
                                                                                  "ExecutionEngineException",
                                                                                  nameof(NullReferenceException),
                                                                                  nameof(OutOfMemoryException),
                                                                                  nameof(StackOverflowException),
                                                                                  nameof(COMException),
                                                                                  nameof(SEHException),
                                                                                  nameof(ApplicationException),
                                                                                  nameof(SystemException),
                                                                                  "InteropServices.COMException",
                                                                                  "InteropServices.SEHException",
                                                                                  "Runtime.InteropServices.COMException",
                                                                                  "Runtime.InteropServices.SEHException",
                                                                                  "System.ExecutionEngineException",
                                                                                  typeof(Exception).FullName,
                                                                                  typeof(AccessViolationException).FullName,
                                                                                  typeof(IndexOutOfRangeException).FullName,
                                                                                  typeof(NullReferenceException).FullName,
                                                                                  typeof(OutOfMemoryException).FullName,
                                                                                  typeof(StackOverflowException).FullName,
                                                                                  typeof(COMException).FullName,
                                                                                  typeof(SEHException).FullName,
                                                                                  typeof(ApplicationException).FullName,
                                                                                  typeof(SystemException).FullName,
                                                                              };

        public MiKo_3010_ReservedExceptionsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => ForbiddenExceptionTypes.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => new[] { Issue(node.Type.ToString(), node) };
    }
}