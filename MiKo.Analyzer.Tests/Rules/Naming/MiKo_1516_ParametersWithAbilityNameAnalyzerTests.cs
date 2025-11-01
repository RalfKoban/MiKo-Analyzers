using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1516_ParametersWithAbilityNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Names = ["Visibility"];

        [Test]
        public void No_issue_is_reported_for_boolean_parameter_without_ability_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_parameter_with_suffix_([ValueSource(nameof(Names))] string name) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void DoSomething(int something" + name + @") { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_parameter_with_Condition_suffix_([ValueSource(nameof(Names))] string name) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void DoSomething(int something" + name + @"Condition) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nullable_non_boolean_parameter_with_Condition_suffix_([ValueSource(nameof(Names))] string name) => No_issue_is_reported_for(@"
#nullable enable

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void DoSomething(string? something" + name + @"Condition) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_parameter_with_suffix_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void DoSomething(bool something" + name + @") { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_parameter_with_Condition_suffix_([ValueSource(nameof(Names))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        public void DoSomething(bool something" + name + @"Condition) { }
    }
}
");

        [TestCase("visibility", "visible")]
        [TestCase("visibilityCondition", "visible")]
        public void Code_gets_fixed_for_parameter_with_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool ###) { }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1516_ParametersWithAbilityNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1516_ParametersWithAbilityNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1516_CodeFixProvider();
    }
}