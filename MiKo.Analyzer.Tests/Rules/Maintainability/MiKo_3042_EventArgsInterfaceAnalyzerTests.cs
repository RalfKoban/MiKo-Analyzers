using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3042_EventArgsInterfaceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_EventArgs_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_with_no_interface_implementation() => No_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
}
");

        [Test]
        public void An_issue_is_reported_for_EventArgs_with_an_interface_implementation() => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs, IDisposable
{
    public void Dispose() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3042_EventArgsInterfaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3042_EventArgsInterfaceAnalyzer();
    }
}