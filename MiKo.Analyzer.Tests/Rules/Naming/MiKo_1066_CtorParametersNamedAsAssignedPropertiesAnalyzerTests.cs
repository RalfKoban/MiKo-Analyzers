using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_primary_constructor_on_record() => No_issue_is_reported_for(@"
public sealed record TestMe(int X, int Y, double Distance);
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(int someProperty) => SomeProperty = someProperty;

    public int SomeProperty { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_parameters() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(int someProperty1, double someProperty2, string someProperty3)
    {
        SomeProperty1 = someProperty1;
        SomeProperty2 = someProperty2;
        SomeProperty3 = someProperty3;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_name_of_single_parameter() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(int someValue) => SomeProperty = someValue;

    public int SomeProperty { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_name_of_specific_parameter() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(int someProperty1, double someProperty, string someProperty3)
    {
        SomeProperty1 = someProperty1;
        SomeProperty2 = someProperty;
        SomeProperty3 = someProperty3;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }
}
");

        [Test]
        public void Code_gets_fixed_for_wrong_name_of_single_parameter()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(int someValue) => SomeProperty = someValue;

    public int SomeProperty { get; }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int someProperty) => SomeProperty = someProperty;

    public int SomeProperty { get; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_wrong_name_of_specific_parameter()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(int a, double b, string c)
    {
        SomeProperty1 = a;
        SomeProperty2 = b;
        SomeProperty3 = c;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int someProperty1, double someProperty2, string someProperty3)
    {
        SomeProperty1 = someProperty1;
        SomeProperty2 = someProperty2;
        SomeProperty3 = someProperty3;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_wrong_name_of_specific_parameters_in_reverse()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(short d, string c, double b, int a)
    {
        SomeProperty1 = a;
        SomeProperty2 = b;
        SomeProperty3 = c;
        SomeProperty4 = d;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }

    public short SomeProperty4 { get; }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(short someProperty4, string someProperty3, double someProperty2, int someProperty1)
    {
        SomeProperty1 = someProperty1;
        SomeProperty2 = someProperty2;
        SomeProperty3 = someProperty3;
        SomeProperty4 = someProperty4;
    }

    public int SomeProperty1 { get; }

    public double SomeProperty2 { get; }

    public string SomeProperty3 { get; }

    public short SomeProperty4 { get; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1066_CtorParametersNamedAsAssignedPropertiesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1066_CodeFixProvider();
    }
}