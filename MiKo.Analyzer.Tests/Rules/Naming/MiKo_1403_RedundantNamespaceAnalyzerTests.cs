using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1403_RedundantNamespaceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_root_namespace() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething() { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_non_redundant_namespace() => No_issue_is_reported_for(@"
namespace Bla.Blubb
{
  public class TestMe
  {
      public void DoSomething() { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_redundant_namespace([Values("Bla.Bla", "Bla.Blubb.Bla", "Bla.Blubb.Blubber.Blubb")] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
  public class TestMe
  {
      public void DoSomething() { }
  }
}
");

        protected override string GetDiagnosticId() => MiKo_1403_RedundantNamespaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1403_RedundantNamespaceAnalyzer();
    }
}