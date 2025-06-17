﻿using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public sealed class MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_there_is_no_multi_line_call_chain() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_call_chain_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString().ToString().ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_multi_line_call_chain_is_indented_correctly() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_multi_line_call_chain_is_mixed_but_indented_correctly() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString().ToString()
                 .ToString();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_multi_line_call_chain_is_mixed_with_member_binding_but_indented_correctly() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                 .ToString()?
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_calls_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                  .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_calls_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                .ToString()
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_members_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Type type)
    {
        var x = type.Assembly
             .EntryPoint
                    .ReturnParameter;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_contains_members_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Type type)
    {
        var x = type.Assembly
                        .EntryPoint
                    .ReturnParameter;
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_is_mixed_with_member_binding_and_access_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_is_mixed_with_member_binding_and_access_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                  .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_is_mixed_with_member_binding_and_binding_indented_more_to_the_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                .ToString()?
                 .ToString();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_multi_line_call_chain_is_mixed_with_member_binding_and_binding_indented_more_to_the_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                  .ToString()?
                 .ToString();
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_each_wrong_indented_call() => An_issue_is_reported_for(2, @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomething(new Dictionary<int, int>
                                            {
                                                { 1, 2 },
                                            })
                                            .DoSomethingMore()
                                    .DoSomething(new Dictionary<int, int>
                                                     {
                                                         { 3, 4 },
                                                     });
    }

    public TestMe DoSomething(Dictionary<int, int> source)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
");

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                  .GetHashCode()
                 .GetType();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                .GetHashCode()
                 .GetType();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_calls_indented_more_to_the_left_and_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                .GetHashCode()
             .GetType()
                  .GetElementType()
                    .GetEnumName();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .GetHashCode()
                 .GetType()
                 .GetElementType()
                 .GetEnumName();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_contains_members_indented_more_to_the_left_and_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(Type type)
    {
        var x = type.Assembly
             .EntryPoint
                  .ReturnParameter
                    .Attributes;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(Type type)
    {
        var x = type.Assembly
                    .EntryPoint
                    .ReturnParameter
                    .Attributes;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_adjusts_multi_line_arguments_as_well()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomething(new Dictionary<int, int>
                                            {
                                                { 1, 2 },
                                            })
                                            .DoSomethingMore()
                                    .DoSomething(new Dictionary<int, int>
                                                     {
                                                         { 3, 4 },
                                                     });
    }

    public TestMe DoSomething(Dictionary<int, int> source)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomething(new Dictionary<int, int>
                                            {
                                                { 1, 2 },
                                            })
                           .DoSomethingMore()
                           .DoSomething(new Dictionary<int, int>
                                            {
                                                { 3, 4 },
                                            });
    }

    public TestMe DoSomething(Dictionary<int, int> source)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_adjusts_single_line_arguments_as_well()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
           .DoSomething(1, 2, 3)
           .DoSomething(4, 5, 6)
           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
                           .DoSomething(1, 2, 3)
                           .DoSomething(4, 5, 6)
                           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_adjusts_but_keeps_comment_with_empty_line()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
           .DoSomething(1, 2, 3)
           .DoSomething(4, 5, 6)

           // some comment here
           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
                           .DoSomething(1, 2, 3)
                           .DoSomething(4, 5, 6)

                           // some comment here
                           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_and_adjusts_but_keeps_multiple_comments_with_empty_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
           .DoSomething(1, 2, 3)
           .DoSomething(4, 5, 6)

           // some comment
           // some comment here
           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe Start()
    {
        return new TestMe().DoSomethingMore()
                           .DoSomething(1, 2, 3)
                           .DoSomething(4, 5, 6)

                           // some comment
                           // some comment here
                           .DoSomething(7, 8, 9);
    }

    public TestMe DoSomething(int x, int y, int z)
    {
        return this;
    }

    public TestMe DoSomethingMore()
    {
        return this;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_is_mixed_with_member_binding_and_access_indented_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                .ToString();
    }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                 .ToString();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_is_mixed_with_member_binding_and_access_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                  .ToString();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                 .ToString();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_is_mixed_with_member_binding_and_binding_indented_more_to_the_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                .ToString()?
                 .ToString();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                 .ToString();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_is_mixed_with_member_binding_and_binding_indented_more_to_the_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                  .ToString()?
                 .ToString();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        var x = o.ToString()
                 .ToString()?
                 .ToString();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_multi_line_call_chain_is_long()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        return o.ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString()
                   .ToString();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string DoSomething(object o)
    {
        return o.ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString()
                .ToString();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6040_CodeFixProvider();
    }
}