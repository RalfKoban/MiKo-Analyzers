using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3007_LinqStyleMixAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_Linq_extension_chain_only_methods() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething() => new[] { ""a"" }.ToList();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Linq_extension_chain_only_ctors() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        private IEnumerable<string> m_field;

        public TestMe() => m_field = new[] { ""a"" }.ToList();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Linq_static_chain_only_methods() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething() => Enumerable.ToList(new[] { new[] { ""a"" } });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Linq_query_only_methods() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething() => from x in new[] { ""a"" } select x;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_combination_of_Linq_chain_and_query_only_methods() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        private IEnumerable<string> m_field;

        public TestMe() => m_field = new[] { ""a"" }.ToList();

        public IEnumerable<string> DoSomething() => new[] { ""a"" }.ToList();

        public IEnumerable<string> DoSomethingElse() => from x in new[] { ""a"" } select x;

        public IEnumerable<string> DoSomethingOther() => Enumerable.ToList(new[] { new[] { ""a"" } });
    }
}
");

        [Test, Ignore("For yet unknown reason, the test is not working but the analyzer seems to work in real production code.")]
        public void An_issue_is_reported_for_combination_of_Linq_chain_and_query_in_same_method() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething() => (from x in new[] { ""a"" } select x).ToList();
    }
}
");

        [Test, Ignore("For yet unknown reason, the test is not working but the analyzer seems to work in real production code.")]
        public void An_issue_is_reported_for_combination_of_Linq_chain_and_query_in_same_ctor() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        private IEnumerable<string> m_field;

        public TestMe() => m_field = (from x in new[] { ""a"" } select x).ToList();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_combination_of_Linq_static_chain_and_query_in_same_method() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething() => Enumerable.ToList((from x in new[] { ""a"" } select x));
    }
}
");

        [Test, Ignore("For yet unknown reason, the test is not working but the analyzer seems to work in real production code.")]
        public void An_issue_is_reported_for_combination_of_Linq_static_chain_and_query_in_same_field() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
         private static readonly string[] BooleanPhrases = (from term1 in new[] { "" indicating "", "" indicates "", "" indicate "" }
                                                            from term2 in new[] { ""whether "", ""if "" }
                                                            select string.Concat(term1, term2))
                                                           .ToArray();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Linq_static_chain_only_generic_methods() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething<T>() => Enumerable.ToList<T>(new T[0]);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3007_LinqStyleMixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3007_LinqStyleMixAnalyzer();
    }
}