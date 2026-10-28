using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6077_SingleParameterListsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_parameter_list_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_single_parameter_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_single_parameter_of_generic_method_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>(T item) { }
}
");

        [Test]
        public void No_issue_is_reported_for_single_parameter_of_constructor_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_parameters_on_multiple_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(
                            int i,
                            string s) { }
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_generic_method_is_on_next_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>
                              (T item) { }
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_method_is_on_next_line() => No_issue_is_reported_for(@"
        {
public class TestMe
{
    public void DoSomething
                           (int i) { }
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_unary_operator_is_on_next_line() => No_issue_is_reported_for(@"
        {
public class TestMe
{
    public static TestMe operator ++
                                   (TestMe value) => value;
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_constructor_is_on_next_line() => No_issue_is_reported_for(@"
        {
public class TestMe
{
    public TestMe
                 (int i) { }
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_destructor_is_on_next_line() => No_issue_is_reported_for(@"
        {
public class TestMe
{
    ~TestMe
           () { }
}
");

        [Test]
        public void No_issue_is_reported_when_parameter_list_of_conversion_operator_is_on_next_line() => No_issue_is_reported_for(@"
        {
public class TestMe
{
    public static explicit operator int
                                       (TestMe value) => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_single_parameter_with_attribute_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber] int i) { }
}
");

        [Test]
        public void An_issue_is_reported_when_empty_parameter_list_is_on_multiple_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(
                            ) { }
}
");

        [Test]
        public void An_issue_is_reported_when_single_parameter_is_on_next_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(
                            int i) { }
}
");

        [Test]
        public void An_issue_is_reported_when_closing_parenthesis_of_single_parameter_is_on_next_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i
                            ) { }
}
");

        [Test]
        public void An_issue_is_reported_when_single_parameter_has_parentheses_on_separate_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(
                            int i
                            ) { }
}
");

        [Test]
        public void An_issue_is_reported_when_single_parameter_of_generic_method_is_on_next_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>(
                               T item) { }
}
");

        [Test]
        public void An_issue_is_reported_when_single_parameter_of_constructor_is_on_next_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe(
                  int i) { }
}
");

        [Test]
        public void An_issue_is_reported_when_single_parameter_with_attribute_is_on_next_line() => An_issue_is_reported_for(@"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething(
                            [CallerLineNumber] int i) { }
}
");

        [Test]
        public void An_issue_is_reported_when_attribute_of_single_parameter_is_on_separate_line() => An_issue_is_reported_for(@"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber]
                            int i) { }
}
");

        [Test]
        public void Code_gets_fixed_when_empty_parameter_list_is_on_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(
                            ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(
                            int i) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_closing_parenthesis_of_single_parameter_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i
                            ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_has_parentheses_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(
                            int i
                            ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_of_generic_method_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T>(
                               T item) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T>(T item) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_of_constructor_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(
                  int i) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_generic_type_is_on_next_line()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(
                            Dictionary<int, string> items) { }
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Dictionary<int, string> items) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_generic_type_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(
                            Dictionary
                                    <
                                     int
                                        ,
                                          string
                                                >
                                                  items
                                                        ) { }
}
";

            const string FixedCode = @"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Dictionary<int, string> items) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parameter_of_unary_operator_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static TestMe operator -(
                                    TestMe value) => value;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static TestMe operator -(TestMe value) => value;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_closing_parenthesis_of_unary_operator_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static bool operator !(TestMe value
                                  ) => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static bool operator !(TestMe value) => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parentheses_of_unary_operator_are_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static bool operator true(
                                     TestMe value
                                     ) => true;

    public static bool operator false(TestMe value) => false;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static bool operator true(TestMe value) => true;

    public static bool operator false(TestMe value) => false;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_empty_parameter_list_of_constructor_is_on_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(
                  ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_closing_parenthesis_of_constructor_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(int i
                  ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parentheses_of_constructor_are_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(
                  int i
                  ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parameter_of_constructor_with_initializer_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe(
                  int i) : this() { }

    public TestMe() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe(int i) : this() { }

    public TestMe() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parameter_list_of_destructor_is_on_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    ~TestMe(
            ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    ~TestMe() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parameter_of_implicit_conversion_operator_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static implicit operator int(
                                        TestMe value) => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static implicit operator int(TestMe value) => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parameter_of_explicit_conversion_operator_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static explicit operator int(
                                        TestMe value) => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static explicit operator int(TestMe value) => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_closing_parenthesis_of_conversion_operator_is_on_next_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static implicit operator int(TestMe value
                                        ) => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static implicit operator int(TestMe value) => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_parentheses_of_conversion_operator_are_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public static implicit operator int(
                                        TestMe value
                                        ) => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public static implicit operator int(TestMe value) => 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_attribute_is_on_next_line()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething(
                            [CallerLineNumber] int i) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_attribute_of_single_parameter_is_on_separate_line()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber]
                            int i) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_attribute_and_parameter_are_on_separate_lines_than_parentheses()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething(
                            [CallerLineNumber]
                            int i
                            ) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_attribute_with_argument_of_single_parameter_is_on_multiple_lines()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething(
                            [CallerLineNumber(
                                      ""message"")]
                            int i) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber(""message"")] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_multiple_attributes_of_single_parameter_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething(
                            [CallerLineNumber]
                            [CLSCompliant(true)]
                            int i) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething([CallerLineNumber][CLSCompliant(true)] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_attribute_of_generic_method_is_on_next_line()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething<T>(
                               [CallerLineNumber] T item) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public void DoSomething<T>([CallerLineNumber] T item) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_attribute_of_constructor_is_on_next_line()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public TestMe(
                  [CallerLineNumber] int i) { }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.CompilerServices;

public class TestMe
{
    public TestMe([CallerLineNumber] int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_modifier_is_on_next_line_([Values("out", "ref", "in", "ref readonly")] string modifier)
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(
                            ### int i) { i = 0; }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(### int i) { i = 0; }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", modifier), FixedCode.Replace("###", modifier));
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_modifier_is_on_separate_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(ref
                            int i) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(ref int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_modifier_has_parentheses_on_separate_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(
                            in int i
                            ) { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(in int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_attribute_and_modifier_is_on_next_line()
        {
            const string OriginalCode = @"
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [Out] ref int i) { }
}
";

            const string FixedCode = @"
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Out] ref int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_single_parameter_with_attribute_and_modifier_is_on_separate_lines()
        {
            const string OriginalCode = @"
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [Out]
                            ref
                            int i
                            ) { }
}
";

            const string FixedCode = @"
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Out] ref int i) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6077_SingleParameterListsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6077_SingleParameterListsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6077_CodeFixProvider();
    }
}
