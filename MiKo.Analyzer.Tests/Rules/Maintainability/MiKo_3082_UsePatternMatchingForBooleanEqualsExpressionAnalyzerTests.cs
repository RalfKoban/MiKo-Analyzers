using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Operators =
                                                     {
                                                         "==",
                                                         "!=",
                                                     };

        private static readonly string[] BooleanValues =
                                                         {
                                                             "true",
                                                             "false",
                                                         };

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_an_(
                                                            [Values("int", "string", "object")] string type,
                                                            [ValueSource(nameof(Operators))] string @operator)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + " a, " + type + @" b)
    {
        if (a " + @operator + @" b)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_comparisons_of_2_boolean_values_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a, bool b)
    {
        if (a " + @operator + @" b)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_boolean_to_([ValueSource(nameof(BooleanValues))] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (" + value + @" == a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_right_sided_comparison_of_a_boolean_to_([ValueSource(nameof(BooleanValues))] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (a == " + value + @")
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_Expression_argument([ValueSource(nameof(BooleanValues))] string value) => No_issue_is_reported_for(@"
using System;
using System.Linq.Expressions;

public class TestMe
{
    public bool DoSomething(bool a) => DoSomething(_ => _ == " + value + @");

    public bool DoSomething(Expression<Func<bool, bool>> expression) => expression != null;
}");

        [TestCase("class TestMe { bool Do(bool a) { return (a == true); } }", "class TestMe { bool Do(bool a) { return (a is true); } }")]
        [TestCase("class TestMe { bool Do(bool a) { return (a == false); } }", "class TestMe { bool Do(bool a) { return (a is false); } }")]
        [TestCase("class TestMe { bool Do(bool a) => a == true; }", "class TestMe { bool Do(bool a) => a is true; }")]
        [TestCase("class TestMe { bool Do(bool a) => a == false; }", "class TestMe { bool Do(bool a) => a is false; }")]
        [TestCase("class TestMe { bool Do(bool a) { return (true == a); } }", "class TestMe { bool Do(bool a) { return (a is true); } }")]
        [TestCase("class TestMe { bool Do(bool a) { return (false == a); } }", "class TestMe { bool Do(bool a) { return (a is false); } }")]
        [TestCase("class TestMe { bool Do(bool a) => true == a; }", "class TestMe { bool Do(bool a) => a is true; }")]
        [TestCase("class TestMe { bool Do(bool a) => false == a; }", "class TestMe { bool Do(bool a) => a is false; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3082_UsePatternMatchingForBooleanEqualsExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3082_CodeFixProvider();
    }
}