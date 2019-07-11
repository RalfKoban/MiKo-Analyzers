using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1069_PropertyNameMeaningAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public int X { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property_with_exact_same_name_as_interface() => An_issue_is_reported_for(@"

public interface ISomeInterface

public class TestMe
{
    public ISomeInterface ISomeInterface { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property_without_interface_prefix_I() => An_issue_is_reported_for(@"

public interface ISomeInterfaceExtended

public class TestMe
{
    public ISomeInterfaceExtended SomeInterfaceExtended { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_getter_only_property() => An_issue_is_reported_for(@"

public interface ISomeInterfaceExtended

public class TestMe
{
    public ISomeInterfaceExtended SomeInterfaceExtended { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_setter_only_property() => An_issue_is_reported_for(@"

public interface ISomeInterfaceExtended

public class TestMe
{
    public ISomeInterfaceExtended SomeInterfaceExtended { set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1069_PropertyNameMeaningAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1069_PropertyNameMeaningAnalyzer();
    }
}