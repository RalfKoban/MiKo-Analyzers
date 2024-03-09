using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_unassigned_static_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private static int m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_unassigned_static_readonly_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private static readonly int m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_assigned_static_readonly_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private static readonly int m_field = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_assigned_non_static_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private int m_field = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_const_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private const int FIELD = 42;
}
");

        [Test]
        public void An_issue_is_reported_for_assigned_static_field() => An_issue_is_reported_for(@"
public class TestMe
{
    private static int m_field = 42;
}
");

        [Test]
        public void Code_gets_fixed_for_assigned_static_field()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static int m_field = 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    private static readonly int m_field = 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3216_CodeFixProvider();
    }
}