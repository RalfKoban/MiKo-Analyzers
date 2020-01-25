using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3038_PropertyGetterInvokesLinqAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_Linq_extension_in_methods() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_property_getter_without_Linq_extension() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get
            {
                return new[] { ""a"" };
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_as_arrow_clause_without_Linq_extension() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get => new[] { ""a"" };
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_Linq_extension() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get
            {
                return Enumerable.ToList(new[] { ""a"" });
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_as_arrow_clause_with_Linq_extension() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get => Enumerable.ToList(new[] { ""a"" });
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_getter_with_yield() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get
            {
                foreach (var x in new[] { ""a"" })
                {
                    yield return x;
                }
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3038_PropertyGetterInvokesLinqAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3038_PropertyGetterInvokesLinqAnalyzer();
    }
}