using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1079_RepositorySuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_repository_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_repository_class_that_cannot_be_renamed() => No_issue_is_reported_for(@"
using System;

public class Repository
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_repository_class() => An_issue_is_reported_for(@"
using System;

public class TestMeRepository
{
    public void DoSomething()
    {
    }
}
");

        [TestCase("UserRepository", "Users")]
        [TestCase("ChildRepository", "Children")]
        public void Code_gets_fixed_for_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class ###
{
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1079_RepositorySuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1079_RepositorySuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1079_CodeFixProvider();
    }
}