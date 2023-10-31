using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1407_TestNamespaceAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNamespaceNames =
                                                               {
                                                                   "Test",
                                                                   "Tests",
                                                                   "UnitTest",
                                                                   "UnitTests",
                                                                   "IntegrationTest",
                                                                   "IntegrationTests",
                                                                   "Test.Abc",
                                                                   "Tests.Abc",
                                                                   "UnitTest.Abc",
                                                                   "UnitTests.Abc",
                                                                   "IntegrationTest.Abc",
                                                                   "IntegrationTests.Abc",
                                                                   "Abc.Test",
                                                                   "Abc.Tests",
                                                                   "Abc.UnitTest",
                                                                   "Abc.UnitTests",
                                                                   "Abc.IntegrationTest",
                                                                   "Abc.IntegrationTests",
                                                               };

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething() { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_with_correct_namespace() => Assert.Multiple(() =>
                                                                                                         {
                                                                                                             foreach (var testFixture in TestFixtures)
                                                                                                             {
                                                                                                                 foreach (var test in Tests)
                                                                                                                 {
                                                                                                                     No_issue_is_reported_for(@"
namespace Bla
{
  [" + testFixture + @"]
  public class TestMe
  {
      [" + test + @"]
      public void DoSomething_does_something() { }
  }
}
");
                                                                                                                 }
                                                                                                             }
                                                                                                         });

        [Test]
        public void An_issue_is_reported_for_test_class_with_incorrect_namespace_([ValueSource(nameof(WrongNamespaceNames))] string namespaceName) => Assert.Multiple(() =>
                                                                                                                                                                           {
                                                                                                                                                                               foreach (var testFixture in TestFixtures)
                                                                                                                                                                               {
                                                                                                                                                                                   foreach (var test in Tests)
                                                                                                                                                                                   {
                                                                                                                                                                                       An_issue_is_reported_for(@"
namespace " + namespaceName + @"
{
  [" + testFixture + @"]
  public class TestMe
  {
      [" + test + @"]
      public void DoSomething_does_something() { }
  }
}
");
                                                                                                                                                                                   }
                                                                                                                                                                               }
                                                                                                                                                                           });

        protected override string GetDiagnosticId() => MiKo_1407_TestNamespaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1407_TestNamespaceAnalyzer();
    }
}