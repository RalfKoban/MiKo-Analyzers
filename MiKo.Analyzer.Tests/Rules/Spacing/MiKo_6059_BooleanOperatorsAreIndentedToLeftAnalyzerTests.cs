using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_if_statement_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1 && condition2)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_statement_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2, bool condition3, bool condition4)
    {
        if (condition1 &&
            condition2 ||
            condition3 &&
            condition4)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_statement_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
         && condition2)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_statement_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
                && condition2)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_statement_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
            && condition2)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_statement_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
           && condition2)
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_if_statement_in_case_operator_is_indented_to_right_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
                && condition2)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
         && condition2)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_if_statement_in_case_operator_is_indented_below_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
            && condition2)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
         && condition2)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_if_statement_in_case_operator_is_outdented_to_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
           && condition2)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        if (condition1
         && condition2)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_return_statement_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1 && condition2;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2, bool condition3, bool condition4)
    {
        return condition1 &&
               condition2 ||
               condition3 &&
               condition4;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
            && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
                && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
               && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
              && condition2;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_return_statement_in_case_operator_is_indented_to_right_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
                && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
            && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_return_statement_in_case_operator_is_indented_below_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
               && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
            && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_return_statement_in_case_operator_is_outdented_to_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
              && condition2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool DoSomething(bool condition1, bool condition2)
    {
        return condition1
            && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

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
        public void No_issue_is_reported_for_when_clause_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_when_clause_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_when_clause_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_when_clause_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_when_clause_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_when_clause_in_case_operator_is_indented_to_right_operand()
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
        public void Code_gets_fixed_for_when_clause_in_case_operator_is_indented_below_left_operand()
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
        public void Code_gets_fixed_for_when_clause_in_case_operator_is_outdented_to_left_operand()
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
        public void No_issue_is_reported_for_variable_declaration_if_complete_operation_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1 && condition2;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2, bool condition3, bool condition4)
    {
        var x = condition1 &&
                condition2 ||
                condition3 &&
                condition4;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
             && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
                    && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
                && condition2;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
            && condition2;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_variable_declaration_in_case_operator_is_indented_to_right_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
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
        var x = condition1
             && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_variable_declaration_in_case_operator_is_indented_below_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
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
        var x = condition1
             && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_variable_declaration_in_case_operator_is_outdented_to_left_operand()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition1, bool condition2)
    {
        var x = condition1
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
        var x = condition1
             && condition2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

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
        public void No_issue_is_reported_for_assignment_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_assignment_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_assignment_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_assignment_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_assignment_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_assignment_in_case_operator_is_indented_to_right_operand()
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
        public void Code_gets_fixed_for_assignment_in_case_operator_is_indented_below_left_operand()
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
        public void Code_gets_fixed_for_assignment_in_case_operator_is_outdented_to_left_operand()
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
        public void No_issue_is_reported_for_argument_in_case_operator_is_behind_left_operand() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_argument_in_case_operator_is_correctly_outdented_to_left_operand() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_argument_in_case_operator_is_indented_to_right_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_argument_in_case_operator_is_indented_below_left_operand() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_argument_in_case_operator_is_outdented_to_left_operand() => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_argument_in_case_operator_is_indented_to_right_operand()
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
        public void Code_gets_fixed_for_argument_in_case_operator_is_indented_below_left_operand()
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
        public void Code_gets_fixed_for_argument_in_case_operator_is_outdented_to_left_operand()
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

        protected override string GetDiagnosticId() => MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6059_CodeFixProvider();
    }
}