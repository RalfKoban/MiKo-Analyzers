using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1077_EnumMemberSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Suffixes = ["Enum", "EnumValue", "EnumMember"];

        [Test]
        public void No_issue_is_reported_for_field_on_type_([Values("interface", "class", "struct", "record")] string type, [ValueSource(nameof(Suffixes))] string suffix) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
    private int my" + suffix + @";
}
");

        [Test]
        public void No_issue_is_reported_for_enum_member_without_suffix_([ValueSource(nameof(Suffixes))] string suffix) => No_issue_is_reported_for(@"
public enum TestMe
{
    " + suffix + @"_None,
}
");

        [Test]
        public void An_issue_is_reported_for_enum_member_with_suffix_([ValueSource(nameof(Suffixes))] string suffix) => An_issue_is_reported_for(@"
public enum TestMe
{
    None" + suffix + @",
}
");

        [Test]
        public void Code_gets_fixed_for_enum_member_with_suffix_([ValueSource(nameof(Suffixes))] string suffix)
        {
            const string Template = @"
public enum TestMe
{
    None###,
}
";

            VerifyCSharpFix(Template.Replace("###", suffix), Template.Replace("###", string.Empty));
        }

        protected override string GetDiagnosticId() => MiKo_1077_EnumMemberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1077_EnumMemberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1077_CodeFixProvider();
    }
}