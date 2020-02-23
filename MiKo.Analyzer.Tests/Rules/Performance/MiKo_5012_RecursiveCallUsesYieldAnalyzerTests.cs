using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5012_RecursiveCallUsesYieldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_methods() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_simple_return_types() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_Linq_returns() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(IEnumerable<int> items)
        {
            return items.Select(_ => _ + 42);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_normal_yield_returns() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething()
        {
            yield return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_normal_yield_in_foreach_returns() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                yield return item + 42;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_normal_yield_in_nested_foreach_returns() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(IEnumerable<IEnumerable<int>> items)
        {
            foreach (var item in items)
            {
                foreach (var i in item)
                {
                    yield return i + 42;
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_recursive_yield() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static IEnumerable<T> PreorderTraversal<T>(Tree<T> root)
        {
            if (root == null)
            {
                yield break;
            }

            yield return root.Value;

            foreach (T item in PreorderTraversal(root.Left))
            {
                yield return item;
            }

            foreach (T item in PreorderTraversal(root.Right))
            {
                yield return item;
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_5012_RecursiveCallUsesYieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5012_RecursiveCallUsesYieldAnalyzer();
    }
}