using System;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3010_ReservedExceptionsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ForbiddenExceptions =
            {
                nameof(Exception),
                nameof(AccessViolationException),
                nameof(IndexOutOfRangeException),
                nameof(ExecutionEngineException),
                nameof(NullReferenceException),
                nameof(OutOfMemoryException),
                nameof(StackOverflowException),
                nameof(COMException),
                nameof(SEHException),
                nameof(ApplicationException),
                nameof(SystemException),
                "System.Exception",
                "System.AccessViolationException",
                "System.IndexOutOfRangeException",
                "System.ExecutionEngineException",
                "System.NullReferenceException",
                "System.OutOfMemoryException",
                "System.StackOverflowException",
                "System.Runtime.InteropServices.COMException",
                "System.Runtime.InteropServices.SEHException",
            };

        [Test]
        public void No_issue_is_reported_for_normal_created_object([Values(nameof(Object), nameof(Int32), nameof(ArgumentException))] string type) => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething()
    {
        var x = new " + type + @"();
    }
}
");
        [Test]
        public void An_issue_is_reported_for_forbidden_exception([ValueSource(nameof(ForbiddenExceptions))] string type) => An_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething()
    {
        var x = new " + type + @"();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3010_ReservedExceptionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3010_ReservedExceptionsAnalyzer();
    }
}