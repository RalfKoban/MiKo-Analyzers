using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3001_DelegateAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_delegate() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public delegate void TestMeDelegatge(string something);
}
");

        protected override string GetDiagnosticId() => MiKo_3001_DelegateAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3001_DelegateAnalyzer();
    }
}