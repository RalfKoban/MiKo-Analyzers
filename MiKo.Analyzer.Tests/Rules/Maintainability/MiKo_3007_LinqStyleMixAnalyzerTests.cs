using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

        [Test]
        public void No_issue_is_reported_for_combination_of_Linq_static_chain_and_query_in_same_field_if_it_is_the_only_Linq_call() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_combination_of_Linq_static_chain_and_query_if_both_are_independent_calls_but_Linq_call_is_ToList() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<char> DoSomething()
        {
            var foo = from letter in ""ABCD"" select char.ToLower(letter);
            var bar = foo.ToList();
            return bar;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_combination_of_Linq_chain_and_query_in_same_method() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => (from x in new[] { ""a"" } select x).FirstOrDefault();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_combination_of_Linq_chain_and_query_in_same_ctor() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        private string m_field;

        public TestMe() => m_field = (from x in new[] { ""a"" } select x).FirstOrDefault();
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
        public string DoSomething() => Enumerable.FirstOrDefault((from x in new[] { ""a"" } select x));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_combination_of_Linq_static_chain_and_query_in_same_field() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
         private static readonly string BooleanPhrase = (from term1 in new[] { "" indicating "", "" indicates "", "" indicate "" }
                                                         from term2 in new[] { ""whether "", ""if "" }
                                                         select string.Concat(term1, term2))
                                                        .FirstOrDefault();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_query_contains_static_Linq_calls() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
         private static readonly string[] Phrases = (from a in new[] { ""Bla"", ""blubb"" }
                                                     from b in new[] { ""Bla"", ""blubb"" }
                                                     let x = a.FirstOrDefault()
                                                     let y = b.FirstOrDefault()
                                                     where x != y
                                                     select string.Concat(x.ToString(), y.ToString())
                                                    .ToArray();
    }
}
");

        [Test, Ignore("Does not detect for yet unknown reason")]
        public void An_issue_is_reported_for_combination_of_Linq_static_chain_and_query_if_both_are_independent_calls() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> DoSomething()
        {
            var temp = (from x in new[] { ""a"", ""b"" } select x).ToList();
            var result = temp.Select(_ => _);
            return result;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3007_LinqStyleMixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3007_LinqStyleMixAnalyzer();
    }
}