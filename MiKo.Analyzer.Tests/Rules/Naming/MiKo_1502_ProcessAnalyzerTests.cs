using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1502_ProcessAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = ["m_", "s_", "t_", "_", string.Empty,];
        private static readonly string[] Names = ["Process", "process", "Processor", "processor"];

        [Test]
        public void No_issue_is_reported_for_type_without_Process_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
  }
}
");

        [Test]
        public void No_issue_is_reported_for_method_without_Process_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public void DoSomething(int i) { }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_event_without_Process_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public event EventHandler DoSomething;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_property_without_Process_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      public int Something { get; set; }
  }
}
");

        [Test]
        public void No_issue_is_reported_for_field_without_Process_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int Something;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class Test" + name + @"Me
  {
      public void DoSomething(int i) { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public void " + name + @"Something(int i) { }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_event_with_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public event EventHandler " + name + @"Something;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      public int " + name + @"Something { get; set;}
  }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_([ValueSource(nameof(FieldPrefixes))] string fieldPrefix, [ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
namespace Bla
{
  public static class TestMe
  {
      private int " + fieldPrefix + name + @"Something(int i) { }
  }
}
");

        protected override string GetDiagnosticId() => MiKo_1502_ProcessAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1502_ProcessAnalyzer();
    }
}