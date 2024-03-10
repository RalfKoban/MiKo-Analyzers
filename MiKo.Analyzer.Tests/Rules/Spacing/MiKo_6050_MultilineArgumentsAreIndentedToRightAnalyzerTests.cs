using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_call_with_no_arguments() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_1_argument() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        DoSomething(42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_2_arguments_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        DoSomething(08, 15);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_2_arguments_where_2nd_argument_is_on_different_line_but_same_position_as_1st_argument() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y)
    {
        DoSomething(08,
                    15);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_3_arguments_where_2nd_argument_is_on_different_line_but_same_position_as_1st_argument_and_3rd_argument_on_same_line_as_2nd_argument() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                    08, 15);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_3_arguments_where_3rd_argument_is_on_different_line_but_same_position_as_1st_argument_and_2nd_argument_on_same_line_as_3rd_argument() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(08, 15,
                    42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_3_arguments_where_2nd_and_3rd_argument_are_each_on_different_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                    08,
                    15);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_call_with_3_arguments_where_all_arguments_each_are_on_different_line_and_outdented() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                123456,
                789012,
                -345678);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:argumentMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_method_call_with_3_arguments_where_all_arguments_each_are_on_different_line_and_not_outdented() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                    42,
                    08,
                    15);
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:argumentMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_method_call_with_3_arguments_where_middle_one_is_ternary_operator() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, bool flag)
    {
        DoSomething(
                    42,
                    flag ? 08 : 15,
                    flag);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_call_with_3_arguments_where_last_is_incomplete() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42, 08,
                    );
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_3_arguments_where_all_arguments_each_are_on_different_line_and_not_outdented()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                    123456,
                    789012,
                    -345678);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                123456,
                789012,
                -345678);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_3_arguments_where_all_arguments_each_are_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                  123456,
                            789012,
          -345678);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(
                123456,
                789012,
                -345678);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_3_arguments_where_2nd_and_3rd_argument_are_each_on_different_line_and_messed_up_character_positions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                08,
                         15);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                    08,
                    15);
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_arguments_that_contains_initializer_without_comments()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                new Dictionary<int, int>
                    {
                        { 1, 2 },
                        { 3, 4 },
                        { 5, 6 },
                    });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                        new Dictionary<int, int>
                            {
                                { 1, 2 },
                                { 3, 4 },
                                { 5, 6 },
                            });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_arguments_that_contains_initializer_with_leading_comment()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                new Dictionary<int, int>
                    {
                        { 1, 2 },
                        /* some comment */ { 3, 4 },
                    });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                        new Dictionary<int, int>
                            {
                                { 1, 2 },
                                /* some comment */ { 3, 4 },
                            });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_arguments_that_contains_initializer_with_trailing_comment()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                new Dictionary<int, int>
                    {
                        { 1, 2 },
                        { 3, 4 }, // some comment
                    });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        DoSomethingCore(42,
                        new Dictionary<int, int>
                            {
                                { 1, 2 },
                                { 3, 4 }, // some comment
                            });
    }

    private void DoSomethingCore(int i, Dictionary<int, int> dictionary)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void CodeFixProvider_does_not_crash_when_attempting_to_fix_incomplete_code()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                08,
                         );
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(int x, int y, int z)
    {
        DoSomething(42,
                    08,
                         );
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6050_MultilineArgumentsAreIndentedToRightAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6050_CodeFixProvider();
    }
}