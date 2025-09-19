using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1515_PropertiesWithAbilityNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Names = ["Visibility"];

        [Test]
        public void No_issue_is_reported_for_boolean_property_without_ability_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public bool Something { get; set; }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_property_with_suffix_([ValueSource(nameof(Names))] string name) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public int " + name + @" { get; set; }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_property_with_suffix_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public bool " + name + @" { get; set; }
    }
}
");

        [TestCase("Visibility", "Visible")]
        public void Code_gets_fixed_for_property_with_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        public bool ### { get; set; }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1515_PropertiesWithAbilityNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1515_PropertiesWithAbilityNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1515_CodeFixProvider();
    }
}