using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public partial class MiKo_1063_AbbreviationsInNameAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_parameters_of_overridden_method_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
#pragma warning disable MiKo_1063

    public abstract class TestMeBase
    {
        public virtual void DoSomething(int parameter" + postfix + @")
        { }
    }

#pragma warning restore MiKo_1063

    public class TestMe : TestMeBase
    {
        public override void DoSomething(int parameter" + postfix + @")
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_variable_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var variable" + postfix + @" = 42;
            return variable" + postfix + @";
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_out_variable_in_try_parse_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            return int.TryParse(""42"", out var variable" + postfix + @") ? variable" + postfix + @" : 0;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_foreach_variable_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int[] variables)
        {
            foreach (var variable" + postfix + @" in variables)
            {
                return variable" + postfix + @";
            }
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_field_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int field" + postfix + @" = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_enum_member_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None = 0,
        Something_" + postfix + @" = 1,
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Property" + postfix + @" { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler My" + postfix + @" { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_virtual_method_on_parameter_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public virtual void DoSomething(int parameter" + postfix + @") { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_overridden_method_on_changed_parameter_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public abstract class TestMeBase
    {
        public virtual void DoSomething(int someParameter) { }
    }

    public class TestMe : TestMeBase
    {
        public override void DoSomething(int parameter" + postfix + @") { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int parameter" + postfix + @") { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void Method" + postfix + @"() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class Class" + postfix + @"
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_postfix_([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla" + postfix + @"
{
    public class TestMe
    { }
}");

        [TestCase("Seq", "Sequence")]
        public void Code_gets_fixed_for_postfix_(string originalTerm, string fixedTerm)
        {
            const string Template = @"
using System;

public class TestMe###
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalTerm), Template.Replace("###", fixedTerm));
        }
    }
}