using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5016_RemoveAllUsesContainsOfHashSetAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Testing
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_but_not_inside_a_lambda() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers)
        {
            if (integers.Contains(42))
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_but_not_inside_a_RemoveAll() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers)
        {
            DoSomethingCore(i => integers.Contains(i));
        }

        private void DoSomethingCore(Func<int, bool> callback)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_of_HashSet_inside_a_RemoveAll() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, HashSet<int> set)
        {
            integers.RemoveAll(i => set.Contains(i));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_of_HashSet_inside_a_RemoveAll_using_a_method_group() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, HashSet<int> set)
        {
            integers.RemoveAll(set.Contains);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_of_ISet_inside_a_RemoveAll() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, ISet<int> set)
        {
            integers.RemoveAll(i => set.Contains(i));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_invokes_Contains_of_ISet_inside_a_RemoveAll_using_a_method_group() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, ISet<int> set)
        {
            integers.RemoveAll(set.Contains);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_invokes_Contains_of_List_inside_a_RemoveAll() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, List<int> other)
        {
            integers.RemoveAll(i => other.Contains(i));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_invokes_Contains_of_List_inside_a_RemoveAll_using_a_method_group() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Testing
{
    public class TestMe
    {
        public void DoSomething(List<int> integers, List<int> other)
        {
            integers.RemoveAll(other.Contains);
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_5016_RemoveAllUsesContainsOfHashSetAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5016_RemoveAllUsesContainsOfHashSetAnalyzer();
    }
}