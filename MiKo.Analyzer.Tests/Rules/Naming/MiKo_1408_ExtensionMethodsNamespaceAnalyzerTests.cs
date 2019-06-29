using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1408_ExtensionMethodsNamespaceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_extension_method_class() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething(int i) { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_extension_method_class_with_correct_namespace() => No_issue_is_reported_for(@"
namespace System
{
  public static class TestMe
  {
      public static void DoSomething(this int i) { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_class_with_incorrect_namespace() => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public static void DoSomething(this int i) { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_class_with_incorrect_namespace_2() => An_issue_is_reported_for(@"
namespace Bla.Blubb
{
  public class TestMe
  {
      public void DoSomething(int i) { }
  }
}

namespace Blubber.Bla.Blubb
{
  public static class TestMeExtensions
  {
      public static void DoSomething(this TestMe t) { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_generic_extension() => No_issue_is_reported_for(@"
namespace Bla
{
    public static class TestMe
    {
        public static void DoSomething<T>(this T value) where T : class
        {
        } 
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1408_ExtensionMethodsNamespaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1408_ExtensionMethodsNamespaceAnalyzer();
    }
}