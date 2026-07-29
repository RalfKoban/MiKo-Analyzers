using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public partial class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_when_clause_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1 && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_when_clause_with_operator_after_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1 &&
                               s.Length < 10 ||
                               s.Length > 11 &&
                               s.Length <= 42:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_when_clause_with_right_operand_directly_below_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                            && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_when_clause_with_operator_right_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                                 && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_when_clause_with_operator_directly_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                               && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_when_clause_with_operator_left_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                        && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_when_operator_is_right_of_left_operand_for_when_clause()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                                && s.Length < 10:
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
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                            && s.Length < 10:
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
        public void Code_gets_fixed_when_operator_is_directly_below_left_operand_for_when_clause()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                               && s.Length < 10:
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
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                            && s.Length < 10:
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
        public void Code_gets_fixed_when_operator_is_left_of_left_operand_for_when_clause()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                        && s.Length < 10:
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
    public bool DoSomething(object o)
    {
        switch (o)
        {
            case string s when s.Length > 1
                            && s.Length < 10:
                return true;

            default:
                return false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}