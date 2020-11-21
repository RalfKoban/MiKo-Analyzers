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
        public void No_issue_is_reported_for_methods_with_same_name_but_no_recursive_yield() => No_issue_is_reported_for(@"
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

        public IEnumerable<int> DoSomething(IEnumerable<IEnumerable<int>> items)
        {
            foreach (var item in items)
            {
                foreach (var i in DoSomething(item))
                {
                    yield return i + 42;
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_same_name_but_no_recursive_yield_alternatively_sorted() => No_issue_is_reported_for(@"
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
                foreach (var i in DoSomething(item))
                {
                    yield return i + 42;
                }
            }
        }

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
        public void An_issue_is_reported_for_recursive_yield() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class Tree<T>
    {
        public T Value;
        public Tree<T> Left;
        public Tree<T> Right;
    }

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

        [Test]
        public void An_issue_is_reported_for_recursive_yield_as_extension() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class Tree<T>
    {
        public T Value;
        public Tree<T> Left;
        public Tree<T> Right;
    }

    public static class TestMe
    {
        public static IEnumerable<T> PreorderTraversal<T>(this Tree<T> root)
        {
            if (root == null)
            {
                yield break;
            }

            yield return root.Value;

            foreach (T item in root.Left.PreorderTraversal())
            {
                yield return item;
            }

            foreach (T item in root.Right.PreorderTraversal())
            {
                yield return item;
            }
        }
    }
}
");

        // TODO: RKN this test is not working as expected - test should simulate 'MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer' source code but does not
        [Test]
        public void No_issue_is_reported_for_non_recursive_yield_due_to_different_parameter_types() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public interface IBase
    {
    }

    public interface IInherited : IBase
    {
        IEnumerable<IBase> Children { get; }
    }

    public class TestMe
    {
        public IEnumerable<IBase> Call(IInherited i)
        {
            foreach (var item in i.Children)
            {
                var b = Call(item);

                if (b != null)
                {
                    yield return b;
                }
            }
        }

        private IBase Call(IBase i) => i;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_5012_RecursiveCallUsesYieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5012_RecursiveCallUsesYieldAnalyzer();
    }
}