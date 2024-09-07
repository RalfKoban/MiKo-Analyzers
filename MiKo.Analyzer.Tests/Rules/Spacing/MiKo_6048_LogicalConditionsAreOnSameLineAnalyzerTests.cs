using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6048_LogicalConditionsAreOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_logical_condition_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1 || flag2)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_logical_condition_parts_are_all_on_their_own_line_with_condition_on_same_line_as_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1 ||
            flag2)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_logical_condition_parts_are_all_on_their_own_line_with_condition_on_same_line_as_last() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1
         || flag2)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_and_combined_condition_is_first() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if ((flag1 || flag2)
            && flag3)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_and_combined_condition_is_last() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if (flag 3 &&
            (flag1 || flag2))
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_multiple_parenthesized_logical_condition_parts_are_all_on_multiple_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if ((flag1 || flag2)
         && (flag3 || flag4)
         && (flag1 != flag4))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_but_combined_condition_is_on_same_line_as_first() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if (flag 3 && (flag1
                     || flag2))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_but_combined_condition_is_on_same_line_as_last() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if ((flag1
          || flag2) && flag3)
            return;
    }
}
");

        [Test]
        public void An_single_issue_is_reported_if_multiple_parenthesized_logical_condition_parts_are_all_on_multiple_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if ((flag1
          || flag2) && (flag3
          || flag4)
            && (flag1 !=
                flag4))
            return;
    }
}
");

        [Test]
        public void Code_gets_fixed_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_but_combined_condition_is_on_same_line_as_first()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if (flag 3 && (flag1
                     || flag2))
            return;
    }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if (flag 3 && (flag1 || flag2))
            return;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_but_combined_condition_is_on_same_line_as_last()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if ((flag1
          || flag2) && flag3)
            return;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if ((flag1 || flag2) && flag3)
            return;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_but_combined_condition_is_on_same_line_as_last_and_parenthesis_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if (
            (flag1
            || flag2) && flag3
           )
            return;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2, bool flag3)
    {
        if ((flag1 || flag2) && flag3)
            return;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_parenthesized_logical_condition_parts_are_all_on_their_own_lines_and_parenthesis_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public string A, B, D;
    public object C;

    public bool Equals(TestMe other)
    {
        return (
A == other.A || (A != null && A.Equals(other.A, StringComparison.Ordinal)))
&& (
B == other.B || (B != null && B.Equals(other.B, StringComparison.Ordinal)))
&& (
C == other.C || (C != null && C.Equals(other.C)))
&& (
D == other.D || (D != null && D.Equals(other.D, StringComparison.Ordinal))) && base.Equals(other);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public string A, B, D;
    public object C;

    public bool Equals(TestMe other)
    {
        return (A == other.A || (A != null && A.Equals(other.A, StringComparison.Ordinal))) && (B == other.B || (B != null && B.Equals(other.B, StringComparison.Ordinal))) && (C == other.C || (C != null && C.Equals(other.C))) && (D == other.D || (D != null && D.Equals(other.D, StringComparison.Ordinal))) && base.Equals(other);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_parenthesized_logical_condition_parts_are_all_on_their_own_lines_and_parenthesis_are_on_separate_lines_2()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public string A, B;

    public bool Equals(TestMe other)
    {
        return (
                A == other.A
                || (A != null
        && A.Equals(other.A, StringComparison.Ordinal)))
        && (
                        B == other.B
                        || (B != null
        && B.Equals(other.B, StringComparison.Ordinal)));
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public string A, B;

    public bool Equals(TestMe other)
    {
        return (A == other.A || (A != null && A.Equals(other.A, StringComparison.Ordinal))) && (B == other.B || (B != null && B.Equals(other.B, StringComparison.Ordinal)));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        //// TODO RKN: Add tests for inline-comments after condition parts

        protected override string GetDiagnosticId() => MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6048_CodeFixProvider();
    }
}