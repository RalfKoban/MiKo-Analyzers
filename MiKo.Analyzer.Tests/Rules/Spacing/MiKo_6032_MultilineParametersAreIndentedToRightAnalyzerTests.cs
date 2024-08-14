using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6032_MultilineParametersAreIndentedToRightAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_no_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_1_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_2_parameters_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_2_parameters_where_2nd_parameter_is_on_different_line_but_same_position_as_1st_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x,
                            int y)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_3_parameters_where_2nd_parameter_is_on_different_line_but_same_position_as_1st_parameter_and_3rd_parameter_on_same_line_as_2nd_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x,
                            int y, int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_3_parameters_where_3rd_parameter_is_on_different_line_but_same_position_as_1st_parameter_and_2nd_parameter_on_same_line_as_3rd_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y,
                            int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_3_parameters_where_2nd_and_3rd_parameter_are_each_on_different_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x,
                            int y,
                            int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_3_parameters_where_all_parameters_each_are_on_different_line_and_outdented() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(
                        int x,
                        int y,
                        int z)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_3_parameters_where_all_parameters_each_are_on_different_line_and_not_outdented() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething(
                            int x,
                            int y,
                            int z)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_3_parameters_where_all_parameters_each_are_on_different_line_and_not_outdented()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                            int x,
                            int y,
                            int z)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                        int x,
                        int y,
                        int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_3_parameters_where_all_parameters_each_are_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                              int x,
                           int y,
                                       int z)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                        int x,
                        int y,
                        int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_3_parameters_where_2nd_and_3rd_parameter_are_each_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x,
                          int y,
                                       int z)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x,
                            int y,
                            int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6032_CodeFixProvider();
    }
}