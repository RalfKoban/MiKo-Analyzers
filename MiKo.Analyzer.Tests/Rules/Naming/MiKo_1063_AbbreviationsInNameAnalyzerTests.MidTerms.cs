﻿using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public partial class MiKo_1063_AbbreviationsInNameAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_local_variable_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var my" + midterm + @"Variable = 42;
            return my" + midterm + @"Variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_out_variable_in_try_parse_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return int.TryParse(""42"", out var my" + midterm + @"Variable) ? my" + midterm + @"Variable : 0;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_foreach_variable_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int[] variables)
        {
            foreach (var my" + midterm + @"Variable in variables)
            {
                return my" + midterm + @"Variable;
            }
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_field_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int My" + midterm + @"Field = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_enum_member_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None = 0,
        Some_" + midterm + @"_Something = 1,
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int My" + midterm + @"Property { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler My" + midterm + @"Event { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_parameter_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int my" + midterm + @"Parameter) { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void My" + midterm + @"Method() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class My" + midterm + @"Class
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_midterm_([ValueSource(nameof(BadMidTerms))] string midterm) => An_issue_is_reported_for(@"
using System;

namespace My" + midterm + @"Namespace
{
    public class TestMe
    { }
}");

        [TestCase("Seq", "Sequential")]
        public void Code_gets_fixed_for_midterm_(string originalTerm, string fixedTerm)
        {
            const string Template = @"
using System;

public class Test###Me
{ }
";

            VerifyCSharpFix(Template.Replace("###", originalTerm), Template.Replace("###", fixedTerm));
        }
    }
}