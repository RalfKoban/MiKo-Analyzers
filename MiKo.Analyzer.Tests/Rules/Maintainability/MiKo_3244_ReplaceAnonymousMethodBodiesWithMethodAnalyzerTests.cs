using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3244_ReplaceAnonymousMethodBodiesWithMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_short_anonymous_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int> x = delegate { return 42; };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_anonymous_method_with_maximum_allowed_lines() => No_issue_is_reported_for(@"using System;

public class TestMe
{
    public void DoSomething()
    {
        Action x = delegate
                     {
                          ;
                          ;
                          ;
                     };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_anonymous_method_with_maximum_allowed_lines_and_empty_lines() => No_issue_is_reported_for(@"using System;

public class TestMe
{
    public void DoSomething()
    {
        Action x = delegate
                     {
                          ;

                          ;

                          ;
                     };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_anonymous_method_with_maximum_allowed_lines_and_braces() => No_issue_is_reported_for(@"using System;

public class TestMe
{
    public void DoSomething()
    {
        Action x = delegate
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
        public void No_issue_is_reported_for_long_anonymous_method_returning_anonymous_object_creation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Func<int, object> x = delegate (int i)
                                  {
                                      return new
                                                 {
                                                     A = i,
                                                     B = i,
                                                     C = i,
                                                     D = i,
                                                     E = i,
                                                     F = i,
                                                 };
                                  };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_anonymous_method_returning_object_creation() => No_issue_is_reported_for(@"
using System;

public class Item
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
    public int E { get; set; }
    public int F { get; set; }
}

public class TestMe
{
    public void DoSomething()
    {
        Func<int, Item> x = delegate (int i)
                                {
                                    return new Item
                                               {
                                                   A = i,
                                                   B = i,
                                                   C = i,
                                                   D = i,
                                                   E = i,
                                                   F = i,
                                               };
                                };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_anonymous_method_with_block_returning_object_creation() => No_issue_is_reported_for(@"
using System;

public class Item
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
    public int E { get; set; }
    public int F { get; set; }
}

public class TestMe
{
    public void DoSomething()
    {
        Func<int, Item> x = delegate (int i)
                                {
                                    return new Item
                                               {
                                                   A = i,
                                                   B = i,
                                                   C = i,
                                                   D = i,
                                                   E = i,
                                                   F = i,
                                               };
                                };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_long_anonymous_method_returning_target_typed_object_creation() => No_issue_is_reported_for(@"
using System;

public class Item
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
    public int E { get; set; }
    public int F { get; set; }
}

public class TestMe
{
    public void DoSomething()
    {
        Func<int, Item> x = delegate (int i)
                                {
                                    return new()
                                               {
                                                   A = i,
                                                   B = i,
                                                   C = i,
                                                   D = i,
                                                   E = i,
                                                   F = i,
                                               };
                                };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_with_Assert_Multiple_anonymous_method_above_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Assert.Multiple(delegate
                             {
                                  ;
                                  ;
                                  ;
                                  ;
                             });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_with_Assert_MultipleAsync_anonymous_method_above_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething() => Assert.MultipleAsync(async delegate
                                                           {
                                                                ;
                                                                ;
                                                                ;
                                                                ;
                                                           });
}
");

        [Test]
        public void No_issue_is_reported_for_test_with_Assert_MultipleAsync_anonymous_method_returning_non_object_creation_above_maximum_allowed_lines() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public Task DoSomething() => Assert.MultipleAsync(delegate
                                                          {
                                                              return Task.WhenAll(
                                                                              Task.CompletedTask,
                                                                              Task.CompletedTask,
                                                                              Task.CompletedTask,
                                                                              Task.CompletedTask);
                                                          });
}
");

        [Test]
        public void An_issue_is_reported_for_long_anonymous_method_inside_Assert_Multiple() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Assert.Multiple(delegate
                             {
                                 Action work = delegate
                                                    {
                                                        ;
                                                        ;
                                                        ;
                                                        ;
                                                    };

                                 Assert.DoesNotThrow(delegate { work(); });
                             });
    }
}
");

        [Test]
        public void An_issue_is_reported_for_anonymous_method_above_maximum_allowed_lines() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        Action x = delegate
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
        public void An_issue_is_reported_for_long_anonymous_method_in_method_returning_queryable() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    public IQueryable<int> DoSomething(IQueryable<int> source, List<int> items)
    {
        var results = items.Where(delegate (int i)
                                  {
                                      return i > 0
                                          && i < 10
                                          && i != 5
                                          && i != 6
                                          && i != 7;
                                  }).ToList();

        return source;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3244_ReplaceAnonymousMethodBodiesWithMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3244_ReplaceAnonymousMethodBodiesWithMethodAnalyzer { MaxLinesOfCode = 3 };
    }
}