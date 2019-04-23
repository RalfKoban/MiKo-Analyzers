using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    partial class MiKo_1063_AbbreviationsInNameAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_local_variable_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var " + prefix + @"Variable = 42;
            return " + prefix + @"Variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_field_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int " + prefix + @"Field = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_property_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int " + prefix + @"Property { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler " + prefix + @"Event { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_parameter_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int " + prefix + @"Parameter) { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void " + prefix + @"Method() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class " + prefix + @"Class
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace " + prefix + @"Namespace
{
    public class TestMe
    { }
}");
    }
}