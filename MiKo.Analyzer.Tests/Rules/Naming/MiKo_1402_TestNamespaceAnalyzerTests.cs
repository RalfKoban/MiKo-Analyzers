using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1402_TestNamespaceAnalyzerTests : CodeFixVerifier
    {
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

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_with_correct_namespace(
                                                                            [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                            [ValueSource(nameof(Tests))] string testAttribute)
            => No_issue_is_reported_for(@"
namespace Bla
{
  [" + testClassAttribute + @"]
  public class TestMe
  {
      [" + testAttribute + @"]
      public void DoSomething_does_something() { }
  }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_class_with_incorrect_namespace(
                                                                            [ValueSource(nameof(WrongNamespaceNames))] string namespaceName,
                                                                            [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                            [ValueSource(nameof(Tests))] string testAttribute)
            => An_issue_is_reported_for(@"
namespace " + namespaceName + @"
{
  [" + testClassAttribute + @"]
  public class TestMe
  {
      [" + testAttribute + @"]
      public void DoSomething_does_something() { }
  }
}
");

        protected override string GetDiagnosticId() => MiKo_1402_TestNamespaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1402_TestNamespaceAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> WrongNamespaceNames() => new[]
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
    }
}