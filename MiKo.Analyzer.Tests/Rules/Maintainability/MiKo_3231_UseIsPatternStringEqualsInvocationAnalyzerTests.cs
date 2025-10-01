using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3231_UseIsPatternStringEqualsInvocationAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] IgnorableComparisons =
                                                                [
                                                                    nameof(StringComparison.CurrentCulture),
                                                                    nameof(StringComparison.CurrentCultureIgnoreCase),
                                                                    nameof(StringComparison.InvariantCulture),
                                                                    nameof(StringComparison.InvariantCultureIgnoreCase),
                                                                    nameof(StringComparison.OrdinalIgnoreCase)
                                                                ];

        [Test]
        public void No_issue_is_reported_for_1_variable_of_type_([Values("bool", "char", "int", "object", "StringComparison")] string type) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (a.Equals(42))
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_1_variable_of_type_as_argument_([Values("bool", "char", "int", "object", "StringComparison")] string type) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (42.Equals(a))
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_2_variables_of_type_([Values("bool", "char", "int", "object", "StringComparison")] string type) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(" + type + " a, " + type + @" b)
    {
        if (object.Equals(a, b))
            return true;
        else
            return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_static_string_Equals() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a, string b)
    {
        if (string.Equals(a, b))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_strings_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a, string b)
    {
        if (a.Equals(b, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_variable_strings_compared_with_Ordinal() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a, string b)
    {
        if (a.Equals(b, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_hardcoded_string_as_1st_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (""someValue"".Equals(a, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_hardcoded_string_as_2nd_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (a.Equals(""someValue"", StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_external_constant_string_as_1st_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string a)
    {
        if (Constants.SomeValue.Equals(a, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_external_constant_string_as_2nd_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string a)
    {
        if (a.Equals(Constants.SomeValue, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_internal_constant_string_as_1st_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    private const string SomeValue = ""Some text"";

    public bool DoSomething(string a)
    {
        if (SomeValue.Equals(a, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_internal_constant_string_as_2nd_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    public int SomeValue { get; set; }

    public bool DoSomething(string a)
    {
        if (a.Equals(nameof(SomeValue), StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_nameof_string_as_1st_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    public int SomeValue { get; set; }

    public bool DoSomething(string a)
    {
        if (nameof(SomeValue).Equals(a, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_nameof_string_as_2nd_argument_compared_with_([ValueSource(nameof(IgnorableComparisons))] string comparison) => No_issue_is_reported_for(@"
public class TestMe
{
    private const string SomeValue = ""Some text"";

    public bool DoSomething(string a)
    {
        if (a.Equals(SomeValue, StringComparison." + comparison + @"))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_hardcoded_string_as_1st_argument_compared_without_comparison() => An_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (""someValue"".Equals(a))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_hardcoded_string_as_2nd_argument_compared_without_comparison() => An_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (a.Equals(""someValue""))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_hardcoded_string_as_1st_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (""someValue"".Equals(a, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_hardcoded_string_as_2nd_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    public bool DoSomething(string a)
    {
        if (a.Equals(""someValue"", StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_external_constant_string_as_1st_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string a)
    {
        if (Constants.SomeValue.Equals(a, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_external_constant_string_as_2nd_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class Constants
{
    public const string SomeValue = ""Some text"";
}

public class TestMe
{
    public bool DoSomething(string a)
    {
        if (a.Equals(Constants.SomeValue, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_internal_constant_string_as_1st_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    private const string SomeValue = ""Some text"";

    public bool DoSomething(string a)
    {
        if (SomeValue.Equals(a, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_internal_constant_string_as_2nd_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    private const string SomeValue = ""Some text"";

    public bool DoSomething(string a)
    {
        if (a.Equals(SomeValue, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_nameof_string_as_1st_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    public int SomeValue { get; set; }

    public bool DoSomething(string a)
    {
        if (nameof(SomeValue).Equals(a, StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_nameof_string_as_2nd_argument_compared_with_Ordinal() => An_issue_is_reported_for(@"
public class TestMe
{
    public int SomeValue { get; set; }

    public bool DoSomething(string a)
    {
        if (a.Equals(nameof(SomeValue), StringComparison.Ordinal))
            return true;
        else
            return false;
    }
}");

        [TestCase(@"class TestMe { bool Do(string a) { return a.Equals(""some text"", StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return a is ""some text""; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return ""some text"".Equals(a, StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return a is ""some text""; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return /* ** */ a.Equals(""some text"", StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return /* ** */ a is ""some text""; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return /* ** */ ""some text"".Equals(a, StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return /* ** */ a is ""some text""; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return a.Equals(""some text"", StringComparison.Ordinal) /* ** */; } }", @"class TestMe { bool Do(string a) { return a is ""some text"" /* ** */; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return ""some text"".Equals(a, StringComparison.Ordinal) /* ** */; } }", @"class TestMe { bool Do(string a) { return a is ""some text"" /* ** */; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return !a.Equals(""some text"", StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return a is ""some text"" is false; } }")]
        [TestCase(@"class TestMe { bool Do(string a) { return !""some text"".Equals(a, StringComparison.Ordinal); } }", @"class TestMe { bool Do(string a) { return a is ""some text"" is false; } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [Test]
        public void Code_gets_fixed_for_external_const_string_as_1st_parameter()
        {
            const string OriginalCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => Constants.SomeValue.Equals(s, StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s is Constants.SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_external_const_string_as_2nd_parameter()
        {
            const string OriginalCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s.Equals(Constants.SomeValue, StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s is Constants.SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_internal_const_string_as_1st_parameter()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => SomeValue.Equals(s, StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s is SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_internal_const_string_as_2nd_parameter()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s.Equals(SomeValue, StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s is SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nameof_string_as_1st_parameter()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => nameof(SomeValue).Equals(s, StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s is nameof(SomeValue);
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nameof_string_as_2nd_parameter()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s.Equals(nameof(SomeValue), StringComparison.Ordinal);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s is nameof(SomeValue);
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_external_const_string_as_1st_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => Constants.SomeValue.Equals(s);
                }

                """;

            const string FixedCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s is Constants.SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_external_const_string_as_2nd_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s.Equals(Constants.SomeValue);
                }

                """;

            const string FixedCode = """

                public class Constants
                {
                    public const string SomeValue = "Some text";
                }

                public class TestMe
                {
                    public bool DoSomething(string s) => s is Constants.SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_internal_const_string_as_1st_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => SomeValue.Equals(s);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s is SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_internal_const_string_as_2nd_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s.Equals(SomeValue);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    private const string SomeValue = "Some text";

                    public bool DoSomething(string s) => s is SomeValue;
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nameof_string_as_1st_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => nameof(SomeValue).Equals(s);
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s is nameof(SomeValue);
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nameof_string_as_2nd_parameter_without_comparison()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s.Equals(nameof(SomeValue));
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    public int SomeValue { get; set; }

                    public bool DoSomething(string s) => s is nameof(SomeValue);
                }

                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_conditions_spanning_multiple_lines()
        {
            const string OriginalCode = """

                public class TestMe
                {
                    public bool DoSomething(string s)
                    {
                        return !s.Equals("Sum", StringComparison.Ordinal) &&
                               s.Equals("Something") &&
                               !s.Equals("Product", StringComparison.Ordinal);
                    }
                }

                """;

            const string FixedCode = """

                public class TestMe
                {
                    public bool DoSomething(string s)
                    {
                        return s is "Sum" is false &&
                               s is "Something" &&
                               s is "Product" is false;
                    }
                }
                
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3231_UseIsPatternStringEqualsInvocationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3231_UseIsPatternStringEqualsInvocationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3231_CodeFixProvider();
    }
}