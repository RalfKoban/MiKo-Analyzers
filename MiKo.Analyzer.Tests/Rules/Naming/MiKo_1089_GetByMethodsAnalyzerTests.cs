using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1089_GetByMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Get_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method() => An_issue_is_reported_for(@"
using System;

public class TestMeRepository
{
    public void GetSomething()
    {
    }
}
");

        [TestCase("GetUserById", "UserWithId")]
        [TestCase("GetById", "WithId")]
        [TestCase("GetSomething", "Something")]
        public void Code_gets_fixed_for_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void ###()
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1079_RepositorySuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1089_GetByMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1089_CodeFixProvider();
    }
}