using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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
        public void An_issue_is_reported_if_parenthesized_logical_condition_parts_are_all_on_their_own_line_and_combined_condition_is_first() => No_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer();
    }
}