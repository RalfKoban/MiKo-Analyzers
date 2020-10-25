using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1108_MockNamingAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames =
            {
                "childFake",
                "childMock1",
                "childShim",
                "fakeChild1",
                "mock",
                "mockChild1",
                "MockManager",
                "shimChild",
                "somethingMock",
                "somethingStub",
                "stub",
                "StubManager",
            };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    private int _something;
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_field_in_non_test_class_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    private int _" + name + @";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    private int _" + name + @";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_variable() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_variable_in_non_test_class_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        int " + name + @" = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_variable_in_foreach_loop() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething()
    {
        foreach (var c in Path.InvalidPathChars)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething()
    {
        int " + name + @" = 0;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_in_foreach_loop_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething()
    {
        foreach (var " + name + @" in Path.InvalidPathChars)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_variable_declaration() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int i: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_declaration_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return;
            default: return;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_variable_on_multi_variable_declaration_in_non_test_class_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        int i = 0, " + name + @" = 0;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_on_multi_variable_declaration_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething()
    {
        int i = 0, " + name + @" = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter_in_ctor() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void TestMe()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_parameter_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething(int " + name + @")
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_parameter_in_ctor_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public TestMe(int " + name + @")
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_parameter_in_lambda_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public int DoSomething()
    {
        return Get(" + name + @" => " + name + @".Data);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field_declaration() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    private int i;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field_declaration_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
   private int " + name + @";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_multi_field_declaration_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
   private int i, " + name + @";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_property_declaration() => No_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    private int i { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property_declaration_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
   private int " + name + @" { get; }
}
");

        [TestCase("[TestFixture] class TestMe { int MyMock { get; } }", "[TestFixture] class TestMe { int My { get; } }")]
        [TestCase("[TestFixture] class TestMe { int MyMock; }", "[TestFixture] class TestMe { int My; }")]
        [TestCase("[TestFixture] class TestMe { void Do() { int myMock = 0; } }", "[TestFixture] class TestMe { void Do() { int my = 0; } }")]
        [TestCase("[TestFixture] class TestMe { void Do(int myMock) { } }", "[TestFixture] class TestMe { void Do(int my) { } }")]
        [TestCase("[TestFixture] class TestMe { void Do(int mock) { } }", "[TestFixture] class TestMe { void Do(int mock) { } }")]
        [TestCase("[TestFixture] class TestMe { void Do(int mock1) { } }", "[TestFixture] class TestMe { void Do(int mock1) { } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1108_MockNamingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1108_MockNamingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1108_CodeFixProvider();
    }
}