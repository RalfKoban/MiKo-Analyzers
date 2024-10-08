using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6060_SwitchCasesAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_switch_case_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_if_value_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_if_value_is_spanning_multiple_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison
                    .Ordinal:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_if_colon_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal
                :
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_if_value_and_colon_are_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison.Ordinal
                    :
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_case_if_value_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_if_value_is_spanning_multiple_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison
                    .Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_if_colon_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal
                :
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_if_value_and_colon_are_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case
                StringComparison.Ordinal
                    :
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(StringComparison comparison)
    {
        switch (comparison)
        {
            case StringComparison.Ordinal:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_switch_case_pattern_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_case_pattern_with_when_clause_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_case_pattern_with_complex_when_clause_spanning_multiple_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5
                            && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_case_pattern_with_complex_when_clause_including_pattern_spanning_multiple_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o, out int result)
    {
        result = 0;

        switch (o)
        {
            case string s when string.IsNullOrEmpty(s)
                            && s.Length is int i:
            {
                result = i;
                return true;
            }

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_type_of_expression_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                string s:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_instance_of_expression_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string
                    s:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_colon_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s
                :
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_when_clause_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s
                    when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_expression_of_when_clause_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when
                            s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_part_of_expression_of_when_clause_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s
                                .Length > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_comparison_part_of_expression_of_when_clause_is_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length
                                    > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_case_pattern_if_all_parts_are_on_different_line() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                string
                    s
                        when
                            s
                            .Length
                                    > 5:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_type_of_expression_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                string s:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_instance_of_expression_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string
                    s:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_colon_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s
                :
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_when_clause_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s
                    when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_expression_of_when_clause_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when
                            s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_part_of_expression_of_when_clause_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s
                                .Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_comparison_part_of_expression_of_when_clause_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length
                                    > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_all_parts_are_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                string
                    s
                        when
                            s
                            .Length
                                    > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 5:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_argument_list_is_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                int i when i.ToString
                                  (""D"") == ""42"":
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int i when i.ToString(""D"") == ""42"":
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_case_pattern_if_all_arguments_are_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case
                string s when s.Equals
                                    (
                                      ""D""
                                        ,
                                         StringComparison
                                           .
                                             Ordinal
                                    )
                                        is true:
                return true;

            default:
                return false;
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Equals(""D"", StringComparison.Ordinal) is true:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6060_SwitchCasesAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6060_SwitchCasesAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6060_CodeFixProvider();
    }
}