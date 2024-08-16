using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3220_LogicalConditionsUsingTrueFalseCanBeSimplifiedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_logical_condition() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null)
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_logical_condition_not_using_boolean_directly_([Values("||", "&&")] string @operator) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool b1, bool b2)
    {
        if (b1 " + @operator + @" b2)
        { }
    }
}
");

        [TestCase("b && true")]
        [TestCase("b && false")]
        [TestCase("b || true")]
        [TestCase("b || false")]
        [TestCase("true && b")]
        [TestCase("false && b")]
        [TestCase("true || b")]
        [TestCase("false || b")]
        public void An_issue_is_reported_for_logical_condition_using_boolean_directly_(string condition) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool b)
    {
        if (" + condition + @")
        { }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [TestCase("true && true")]
        [TestCase("true && false")]
        [TestCase("false && true")]
        [TestCase("false && false")]
        public void An_issue_is_reported_for_logical_condition_using_both_booleans_directly_(string condition) => An_issue_is_reported_for(2, @"
public class TestMe
{
    public void DoSomething(bool b)
    {
        if (" + condition + @")
        { }
    }
}
");

        [TestCase("b && true", "b")]
        [TestCase("b && false", "false")]
        [TestCase("b || true", "b")]
        [TestCase("b || false", "b")]
        [TestCase("true && b", "b")]
        [TestCase("false && b", "false")]
        [TestCase("true || b", "b")]
        [TestCase("false || b", "b")]
        [TestCase("true && true", "true")]
        [TestCase("true && false", "false")]
        [TestCase("false && true", "false")]
        [TestCase("false && false", "false")]
        public void Code_gets_fixed_for_logical_condition_using_boolean_directly_(string originalCondition, string fixedCondition)
        {
            const string Template = @"
public class TestMe
{
    public void DoSomething(bool b)
    {
        if (###)
        { }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalCondition), Template.Replace("###", fixedCondition));
        }

        protected override string GetDiagnosticId() => MiKo_3220_LogicalConditionsUsingTrueFalseCanBeSimplifiedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3220_LogicalConditionsUsingTrueFalseCanBeSimplifiedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3220_CodeFixProvider();
    }
}