using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3070_EnumerableMethodReturnsNullAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() {}
}");

        [Test]
        public void No_issue_reported_for_non_Enumerable_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething()
    {
        return null;
    }
}");

        [Test]
        public void No_issue_reported_for_non_Enumerable_method_body() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething() => null;
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_returning_a_valid_List() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething()
        {
            return new List<int>();
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_with_null_check() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(IEnumerable o)
        {
            if (o == null)
                return Enumerable.Empty<int>();
            return new List<int>();
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_with_pattern_null_check() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(IEnumerable o)
        {
            if (o is null)
                return Enumerable.Empty<int>();
            return new List<int>();
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_with_null_check_and_conditional() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(IEnumerable o)
        {
            return o == null ? Enumerable.Empty<int>() : new List<int>();
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_with_pattern_null_check_and_conditional() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(IEnumerable o)
        {
            return o is null ? Enumerable.Empty<int>() : new List<int>();
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_body_returning_a_valid_List() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething() => new List<int>();
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_yield_returning_null() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething()
        {
            yield return null;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_null() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething()
        {
            return null;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_body_returning_null() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething() => null;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_variable_that_is_null() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething()
        {
            IEnumerable variable = null;
            return variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_variable_that_is_potentially_null_because_of_if_block() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            IEnumerable variable;

            if (flag)
                variable = new List<int>();
            else
                variable = null;

            return variable;
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_returning_a_variable_that_is_not_null_although_it_has_if_block() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            IEnumerable variable;

            if (flag)
                variable = new List<int>();
            else
                variable = new int[0];

            return variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_variable_that_is_potentially_null_because_of_ternary_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            IEnumerable variable = flag
                                       ? new List<int>()
                                       : null;
            return variable;
        }
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_returning_a_variable_that_is_not_null_but_has_ternary_operator() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            IEnumerable variable = flag
                                       ? new List<int>()
                                       : new int[0];
            return variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_result_that_is_potentially_null_because_of_ternary_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            return flag
                   ? new List<int>()
                   : null;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_body_returning_a_result_that_is_potentially_null_because_of_ternary_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag) => flag ? new List<int>() : null;
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_body_returning_a_result_that_is_potentially_null_because_of_quite_complex_ternary_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(int i) => i > 0 ? i > 42 ? null : new List<int>() : i < -42 ? null : new List<int>();
    }
}");

        [Test]
        public void No_issue_reported_for_Enumerable_method_returning_a_variable_that_is_null_but_then_reassigned() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething()
        {
            IEnumerable variable = null;
            variable = new List<int>();

            return variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_result_that_is_potentially_null_because_of_switch_conditions() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(int i)
        {
            switch (i)
            {
                case 0815:
                case 42:
                    return new List<int>();

                default:
                    return null;
            }
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_Enumerable_method_returning_a_variable_based_on_Coalescence_operator() => No_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
    {
        public class TestMe
        {
            public IEnumerable<int> DoSomething()
            {
                IEnumerable<int> variable = null;
                return variable ?? Enumerable.Empty<int>();
            }
        }
    }");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_a_variable_that_is_null_and_used_on_right_side_of_Coalescence_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
    {
        public class TestMe
        {
            public IEnumerable<int> DoSomething()
            {
                IEnumerable<int> variable1 = null;
                IEnumerable<int> variable2 = null;
                return variable1 ?? variable2;
            }
        }
    }");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_returning_an_optional_parameter_that_is_null_and_used_on_right_side_of_Coalescence_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
    {
        public class TestMe
        {
            public IEnumerable<int> DoSomething(IEnumerable<int> p1, IEnumerable<int> p2 = null)
            {
                return p1 ?? p2;
            }
        }
    }");

        [Test]
        public void An_issue_is_reported_for_Enumerable_method_body_returning_an_optional_parameter_that_is_null_and_used_on_right_side_of_Coalescence_operator() => An_issue_is_reported_for(@"
using System.Collections;
using System.Collections.Generic;

namespace Bla
    {
        public class TestMe
        {
            public IEnumerable<int> DoSomething(IEnumerable<int> p1, IEnumerable<int> p2 = null) => p1 ?? p2;
        }
    }");

        // TODO: RKN what about Linq calls such as FirstOrDefault();

        protected override string GetDiagnosticId() => MiKo_3070_EnumerableMethodReturnsNullAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3070_EnumerableMethodReturnsNullAnalyzer();
    }
}