using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_EventArgs_with_multiple_interface_implementations() => An_issue_is_reported_for(2, @"
using System;

public class TestMeEventArgs : EventArgs, IDisposable, ICloneable
{
    public void Dispose() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3042_EventArgsInterfaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3042_EventArgsInterfaceAnalyzer();
    }
}