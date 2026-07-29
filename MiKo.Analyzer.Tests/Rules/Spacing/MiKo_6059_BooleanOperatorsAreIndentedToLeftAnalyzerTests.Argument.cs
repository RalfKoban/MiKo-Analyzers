using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public partial class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_argument_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1 && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_argument_with_operator_after_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2, bool condition3, bool condition4)
    {
        DoSomethingElse(condition1 &&
                        condition2 ||
                        condition3 &&
                        condition4;
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_argument_with_right_operand_directly_below_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                     && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_argument_with_operator_right_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                            && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_argument_with_operator_directly_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                        && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_argument_with_operator_left_of_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                    && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_when_operator_is_right_of_left_operand_for_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                           && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                     && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_operator_is_directly_below_left_operand_for_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                        && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                     && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_operator_is_left_of_left_operand_for_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                  && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        DoSomethingElse(condition1
                     && condition2);
    }

    public void DoSomethingElse(bool condition)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}