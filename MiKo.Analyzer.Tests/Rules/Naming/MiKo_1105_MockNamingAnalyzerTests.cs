using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1105_MockNamingAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_incorrectly_named_field_in_non_test_class([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    private int _" + name + @";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_incorrectly_named_variable_in_non_test_class([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        int " + name + @" = 0;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_named_variable_declaration([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_incorrectly_named_variable_on_multi_variable_declaration_in_non_test_class([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        int i = 0, " + name + @" = 0;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_on_multi_variable_declaration([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_named_parameter([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
[TestFixture]
public class TestMe
{
    public void DoSomething(int " + name + @")
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1105_MockNamingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1105_MockNamingAnalyzer();

        private static IEnumerable<string> WrongNames() => new[]
                                                               {
                                                                   "somethingMock",
                                                                   "mock",
                                                                   "MockManager",
                                                                   "somethingStub",
                                                                   "stub",
                                                                   "StubManager",
                                                               };
    }
}