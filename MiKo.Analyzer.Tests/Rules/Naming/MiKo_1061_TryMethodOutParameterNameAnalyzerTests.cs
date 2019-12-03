using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1061_TryMethodOutParameterNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_no_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Try_method_with_an_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething(out int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_no_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_Try_method_with_correctly_named_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(out int result) { }
}
");

        [Test]
        public void An_issue_is_reported_for_Try_method_with_incorrectly_named_out_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryDoSomething(out int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_TryGet_method_with_out_parameter_named_result() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int result) { }
}
");

        [Test]
        public void No_issue_is_reported_for_TryGet_method_with_correctly_named_out_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int myOwnValue) { }
}
");

        [Test]
        public void An_issue_is_reported_for_TryGet_method_with_incorrectly_named_out_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    public void TryGetMyOwnValue(out int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_TryGet_method_with_out_parameter_named_value() => No_issue_is_reported_for(@"

public class TestMe
{
    public void TryGet(out int value) { }
}
");

        protected override string GetDiagnosticId() => MiKo_1061_TryMethodOutParameterNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1061_TryMethodOutParameterNameAnalyzer();
    }
}