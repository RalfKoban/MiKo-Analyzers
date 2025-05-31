using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3227_UsePatternMatchingForLiteralEqualsExpressionAnalyzerTests : CodeFixVerifier
    {
        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_2_variables_of_type_(
                                                                             [Values("bool", "char", "int", "string", "object", "StringComparison")] string type,
                                                                             [Values("==", "!=")] string @operator)
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
}
");

        [Test]
        public void No_issue_is_reported_for_on_lambda_with_enum_property_access() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class Dto
{
    public StringComparison Comparison;
}

public class TestMe
{
    public bool DoSomething(IEnumerable<Dto> dto, StringComparison comparison)
    {
        if (dto.Any(_ => _.Comparison == comparison))
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_on_lambda_with_nested_enum_property_access() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class Dto
{
    public SubDto Nested;
}

public class SubDto
{
    public StringComparison Comparison;
}

public class TestMe
{
    public bool DoSomething(IEnumerable<Dto> dto, StringComparison comparison)
    {
        if (dto.Any(_ => _.Nested.Comparison == comparison))
            return true;
        else
            return false;
    }
}
");

        [TestCase("int", "42")]
        [TestCase("int", "-42")]
        [TestCase("char", "'X'")]
        [TestCase("string", @"""some text""")]
        [TestCase("StringComparison", "StringComparison.Ordinal")]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_(string type, string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (" + value + @" == a)
            return true;
        else
            return false;
    }
}
");

        [TestCase("int", "42")]
        [TestCase("int", "-42")]
        [TestCase("char", "'X'")]
        [TestCase("string", @"""some text""")]
        [TestCase("StringComparison", "StringComparison.Ordinal")]
        public void An_issue_is_reported_for_a_right_sided_comparison_of_(string type, string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (a == " + value + @")
            return true;
        else
            return false;
    }
}
");

        [TestCase("class TestMe { bool Do(int a) { return (a == 42); } }", "class TestMe { bool Do(int a) { return (a is 42); } }")]
        [TestCase("class TestMe { bool Do(int a) { return (42 == a); } }", "class TestMe { bool Do(int a) { return (a is 42); } }")]
        [TestCase("class TestMe { bool Do(int a) => a == 42; }", "class TestMe { bool Do(int a) => a is 42; }")]
        [TestCase("class TestMe { bool Do(int a) => 42 == a; }", "class TestMe { bool Do(int a) => a is 42; }")]

        [TestCase("class TestMe { bool Do(int a) { return (a == -42); } }", "class TestMe { bool Do(int a) { return (a is -42); } }")]
        [TestCase("class TestMe { bool Do(int a) { return (-42 == a); } }", "class TestMe { bool Do(int a) { return (a is -42); } }")]
        [TestCase("class TestMe { bool Do(int a) => a == -42; }", "class TestMe { bool Do(int a) => a is -42; }")]
        [TestCase("class TestMe { bool Do(int a) => -42 == a; }", "class TestMe { bool Do(int a) => a is -42; }")]

        [TestCase("class TestMe { bool Do(char a) { return (a == 'X'); } }", "class TestMe { bool Do(char a) { return (a is 'X'); } }")]
        [TestCase("class TestMe { bool Do(char a) { return ('X' == a); } }", "class TestMe { bool Do(char a) { return (a is 'X'); } }")]
        [TestCase("class TestMe { bool Do(char a) => a == 'X'; }", "class TestMe { bool Do(char a) => a is 'X'; }")]
        [TestCase("class TestMe { bool Do(char a) => 'X' == a; }", "class TestMe { bool Do(char a) => a is 'X'; }")]

        [TestCase(@"class TestMe { bool Do(string a) { return (a == ""some text""); } }", @"class TestMe { bool Do(string a) { return (a is ""some text""); } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return (""some text"" == a); } }", @"class TestMe { bool Do(string a) { return (a is ""some text""); } }")]
        [TestCase(@"class TestMe { bool Do(string a) => a == ""some text""; }", @"class TestMe { bool Do(string a) => a is ""some text""; }")]
        [TestCase(@"class TestMe { bool Do(string a) => ""some text"" == a; }", @"class TestMe { bool Do(string a) => a is ""some text""; }")]

        [TestCase("using System; class TestMe { bool Do(StringComparison a) { return (a == StringComparison.Ordinal); } }", "using System; class TestMe { bool Do(StringComparison a) { return (a is StringComparison.Ordinal); } }")]
        [TestCase("using System; class TestMe { bool Do(StringComparison a) { return (StringComparison.Ordinal == a); } }", "using System; class TestMe { bool Do(StringComparison a) { return (a is StringComparison.Ordinal); } }")]
        [TestCase("using System; class TestMe { bool Do(StringComparison a) => a == StringComparison.Ordinal; }", "using System; class TestMe { bool Do(StringComparison a) => a is StringComparison.Ordinal; }")]
        [TestCase("using System; class TestMe { bool Do(StringComparison a) => StringComparison.Ordinal == a; }", "using System; class TestMe { bool Do(StringComparison a) => a is StringComparison.Ordinal; }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [Test]
        public void Code_gets_fixed_for_multiline_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison, object o1, object o2) => o1 != null
                                                                               && comparison == StringComparison.Ordinal
                                                                               && o2 != null;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(StringComparison comparison, object o1, object o2) => o1 != null
                                                                               && comparison is StringComparison.Ordinal
                                                                               && o2 != null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_const_string()
        {
            const string OriginalCode = @"
using System;

public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string s) => s == Constants.SomeValue;
}
";

            const string FixedCode = @"
using System;

public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string s) => s is Constants.SomeValue;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3227_UsePatternMatchingForLiteralEqualsExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3227_UsePatternMatchingForLiteralEqualsExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3227_CodeFixProvider();
    }
}