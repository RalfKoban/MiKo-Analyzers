using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_ternary_operator_that_is_on_single_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition ? true : false);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ternary_operator_if_condition_and_colon_is_placed_on_same_position_as_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_question_is_placed_on_position_before_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                   ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_question_is_placed_on_position_after_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                     ? true
                    : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_colon_is_placed_on_position_before_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                   : false);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ternary_operator_if_colon_is_placed_on_position_after_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                     : false);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_question_is_placed_on_position_before_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                   ? true
                    : false);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_question_is_placed_on_position_after_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                     ? true
                    : false);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_colon_is_placed_on_position_before_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                   : false);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ternary_operator_if_colon_is_placed_on_position_after_condition()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                     : false);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(bool condition)
    {
        DoSomething(condition
                    ? true
                    : false);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6031_CodeFixProvider();
    }
}