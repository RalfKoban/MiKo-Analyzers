using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3244_ReplaceLambdaWithMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_short_lambda() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () => 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lambda_with_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                          ;
                          ;
                          ;
                     };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lambda_with_maximum_allowed_lines_and_empty_lines() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                          ;

                          ;

                          ;
                     };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lambda_with_maximum_allowed_lines_and_braces() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                         {
                              ;
                         }
                         {
                              ;
                         }
                     };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_expression_tree_lambda_in_query_predicate() => No_issue_is_reported_for(@"
using System;
using System.Linq;
using System.Linq.Expressions;

public class TestMe
{
    public IQueryable<int> DoSomething(IQueryable<int> source)
    {
        return source.Where(_ =>
                                  _ > 0
                               && _ < 10
                               && _ != 5
                               && _ != 6
                               && _ != 7);
    }

    public Expression<Func<int, bool>> DoSomethingElse()
    {
        return _ =>
                    _ > 0
                 && _ < 10
                 && _ != 5
                 && _ != 6
                 && _ != 7;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_expression_tree_lambda_in_method_not_returning_queryable() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public List<int> DoSomething(IQueryable<int> source)
    {
        var query = source.Where(_ =>
                                       _ > 0
                                    && _ < 10
                                    && _ != 5
                                    && _ != 6
                                    && _ != 7);

        return query.ToList();
    }

    public void DoSomethingElse(IQueryable<int> source)
    {
        var query = source.Where(_ =>
                                       _ > 0
                                    && _ < 10
                                    && _ != 5
                                    && _ != 6
                                    && _ != 7);

        foreach (var item in query)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_nested_delegate_lambda_inside_expression_tree() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class Parent
{
    public List<int> Children { get; set; }
}

public class TestMe
{
    public void DoSomething(IQueryable<Parent> query)
    {
        var result = query.Where(p => p.Children.Any(c =>
                                                              c > 0
                                                           && c < 10
                                                           && c != 5
                                                           && c != 6
                                                           && c != 7)).ToList();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_nested_delegate_lambda_inside_assigned_expression_tree() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Parent
{
    public List<int> Children { get; set; }
}

public class TestMe
{
    public void DoSomething()
    {
        Expression<Func<Parent, bool>> predicate = p => p.Children.Any(c =>
                                                                              c > 0
                                                                           && c < 10
                                                                           && c != 5
                                                                           && c != 6
                                                                           && c != 7);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_nested_delegate_lambda_inside_returned_expression_tree() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class Parent
{
    public List<int> Children { get; set; }
}

public class TestMe
{
    public Expression<Func<Parent, bool>> DoSomething()
    {
        return p => p.Children.Any(c =>
                                         c > 0
                                      && c < 10
                                      && c != 5
                                      && c != 6
                                      && c != 7);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lambda_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = () =>
                     {
                          ;
                          ;
                          ;
                          ;
                     };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_long_non_expression_tree_lambda_in_method_returning_queryable() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public IQueryable<int> DoSomething(IQueryable<int> source, List<int> items)
    {
        var results = items.Where(_ =>
                                        _ > 0
                                     && _ < 10
                                     && _ != 5
                                     && _ != 6
                                     && _ != 7).ToList();

        return source;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3244_ReplaceLambdaWithMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3244_ReplaceLambdaWithMethodAnalyzer { MaxLinesOfCode = 3 };
    }
}