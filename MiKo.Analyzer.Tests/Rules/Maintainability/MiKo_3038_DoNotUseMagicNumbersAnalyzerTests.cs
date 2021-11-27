using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3038_DoNotUseMagicNumbersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_const_([Values("-42", "-1", "0", "1", "42")] string value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private const int i = " + value + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_enum_member() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None = 0,
        Something = 1,
        Anything = 42,
    }
}
");

        [Test]
        public void No_issue_is_reported_for_attribute_argument() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    [SomeAttribute(42)]
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_assigned_to_zero() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_call_with_minus_1() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() => DoSomething(-1);

        private void DoSomething(int i) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_initial_assignment_in_for_loop() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < int.MaxValue; i++)
            { }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_subtract_of_1_in_initial_assignment_for_loop() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = int.MaxValue - 1; i > 0; i--)
            { }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_increment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;
            i = i + 1;
            i += 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_decrement() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 0;
            i = i - 1;
            i -= 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_fixture_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [" + fixture + @"]
    public class TestMe
    {
        public void DoSomething() => DoSomething(0815 - 42);

        private void DoSomething(int i)
        {
            i += 4;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_type_in_test_fixture_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [" + fixture + @"]
    public class TestMe
    {
        private sealed class Helper
        {
            public int ReturnSomething() => 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            var i = 0815 - 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_([Values("-42", "42")] string value) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int i = " + value + @";
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3038_DoNotUseMagicNumbersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3038_DoNotUseMagicNumbersAnalyzer();
    }
}