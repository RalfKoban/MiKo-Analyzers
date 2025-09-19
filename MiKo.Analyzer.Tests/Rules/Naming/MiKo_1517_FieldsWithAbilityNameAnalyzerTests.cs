using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1517_FieldsWithAbilityNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = Constants.Markers.FieldPrefixes;
        private static readonly string[] Names = ["Visibility"];

        [Test]
        public void No_issue_is_reported_for_boolean_field_without_ability_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private bool something;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_field_with_suffix_([ValueSource(nameof(FieldPrefixes))] string prefix, [ValueSource(nameof(Names))] string name) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private int " + prefix + "something" + name + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_field_with_suffix_([ValueSource(nameof(FieldPrefixes))] string prefix, [ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private bool " + prefix + "something" + name + @";
    }
}
");

        [TestCase("m_visibility", "m_visible")]
        public void Code_gets_fixed_for_field_with_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private bool ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1517_FieldsWithAbilityNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1517_FieldsWithAbilityNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1517_CodeFixProvider();
    }
}