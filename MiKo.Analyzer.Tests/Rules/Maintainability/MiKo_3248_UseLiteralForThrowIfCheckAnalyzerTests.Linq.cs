using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_argument_and_LINQ_query_syntax_as_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(int argument, IEnumerable<TestMe> items) => " + invocation + @"(argument, (from item in items select item.Number).First(), nameof(items));
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_LINQ_query_syntax_as_argument_and_other_and_paramName_([ValueSource(nameof(ValueChecksWithOther))] string invocation) => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(IEnumerable<TestMe> items, int other) => " + invocation + @"((from item in items select item.Number).First(), other, nameof(items));
}
");

        [Test]
        public void An_issue_is_reported_for_Integer_invocation_with_LINQ_query_syntax_as_argument_and_paramName_([ValueSource(nameof(ValueChecksWithoutOther))] string invocation) => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public int Number { get; set; }

    public void DoSomething(IEnumerable<TestMe> items) => " + invocation + @"((from item in items select item.Number).First(), nameof(items));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_LINQ_query_syntax_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(IEnumerable<TestMe> items) => ArgumentNullException.ThrowIfNull(from item in items select item.Name);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_LINQ_query_syntax_argument_and_paramName() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(IEnumerable<TestMe> items) => ArgumentNullException.ThrowIfNull(from item in items select item.Name, nameof(items));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_LINQ_query_syntax_with_where_clause_argument_and_paramName() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(IEnumerable<TestMe> items) => ArgumentNullException.ThrowIfNull(from item in items where item.Name != null select item.Name, nameof(items));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_LINQ_query_syntax_argument_and_paramName() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;
using System;

public class TestMe
{
    public string Name { get; set; }

    public void DoSomething(IEnumerable<TestMe> items) => ArgumentNullException.ThrowIfNull((from item in items select item.Name), nameof(items));
}
");
    }
}