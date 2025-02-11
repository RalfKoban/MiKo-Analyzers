using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1501_FilterAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = ["m_", "s_", "t_", "_", string.Empty,];
        private static readonly string[] FilterNames = ["Filter", "filter"];

        [Test]
        public void No_issue_is_reported_for_method_without_Filter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething(int i) { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_event_without_Filter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public event EventHandler DoSomething;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_property_without_Filter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public int Something { get; set; }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_field_without_Filter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int Something;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_([ValueSource(nameof(FilterNames))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public void " + name + @"Something(int i) { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_event_with_([ValueSource(nameof(FilterNames))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public event EventHandler " + name + @"Something;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_([ValueSource(nameof(FilterNames))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public int " + name + @"Something { get; set;}
  }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_([ValueSource(nameof(FieldPrefixes))] string fieldPrefix, [ValueSource(nameof(FilterNames))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      private int " + fieldPrefix + name + @"Something(int i) { }
  }
}
");

        protected override string GetDiagnosticId() => MiKo_1501_FilterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1501_FilterAnalyzer();
    }
}