using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_namespace_without_leading_or_trailing_underscore() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething(int i) { }
  }
}
");

        [TestCase("_Abc")]
        [TestCase("_Abc_")]
        [TestCase("Abc_")]
        [TestCase("_Abc.Def.Ghi")]
        [TestCase("_Abc_.Def.Ghi")]
        [TestCase("Abc_.Def.Ghi")]
        [TestCase("Abc._Def.Ghi")]
        [TestCase("Abc._Def_.Ghi")]
        [TestCase("Abc.Def_.Ghi")]
        [TestCase("Abc.Def._Ghi")]
        [TestCase("Abc.Def._Ghi_")]
        [TestCase("Abc.Def.Ghi_")]
        public void An_issue_is_reported_for_namespace_with_underscore_(string fullQualifiedNamespaceName) => An_issue_is_reported_for(@"
namespace " + fullQualifiedNamespaceName + @"
{
  public static class TestMe
  {
      public static void DoSomething(this int i) { }
  }
}
");

        protected override string GetDiagnosticId() => MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer();
    }
}