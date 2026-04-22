using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3504_TrainWreckAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_a_number_assignment() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        var value = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_StringBuilder_build_chain() => No_issue_is_reported_for("""
                                                                                                       using System;
                                                                                                       using System.Text;

                                                                                                       public class TestMe
                                                                                                       {
                                                                                                           public void DoSomething()
                                                                                                           {
                                                                                                               var value = new StringBuilder()
                                                                                                                                        .Append('A')
                                                                                                                                        .Append('B')
                                                                                                                                        .Append('C')
                                                                                                                                        .Append('D')
                                                                                                                                        .Append('E')
                                                                                                                                        .Append('F')
                                                                                                                                        .Append('G')
                                                                                                                                        .ToString();
                                                                                                           }
                                                                                                       }

                                                                                                       """);

        [Test]
        public void No_issue_is_reported_for_event_registration_when_used_with_full_qualified_names() => No_issue_is_reported_for(@"
using System;

namespace This.Is.My.Namespace
{
    public class TestMeData
    {
        public EventHandler MyEvent;
    }
}

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            This.Is.My.Namespace.TestMeData.MyEvent += OnMyEvent;
        }

        private void OnMyEvent(object sender, EventArgs e)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_2_properties_in_a_row_when_invoking_a_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        A.B.DoSomething();
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_2_properties_in_a_row_when_invoking_a_method_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public int Invocation() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_2_array_elements_in_a_row_when_invoking_a_method_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public int Invocation() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_properties_in_a_row_when_invoking_a_method_inside_using_declaration_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        using var x = A.B.C.DoSomething();
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public IDisposable DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_properties_in_a_row_when_invoking_a_method_inside_using_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        using (A.B.C.DoSomething())
        {
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public IDisposable DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_properties_in_a_row_when_invoking_a_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        A.B.C.DoSomething();
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_properties_in_a_row_when_invoking_a_method_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int Invocation() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_array_elements_in_a_row_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].C[3]);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public int[] C { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_3_array_elements_in_a_row_when_invoking_a_method_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].C[3].Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public TestMeC[] C { get; }
}

public class TestMeC
{
    public int Invocation() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_assigning_a_value() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var value = A.B.C.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_assigning_a_value_using_conditional_access() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var value = A?.B?.C?.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_prefix_unary_expression() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(-A.B.C.D);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_postfix_unary_expression() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D++);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_binary_expression_left_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var flag = false;

        DoSomething(A.B.C.D || flag);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_binary_expression_right_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var flag = false;

        DoSomething(flag || A.B.C.D);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_binary_expression_in_comparison_left_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D == 42);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_binary_expression_in_comparison_right_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(42 == A.B.C.D);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_an_argument_using_is_pattern_in_comparison() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D is 42);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_switch() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        switch (A.B.C.D)
        {
            case 42: break;
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_switch_when() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(object o)
    {
        switch (o)
        {
            case TestMeA a when a.B.C.D:
                break;
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_switch_expression() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public bool DoSomething() => A.B.C.D switch
                                                {
                                                    42 => true,
                                                    _ => false,
                                                };
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_via_conditional_access_when_being_used_in_if_condition() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A?.B?.C?.D != null)
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public object D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_if_condition() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A.B.C.D)
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_ternary_operator_condition() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var i = A.B.C.D
                ? 42
                : 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_ternary_operator_true_case() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(int i)
    {
        var b = (i == 42)
                ? A.B.C.D
                : false;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_ternary_operator_false_case() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(int i)
    {
        var b = (i == 42)
                ? true
                : A.B.C.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_return_statement() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        return A.B.C.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_yield_return_statement() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public TestMeA A { get; }

    public IEnumerable<int> DoSomething()
    {
        yield return A.B.C.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_do_while_loop() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        do { }
        while (A.B.C.D);
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_while_loop() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        while (A.B.C.D)
        { }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public bool D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_assignment_on_left_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        A.B.C.D += 42;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_accessing_4_properties_in_a_row_when_being_used_in_assignment_on_right_side() => No_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var x = 42;
        x += A.B.C.D;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public int D { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_NUnits_Assert_Constraint_model_calls() => No_issue_is_reported_for("""

                                                                                                                using NUnit.Framework;

                                                                                                                public class TestMe
                                                                                                                {
                                                                                                                    public void DoSomething()
                                                                                                                    {
                                                                                                                        Assert.That("message", Does.Not.EndWith(".").And.Not.EndWith(" "));
                                                                                                                    }
                                                                                                                }

                                                                                                                """);

        [Test]
        public void No_issue_is_reported_for_Extension_method_calls() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public static class TestMe
{
    internal static readonly string[] Names;
    internal static readonly string[] MoreNames;
    internal static readonly string[] EventMoreNames;

    internal static readonly ISet<string> AllNames = Array.Empty<string>()
                                                          .Concat(Names)
                                                          .Concat(MoreNames)
                                                          .Concat(EventMoreNames)
                                                          .ToHashSet();
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_properties_in_a_row_when_invoking_a_method_inside_using_declaration_statement() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        using var x = A.B.C.D.DoSomething();
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public IDisposable DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_properties_in_a_row_when_invoking_a_method_inside_using_statement() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        using (A.B.C.D.DoSomething())
        {
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public IDisposable DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_properties_in_a_row_when_invoking_a_method() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        A.B.C.D.DoSomething();
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_properties_in_a_row_when_invoking_a_method_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D.Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int Invocation() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_array_elements_in_a_row_when_invoking_a_method_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].C[3].D[4].Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public TestMeC[] C { get; }
}

public class TestMeC
{
    public TestMeD[] D { get; }
}

public class TestMeD
{
    public int Invocation() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_4_indexers_in_a_row_when_invoking_a_method_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A[1][2][3][4].Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB this[int index] { get; }
}

public class TestMeB
{
    public TestMeC this[int index] { get; }
}

public class TestMeC
{
    public TestMeD this[int index] { get; }
}

public class TestMeD
{
    public TestMeE this[int index] { get; }
}

public class TestMeE
{
    public int Invocation() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_assigning_a_value() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var value = A.B.C.D.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_assigning_a_value_when_using_conditional_access() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var value = A?.B?.C?.D?.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D.E);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_array_elements_in_a_row_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].C[3].D[4].E[5]);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public TestMeC[] C { get; }
}

public class TestMeC
{
    public TestMeD[] D { get; }
}

public class TestMeD
{
    public int[] E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_array_elements_in_a_row_when_invoking_a_method_when_being_an_argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA[] A { get; }

    public void DoSomething()
    {
        DoSomething(A[1].B[2].C[3].D[4].E[5].Invocation());
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB[] B { get; }
}

public class TestMeB
{
    public TestMeC[] C { get; }
}

public class TestMeC
{
    public TestMeD[] D { get; }
}

public class TestMeD
{
    public TestMeE[] E { get; }
}

public class TestMeE
{
    public int Invocation() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_prefix_unary_expression() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(-A.B.C.D.E);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_postfix_unary_expression() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D.E++);
    }

    private void DoSomething(int value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_binary_expression_left_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var flag = false;

        DoSomething(A.B.C.D.E || flag);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_binary_expression_right_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var flag = false;

        DoSomething(A.B.C.D.E || flag);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_binary_expression_in_comparison_left_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D.E == 42);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_binary_expression_in_comparison_right_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(42 == A.B.C.D.E);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_an_argument_using_is_pattern_in_comparison() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        DoSomething(A.B.C.D.E is 42);
    }

    private void DoSomething(bool value)
    {
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_switch() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        switch (A.B.C.D.E)
        {
            case 42: break;
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_switch_when() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(object o)
    {
        switch (o)
        {
            case TestMeA a when a.B.C.D.E:
                break;
        }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_switch_expression() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public bool DoSomething() => A.B.C.D.E switch
                                                  {
                                                      42 => true,
                                                      _ => false,
                                                  };
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_via_conditional_access_when_being_used_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A?.B?.C?.D?.E != null)
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public object E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A.B.C.D.E)
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_ternary_operator_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var i = A.B.C.D.E
                ? 42
                : 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_ternary_operator_true_case() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(int i)
    {
        var b = (i == 42)
                ? A.B.C.D.E
                : false;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_ternary_operator_false_case() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething(int i)
    {
        var b = (i == 42)
                ? true
                : A.B.C.D.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_return_statement() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        return A.B.C.D.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_yield_return_statement() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public TestMeA A { get; }

    public IEnumerable<int> DoSomething()
    {
        yield return A.B.C.D.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_do_while_loop() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        do { }
        while (A.B.C.D.E);
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_while_loop() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        while (A.B.C.D.E)
        { }
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public bool E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_assignment_on_left_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        A.B.C.D.E += 42;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_assignment_on_right_side() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public void DoSomething()
    {
        var x = 42;
        x += A.B.C.D.E;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public int E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_pattern_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A.B.C.D.E is { })
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public object E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_conditional_access_and_pattern_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A?.B?.C?.D?.E is { })
            return 42;

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public object E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_recursive_pattern_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A.B.C.D.E is not MemberExpression
                             {
                                 Member: PropertyInfo
                                 {
                                     ReflectedType: { } reflectedType,
                                     Name: { } name
                                 }
                             })
        {
            return 42;
        }

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public object E { get; }
}
");

        [Test]
        public void An_issue_is_reported_for_accessing_5_properties_in_a_row_when_being_used_in_conditional_access_and_recursive_pattern_in_if_condition() => An_issue_is_reported_for(@"

public class TestMe
{
    public TestMeA A { get; }

    public int DoSomething()
    {
        if (A?.B?.C?.D?.E is not MemberExpression
                                 {
                                     Member: PropertyInfo
                                     {
                                         ReflectedType: { } reflectedType,
                                         Name: { } name
                                     }
                                 })
        {
            return 42;
        }

        return 0;
    }
}

public class TestMeA
{
    public TestMeB B { get; }
}

public class TestMeB
{
    public TestMeC C { get; }
}

public class TestMeC
{
    public TestMeD D { get; }
}

public class TestMeD
{
    public object E { get; }
}
");

        //// TODO RKN: Implement tests for method invocations, be aware of builder patterns

        protected override string GetDiagnosticId() => MiKo_3504_TrainWreckAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3504_TrainWreckAnalyzer();
    }
}