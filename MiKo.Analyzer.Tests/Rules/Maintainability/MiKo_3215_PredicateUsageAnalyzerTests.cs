using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3215_PredicateUsageAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_([Values("int", "object")] string type) => No_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    void DoSomething(" + type + @" parameter);
}
");

        [Test]
        public void No_issue_is_reported_for_delegate_type_([Values("Action", "Action<bool>", "Func<bool>", "Func<int, bool>")] string type) => No_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    void DoSomething(" + type + @" parameter);
}
");

        [Test]
        public void An_issue_is_reported_for_predicate() => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    void DoSomething(Predicate<int> parameter);
}
");

        [Test]
        public void Code_gets_fixed_for_predicate()
        {
            const string Template = @"
using System;

public interface ITestMe
{
    void DoSomething(### parameter);
}
";

            VerifyCSharpFix(Template.Replace("###", "Predicate<int>"), Template.Replace("###", "Func<int, bool>"));
        }

        protected override string GetDiagnosticId() => MiKo_3215_PredicateUsageAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3215_PredicateUsageAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3215_CodeFixProvider();
    }
}