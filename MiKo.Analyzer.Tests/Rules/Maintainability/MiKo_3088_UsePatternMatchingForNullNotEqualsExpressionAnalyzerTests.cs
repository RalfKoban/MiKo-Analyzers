using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3088_UsePatternMatchingForNullNotEqualsExpressionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Operators = ["==", "!="];

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_2_variables_of_type_([Values("int", "string", "object")] string type, [ValueSource(nameof(Operators))] string @operator)
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
}", languageVersion: LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void No_issue_is_reported_for_a_left_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (null != a)
            return true;
        else
            return false;
    }
}", languageVersion: LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (a != null)
            return true;
        else
            return false;
    }
}", languageVersion: LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void No_issue_is_reported_for_Expression_argument() => No_issue_is_reported_for(@"
using System;
using System.Linq.Expressions;

public class TestMe
{
    public bool DoSomething(obj o) => DoSomething(_ => _ != null);

    public bool DoSomething(Expression<Func<o, bool>> expression) => expression == null;
}", languageVersion: LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (null != a)
            return true;
        else
            return false;
    }
}", languageVersion: LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_a_right_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (a != null)
            return true;
        else
            return false;
    }
}", languageVersion: LanguageVersion.CSharp9);

        [TestCase("class TestMe { bool Do(object a) { return (a != null); } }", "class TestMe { bool Do(object a) { return (a is not null); } }")]
        [TestCase("class TestMe { bool Do(object a) { return (null != a); } }", "class TestMe { bool Do(object a) { return (a is not null); } }")]
        [TestCase("class TestMe { bool Do(object a) => a != null; }", "class TestMe { bool Do(object a) => a is not null; }")]
        [TestCase("class TestMe { bool Do(object a) => null != a; }", "class TestMe { bool Do(object a) => a is not null; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode, LanguageVersion.CSharp9);

        protected override string GetDiagnosticId() => MiKo_3088_UsePatternMatchingForNullNotEqualsExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3088_UsePatternMatchingForNullNotEqualsExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3088_CodeFixProvider();
    }
}