using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_property_with_meaningful_name() => No_issue_is_reported_for(@"
public class TestMe
{
    public int X { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_imported_property() => No_issue_is_reported_for(@"
using System.Composition;

public interface ISomeInterfaceExtended

public class TestMe
{
    [Import]
    public ISomeInterfaceExtended SomeInterfaceExtended { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_named_exactly_like_its_interface_type() => An_issue_is_reported_for(@"

public interface ISomeInterface

public class TestMe
{
    public ISomeInterface ISomeInterface { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_named_like_interface_type_without_leading_I() => An_issue_is_reported_for(@"

public interface ISomeInterfaceExtended

public class TestMe
{
    public ISomeInterfaceExtended SomeInterfaceExtended { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_getter_only_property_named_like_interface_type_without_leading_I() => An_issue_is_reported_for(@"

public interface ISomeInterfaceExtended

public class TestMe
{
    public ISomeInterfaceExtended SomeInterfaceExtended { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_setter_only_property_named_like_interface_type_without_leading_I() => An_issue_is_reported_for(@"

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