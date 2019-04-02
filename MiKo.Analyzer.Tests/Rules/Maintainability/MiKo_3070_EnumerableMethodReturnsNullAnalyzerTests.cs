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
        public void An_issue_reported_for_Enumerable_method_returning_null() => An_issue_is_reported_for(@"
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
        public void An_issue_reported_for_Enumerable_method_body_returning_null() => An_issue_is_reported_for(@"
using System.Collections;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething() => null;
    }
}");

        [Test]
        public void An_issue_reported_for_Enumerable_method_returning_a_variable_that_is_null() => An_issue_is_reported_for(@"
using System.Collections;

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
        public void An_issue_reported_for_Enumerable_method_returning_a_variable_that_is_potentially_null() => An_issue_is_reported_for(@"
using System.Collections;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable DoSomething(bool flag)
        {
            IEnumerable variable;

            if (flag)
                variable = new List<int>()
            else
                variable = null;

            return variable;
        }
    }
}");

        // TODO: RKN what about Linq calls such as FirstOrDefault();

        protected override string GetDiagnosticId() => MiKo_3070_EnumerableMethodReturnsNullAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3070_EnumerableMethodReturnsNullAnalyzer();
    }
}