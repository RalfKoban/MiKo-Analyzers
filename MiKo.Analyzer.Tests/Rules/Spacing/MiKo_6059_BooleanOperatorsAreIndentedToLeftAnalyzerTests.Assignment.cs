using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public partial class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_assignment_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1 && condition2;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_assignment_with_operator_after_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2, bool condition3, bool condition4)
    {
        condition1 = condition1 &&
                     condition2 ||
                     condition3 &&
                     condition4;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_assignment_with_right_operand_directly_below_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                  && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_with_operator_right_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                          && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_with_operator_directly_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                     && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_with_operator_left_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
            && condition2;
    }
}
");

        [Test]
        public void Code_gets_fixed_when_operator_is_right_of_left_operand_for_assignment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                        && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                  && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_operator_is_directly_below_left_operand_for_assignment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                     && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                  && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_operator_is_left_of_left_operand_for_assignment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
            && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        condition1 = condition1
                  && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}