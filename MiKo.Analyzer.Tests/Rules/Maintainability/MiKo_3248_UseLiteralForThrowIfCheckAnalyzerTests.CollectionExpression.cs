using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        // only the checks without type constraints are used here, as collection expressions need an explicit target type
        private static readonly string[] EqualityChecks =
                                                          [
                                                              "ArgumentOutOfRangeException.ThrowIfEqual",
                                                              "ArgumentOutOfRangeException.ThrowIfNotEqual",
                                                          ];

        [Test]
        public void An_issue_is_reported_for_other_argument_with_collection_expression_with_simple_elements_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, int first, int second) => " + invocation + @"<List<int>>(items, [first, second]);
}
");

        [Test]
        public void An_issue_is_reported_for_value_argument_with_collection_expression_with_simple_elements_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, int first, int second) => " + invocation + @"<List<int>>([first, second], items);
}
");

        [Test]
        public void An_issue_is_reported_for_other_argument_with_empty_collection_expression_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items) => " + invocation + @"<List<int>>(items, []);
}
");

        [Test]
        public void An_issue_is_reported_for_other_argument_with_collection_expression_with_spread_element_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, List<int> others) => " + invocation + @"<List<int>>(items, [..others]);
}
");

        [Test]
        public void An_issue_is_reported_for_other_argument_with_parenthesized_collection_expression_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, int first, int second) => " + invocation + @"<List<int>>(items, ([first, second]));
}
");

        [Test]
        public void An_issue_is_reported_for_named_other_argument_with_collection_expression_with_simple_elements_([ValueSource(nameof(EqualityChecks))] string invocation) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, int first, int second) => " + invocation + @"<List<int>>(value: items, other: [first, second]);
}
");

        [Test]
        public void No_issue_is_reported_for_arguments_with_simple_values_and_explicit_type_([ValueSource(nameof(EqualityChecks))] string invocation) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(List<int> items, List<int> others) => " + invocation + @"<List<int>>(items, others);
}
");
    }
}
