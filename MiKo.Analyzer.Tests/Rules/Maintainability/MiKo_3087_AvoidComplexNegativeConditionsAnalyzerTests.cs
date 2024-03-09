using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3087_AvoidComplexNegativeConditionsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_simple_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_negative_condition() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (!flag1)
            return;

        if (!(flag2))
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_negative_condition_with_Is_pattern() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1 is false)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_Is_false_pattern() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1 is false)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_simple_Is_pattern_with_Declarations_or_null() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o1, object o2)
    {
        if (o1 is string)
            return;

        if (o1 is string s)
            return;

        if (o2 is null)
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_or_combined_Is_declaration_pattern_with_additional_call() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object parameter)
    {
        if (parameter is string s && char.TryParse(s, out value))
            return;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_condition_with_OR_combined_and_an_additional_call() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public bool IsSomething(TestMe parameter)
    {
        if (some.Name != ""something"" && IsSomething(some) is false)
            return true;
        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_complex_negative_condition() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (!(flag1 || flag2))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_negative_condition_with_Is_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (!(o is string))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_negative_condition_with_Is_declaration_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (!(o is string s))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Is_false_pattern_condition_with_parenthesis_around_Is_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if ((o is string) is false)
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_complex_Is_false_pattern_condition_without_parenthesis_around_Is_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is string is false)
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Is_false_pattern_condition_with_parenthesis_around_declaration_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if ((o is string s) is false)
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_complex_Is_false_pattern_condition_without_parenthesis_around_declaration_pattern() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is string s is false)
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_OR_combined_Is_declaration_pattern_with_additional_call() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object parameter)
    {
        if (!(parameter is string) || !char.TryParse((string) parameter, out value))
            return;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_negative_condition_with_OR_combined_and_an_additional_call() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public bool IsSomething(TestMe parameter)
    {
        if (!(some.Name == ""something"" || IsSomething(some)))
            return true;
        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_complex_negative_additional_call() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestMe
{
    private TestMe _verifier;

    public string SomeInfo { get; set; }

    public TestMe Whatever { get; set; }
    
    public TestMe Data { get; set; }

    public bool CanDoSomething(TestMe obj, IEnumerable<TestMe> items)
    {
        if (!_verifier.CanDoSomething(GetSomeData(obj.SomeInfo), items.Select(_ => _.Whatever.Data).ToList()))
            return true;
        return false;
    }

    public TestMe GetSomeData(string info) => null;
}
");

        // TODO RKN
        // test for variable assignments
        // test for arguments
        protected override string GetDiagnosticId() => MiKo_3087_AvoidComplexNegativeConditionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3087_AvoidComplexNegativeConditionsAnalyzer();
    }
}