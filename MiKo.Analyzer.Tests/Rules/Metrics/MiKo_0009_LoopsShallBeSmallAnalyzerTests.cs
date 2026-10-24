using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0009_LoopsShallBeSmallAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_if_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_while_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_do_while_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
        }
        while (flag);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_for_statement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_foreach_statement() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        foreach (var x in Enumerable.Empty<object>())
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_statement_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
        {
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_do_while_statement_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
            ;
            ;
            ;
        }
        while (flag);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_statement_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++)
        {
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_statement_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        foreach (var x in Enumerable.Empty<object>())
        {
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_statement_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        foreach (var (key, value) in Enumerable.Empty<Tuple>())
        {
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_while_statement_when_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (flag)
        {
            ;
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_do_while_statement_when_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
            ;
            ;
            ;
            ;
        }
        while (flag);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_for_statement_when_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        for (var i = 0; i < 10; i++)
        {
            ;
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_statement_when_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        foreach (var x in Enumerable.Empty<object>())
        {
            ;
            ;
            ;
            ;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_statement_when_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        foreach (var (key, value) in Enumerable.Empty<Tuple>())
        {
            ;
            ;
            ;
            ;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_0009_LoopsShallBeSmallAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0009_LoopsShallBeSmallAnalyzer { MaxLinesOfCode = 3 };
    }
}