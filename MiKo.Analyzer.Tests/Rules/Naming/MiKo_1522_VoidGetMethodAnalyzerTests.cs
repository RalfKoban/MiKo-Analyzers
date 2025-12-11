using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1522_VoidGetMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method_starting_without_Get() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_void_method_starting_with_Get() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private object GetSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_test_method_starting_with_Get() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void GetSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_starting_with_Get() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void GetSomething(int something)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1522_VoidGetMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1522_VoidGetMethodAnalyzer();
    }
}