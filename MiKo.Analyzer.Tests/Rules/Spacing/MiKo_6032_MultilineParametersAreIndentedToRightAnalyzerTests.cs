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

        [Test]
        public void No_issue_is_reported_for_method_with_parameter_where_modifier_is_inside_conditional_directive_and_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                            ref
#endif
                            int y)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameter_where_type_is_inside_conditional_directive_and_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                            long
#else
                            int
#endif
                            y)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_and_default_values_where_parameters_are_aligned_with_1st_parameter() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Optional] int x = 1,
                            [Optional]
                            int y = 2,
                            [Optional] int z = 3)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_aligned_with_1st_parameter() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In]
                            int x,
                            [In]
                            int y,
                            [In]
                            int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_outdented() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                        [In]
                        int x,
                        [In]
                        int y,
                        [In]
                        int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_aligned_with_1st_parameter() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In] int x,
                            [In] int y,
                            [In] int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_outdented() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                        [In] int x,
                        [In] int y,
                        [In] int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_attributes_where_only_some_attributes_are_on_different_line_than_parameters() => No_issue_is_reported_for(@"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In] int x,
                            [In]
                            int y,
                            int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_default_values_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x = 1, int y = 2)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_default_values_where_default_values_are_on_different_lines_and_aligned_with_1st_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x
                                = 1,
                            int y
                                = 2,
                            int z)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_default_values_where_parameters_are_aligned_with_1st_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x = 1,
                            int y = 2,
                            int z = 3)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parameters_with_default_values_where_parameters_are_outdented() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(
                        int x = 1,
                        int y = 2,
                        int z = 3)
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
        public void An_issue_is_reported_for_method_with_parameter_where_modifier_is_inside_conditional_directive_and_not_aligned() => An_issue_is_reported_for(1, @"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                          ref
#endif
                          int y)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parameter_where_type_is_inside_conditional_directive_and_not_aligned() => An_issue_is_reported_for(1, @"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                          long
#else
                          int
#endif
                          y)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_attributes_and_default_values_where_parameters_are_not_aligned_with_1st_parameter() => An_issue_is_reported_for(2, @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Optional] int x = 1,
                          [Optional]
                          int y = 2,
                                       [Optional] int z = 3)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_not_aligned_with_1st_parameter() => An_issue_is_reported_for(2, @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In]
                            int x,
                          [In]
                          int y,
                                       [In]
                                       int z)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_not_outdented() => An_issue_is_reported_for(3, @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [In]
                            int x,
                            [In]
                            int y,
                            [In]
                            int z)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_not_aligned_with_1st_parameter() => An_issue_is_reported_for(2, @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In] int x,
                          [In] int y,
                                       [In] int z)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_not_outdented() => An_issue_is_reported_for(3, @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [In] int x,
                            [In] int y,
                            [In] int z)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_default_values_where_parameters_are_not_aligned_with_1st_parameter() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething(int x = 1,
                          int y = 2,
                                       int z = 3)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_parameters_with_default_values_where_parameters_are_not_outdented() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething(
                            int x = 1,
                            int y = 2,
                            int z = 3)
    {
    }
}
");

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
        public void Code_gets_fixed_for_method_with_last_parameter_where_attribute_modifier_type_and_identifier_are_each_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(int x,
                            int y,
                              [In]
                       params
                                     int[]
                          values)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(int x,
                            int y,
                            [In] params int[] values)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_last_parameter_where_modifier_type_and_identifier_are_each_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x,
                            int y,
                       params
                                     int[]
                          values)
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
                            params int[] values)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameter_where_modifier_is_inside_conditional_directive_and_type_and_identifier_are_each_on_different_line()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                          ref
#endif
                                    int
                                    y)
    {
    }
}
";

            const string FixedCode = @"
#define FLAG

using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                            ref
#endif
                            int y)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameter_where_type_is_inside_conditional_directive_and_not_aligned_and_directive_is_true()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                        long
#else
                        int
#endif
                                     y)
    {
    }
}
";

            const string FixedCode = @"
#define FLAG

using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                            long
#else
                            int
#endif
                            y)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameter_where_type_is_inside_conditional_directive_and_not_aligned_and_directive_is_false()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                        long
#else
                                  int
#endif
                                     y)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x,
#if FLAG
                            long
#else
                            int
#endif
                            y)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameter_with_default_value_where_modifier_type_identifier_and_default_value_are_each_on_different_line()
        {
            const string OriginalCode = @"
using System;

public static class TestMe
{
    public static void DoSomething(
                        this
                        int
                        x,
                        int
                        y
                        = 1)
    {
    }
}
";

            const string FixedCode = @"
using System;

public static class TestMe
{
    public static void DoSomething(
                               this int x,
                               int y = 1)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_and_default_values_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Optional] int x = 1,
                          [Optional]
                          int y = 2,
                                       [Optional] int z = 3)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([Optional] int x = 1,
                            [Optional] int y = 2,
                            [Optional] int z = 3)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In]
                            int x,
                          [In]
                          int y,
                                       [In]
                                       int z)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In]
                            int x,
                            [In] int y,
                            [In] int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_on_different_line_than_parameters_and_not_outdented()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [In]
                            int x,
                            [In]
                            int y,
                            [In]
                            int z)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                        [In] int x,
                        [In] int y,
                        [In] int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In] int x,
                          [In] int y,
                                       [In] int z)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething([In] int x,
                            [In] int y,
                            [In] int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_on_same_line_as_parameters_and_not_outdented()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                            [In] int x,
                            [In] int y,
                            [In] int z)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                        [In] int x,
                        [In] int y,
                        [In] int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_attributes_where_attributes_are_on_different_lines_and_outdented_parameters_have_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                              [In]
                              int x,
                           [In] int y,
                                       [In]
                                       int z)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public class TestMe
{
    public void DoSomething(
                        [In] int x,
                        [In] int y,
                        [In] int z)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_default_values_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x = 1,
                          int y = 2,
                                       int z = 3)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x = 1,
                            int y = 2,
                            int z = 3)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_default_values_on_different_lines_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x
                                = 1,
                          int y
                                   = 2,
                                       int z = 3)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x
                                = 1,
                            int y = 2,
                            int z = 3)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameters_with_default_values_where_parameters_are_not_outdented()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                            int x = 1,
                            int y = 2,
                            int z = 3)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(
                        int x = 1,
                        int y = 2,
                        int z = 3)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_outdented_extension_method_with_1st_parameter_where_attribute_modifier_type_and_identifier_are_each_on_different_line()
        {
            const string OriginalCode = @"
using System;
using System.Runtime.InteropServices;

public static class TestMe
{
    public static void DoSomething(
                        [In]
                        this
                        int
                        x,
                        int y)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Runtime.InteropServices;

public static class TestMe
{
    public static void DoSomething(
                               [In] this int x,
                               int y)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_outdented_extension_method_with_1st_parameter_where_modifier_type_and_identifier_are_each_on_different_line()
        {
            const string OriginalCode = @"
using System;

public static class TestMe
{
    public static void DoSomething(
                        this
                        int
                        x,
                        int y)
    {
    }
}
";

            const string FixedCode = @"
using System;

public static class TestMe
{
    public static void DoSomething(
                               this int x,
                               int y)
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