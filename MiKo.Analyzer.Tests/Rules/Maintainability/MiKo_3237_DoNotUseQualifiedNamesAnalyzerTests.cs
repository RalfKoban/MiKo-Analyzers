using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3237_DoNotUseQualifiedNamesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => File.ReadAllText(""some path"");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_binary_expression_with_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (File.ReadAllText(""some path"") == ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_is_pattern_with_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (File.ReadAllText(""some path"") is ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_binary_expression() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string Name { get; set; }

        public void DoSomething(TestMe other)
        {
            if (other.Name == ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_prefix_unary_expression() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool IsValid { get; set; }

        public void DoSomething(TestMe other)
        {
            if (!other.IsValid)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_postfix_unary_expression() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Counter { get; set; }

        public void DoSomething(TestMe other)
        {
            other.Counter++;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_assignment_expression() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Counter { get; set; }

        public void DoSomething(TestMe other)
        {
            other.Counter += 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_is_pattern_with_literal() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string Name { get; set; }

        public void DoSomething(TestMe other)
        {
            if (other.Name is ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parenthesized_expression_with_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var text = (File.ReadAllText(""some path""));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething(bool flag) => flag ? File.ReadAllText(""some path"") : string.Empty;
    }
}
");

        [TestCase("System")]
        [TestCase("System.Collections")]
        [TestCase("System.Collections.Generic")]
        public void No_issue_is_reported_for_fully_qualified_namespace_only_in_nameof_(string ns) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => nameof(" + ns + @");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_namespace_with_type_in_nameof() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => nameof(System.IO.File);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => System.IO.File.ReadAllText(""some path"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_name_with_3_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething() => System.Collections.Generic.KeyValuePair.Create(""a"", ""b"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_binary_expression_with_fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe other)
        {
            if (System.IO.File.ReadAllText(""some path"") == ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_is_pattern_with_fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe other)
        {
            if (System.IO.File.ReadAllText(""some path"") is ""MyName"")
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parenthesized_expression_with__fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var text = (System.IO.File.ReadAllText(""some path""));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething(bool flag) => flag ? System.IO.File.ReadAllText(""some path"") : string.Empty;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3237_DoNotUseQualifiedNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3237_DoNotUseQualifiedNamesAnalyzer();
    }
}