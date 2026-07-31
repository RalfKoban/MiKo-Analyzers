using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6074_StringConcatenationsAreVerticallyAlignedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_is_placed_on_single_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" + ""with some other text"" + ""and even more text"";
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_vertically_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                   + ""with some other text""
                   + ""and even more text"";
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_vertically_aligned_and_operator_is_at_end_of_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                     ""with some other text"" +
                     ""and even more text"";
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_vertically_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                   + Environment.NewLine
                   + Environment.NewLine
                   + text2;
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_vertically_aligned_and_operator_is_at_end_of_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                     Environment.NewLine +
                     Environment.NewLine +
                     text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_left() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
            + ""with some other text""
            + ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
            ""with some other text"" +
            ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_left() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
            + Environment.NewLine
            + Environment.NewLine
            + text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
            Environment.NewLine +
            Environment.NewLine +
            text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_right() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                     + ""with some other text""
                     + ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                       ""with some other text"" +
                       ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_right() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                     + Environment.NewLine
                     + Environment.NewLine
                     + text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                       Environment.NewLine +
                       Environment.NewLine +
                       text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_all_are_aligned_differentlty_more_to_the_right() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                     + ""with some other text""
                       + ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_spans_multiple_lines_and_all_are_aligned_differentlty_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                       ""with some other text"" +
                         ""and even more text"";
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                     + Environment.NewLine
                       + Environment.NewLine
                         + text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                       Environment.NewLine +
                         Environment.NewLine +
                           text2;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_within_attribute_spans_multiple_lines() => An_issue_is_reported_for(2, @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text""
                        + ""with some other text""
                        + ""and even more text"")]
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_within_attribute_spans_multiple_lines_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text"" +
                          ""with some other text"" +
                          ""and even more text"")]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
            + ""with some other text""
            + ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                   + ""with some other text""
                   + ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
            ""with some other text"" +
            ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                     ""with some other text"" +
                     ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
            + Environment.NewLine
            + Environment.NewLine
            + text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                   + Environment.NewLine
                   + Environment.NewLine
                   + text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
            Environment.NewLine +
            Environment.NewLine +
            text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                     Environment.NewLine +
                     Environment.NewLine +
                     text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                     + ""with some other text""
                     + ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                   + ""with some other text""
                   + ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                       ""with some other text"" +
                       ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                     ""with some other text"" +
                     ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                     + Environment.NewLine
                     + Environment.NewLine
                     + text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                   + Environment.NewLine
                   + Environment.NewLine
                   + text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                       Environment.NewLine +
                       Environment.NewLine +
                       text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                     Environment.NewLine +
                     Environment.NewLine +
                     text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_all_are_aligned_differentlty_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                     + ""with some other text""
                       + ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text""
                   + ""with some other text""
                   + ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_spans_multiple_lines_and_all_are_aligned_differentlty_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                       ""with some other text"" +
                         ""and even more text"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var result = ""some text"" +
                     ""with some other text"" +
                     ""and even more text"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                     + Environment.NewLine
                       + Environment.NewLine
                         + text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text
                   + Environment.NewLine
                   + Environment.NewLine
                   + text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                       Environment.NewLine +
                         Environment.NewLine +
                           text2;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        var result = text +
                     Environment.NewLine +
                     Environment.NewLine +
                     text2;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_is_fixed_if_add_operation_of_string_constants_within_attribute_spans_multiple_lines()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text""
                        + ""with some other text""
                        + ""and even more text"")]
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text""
                      + ""with some other text""
                      + ""and even more text"")]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_is_fixed_if_add_operation_of_string_constants_within_attribute_spans_multiple_lines_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text"" +
                          ""with some other text"" +
                          ""and even more text"")]
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test(Description = ""some text"" +
                        ""with some other text"" +
                        ""and even more text"")]
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_is_placed_on_single_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" + ""with some other text"" + ""and even more text"");
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_vertically_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                        + ""with some other text""
                        + ""and even more text"");
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_vertically_aligned_and_operator_is_at_end_of_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                          ""with some other text"" +
                          ""and even more text"");
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_vertically_aligned() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                        + Environment.NewLine
                        + Environment.NewLine
                        + text2);
    }
}
");

        [Test]
        public void No_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_vertically_aligned_and_operator_is_at_end_of_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                          Environment.NewLine +
                          Environment.NewLine +
                          text2);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_misaligned_but_operator_is_at_begin_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
            + ""with some other text""
            + ""and even more text"");
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_misaligned_but_operator_is_at_begin_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
            + Environment.NewLine
            + Environment.NewLine
            + text2);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
            ""with some other text"" +
            ""and even more text"");
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
            Environment.NewLine +
            Environment.NewLine +
            text2);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                             ""with some other text"" +
                             ""and even more text"");
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                             Environment.NewLine +
                             Environment.NewLine +
                             text2);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                             ""with some other text"" +
                               ""and even more text"");
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                             Environment.NewLine +
                               Environment.NewLine +
                                 text2);
    }
}
");

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
           + ""with some other text""
           + ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                        + ""with some other text""
                        + ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
            ""with some other text"" +
            ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                          ""with some other text"" +
                          ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
          + Environment.NewLine
          + Environment.NewLine
          + text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                        + Environment.NewLine
                        + Environment.NewLine
                        + text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_left_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
            Environment.NewLine +
            Environment.NewLine +
            text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                          Environment.NewLine +
                          Environment.NewLine +
                          text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                            + ""with some other text""
                            + ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                        + ""with some other text""
                        + ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                             ""with some other text"" +
                             ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                          ""with some other text"" +
                          ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                           + Environment.NewLine
                           + Environment.NewLine
                           + text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                        + Environment.NewLine
                        + Environment.NewLine
                        + text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_is_aligned_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                             Environment.NewLine +
                             Environment.NewLine +
                             text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                          Environment.NewLine +
                          Environment.NewLine +
                          text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                            + ""with some other text""
                              + ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text""
                        + ""with some other text""
                        + ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_constants_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                             ""with some other text"" +
                               ""and even more text"");
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""some text"" +
                          ""with some other text"" +
                          ""and even more text"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                           + Environment.NewLine
                             + Environment.NewLine
                               + text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1
                        + Environment.NewLine
                        + Environment.NewLine
                        + text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_add_operation_of_string_variables_used_as_method_argument_spans_multiple_lines_and_all_are_aligned_differently_more_to_the_right_and_operator_is_at_end_of_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                             Environment.NewLine +
                               Environment.NewLine +
                                 text2);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var text1 = ""some text"";
        var text2 = ""some other text"";

        Console.WriteLine(text1 +
                          Environment.NewLine +
                          Environment.NewLine +
                          text2);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6074_StringConcatenationsAreVerticallyAlignedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6074_StringConcatenationsAreVerticallyAlignedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6074_CodeFixProvider();
    }
}