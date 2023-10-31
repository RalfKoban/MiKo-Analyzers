using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3039_PropertyGetterInvokesLinqAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_property_getter_with_Empty_Linq_extension() => No_issue_is_reported_for(@"
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
                return Enumerable.Empty<string>();
            }
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
        public void An_issue_is_reported_for_property_getter_with_cast_as_Linq_extension() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<short> Bla
        {
            get
            {
                return new[] { 1, 2, 3 }.Cast<short>()));
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
        public void An_issue_is_reported_for_property_getter_as_arrow_clause_with_Linq_extension_and_elvis_operator() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        private IEnumerable<string> _bla;

        public IEnumerable<string> Bla
        {
            get => _bla?.Select(_ => _).ToList();
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

        [Test]
        public void No_issue_is_reported_for_property_getter_as_arrow_clause_with_Enumerable_Empty() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<string> Bla
        {
            get => Enumerable.Empty<string>();
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3039_PropertyGetterInvokesLinqAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3039_PropertyGetterInvokesLinqAnalyzer();
    }
}