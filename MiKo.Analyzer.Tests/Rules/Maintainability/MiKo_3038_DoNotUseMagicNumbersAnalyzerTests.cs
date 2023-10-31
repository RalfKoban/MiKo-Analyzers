using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3038_DoNotUseMagicNumbersAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CollectionTypes =
                                                           {
                                                               "ArrayList",
                                                               "HashSet<string>",
                                                               "List<string>",
                                                               "Dictionary<string, string>",
                                                               "Queue<string>",
                                                               "Stack<string>",
                                                           };

        [Test]
        public void No_issue_is_reported_for_const_field_([Values(-42, -1, 0, 1, 42)] int value) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_const_variable_([Values(-42, -1, 0, 1, 42)] int value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            const int i = " + value + @";
        }
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
        Anything = 42 + 0815,
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
        public void No_issue_is_reported_for_capacity_argument_of_object_initializer_of_([ValueSource(nameof(CollectionTypes))] string type) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public " + type + @" DoSomething()
        {
            var result = new " + type + @"(2) { };

            return result;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_capacity_argument_of_ctor_of_([ValueSource(nameof(CollectionTypes))] string type) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public " + type + @" DoSomething()
        {
            var result = new " + type + @"(2);

            return result;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_initializer() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Property { get; set; } = 1234;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_getter_return_value() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get { return 42; }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_expression_body_getter_return_value() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get => 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_property_return_value() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int Something => 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_assigned_to_([Values("0", "1", "2", "42")] string value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int i = " + value + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assigned_to_([Values("0", "0L", "0u", "0.0", "0d", "0.0d", "0f", "0.0f", "1", "1L", "1u", "1.0", "1d", "1.0d", "1f", "1.0f", "-1", "-1L", "-1d", "-1f", "-1.0", "42")] string value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = " + value + @";
        }
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
        public void No_issue_is_reported_for_division_by_([Values("2", "2l", "2u", "2.0", "2.0d", "2.0f")] string number) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = 1 / " + number + @";
            i /= " + number + @";
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_assignment_([Values("42", "-42", "(-42)")] string number) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int i;
            i = " + number + @";
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_case() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_range_expression() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething(string value)
        {
            return char.ToUpper(value[0]) + value[1..];
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_array_access_at_([Values(0, 1)] int value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int[] values)
        {
            int value = values[" + value + @"];
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int GetHashCode()
        {
            return 297 * 511 ^ 123;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Version_ctor() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public Version Create()
        {
            return new Version(1, 2, 3, 4);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_DateTime_ctor_with_hour_minute_seconds() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public DateTime Create()
        {
            return new DateTime(2021, 1, 1);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_TimeSpan_ctor_with_hour_minute_seconds() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan Create()
        {
            return new TimeSpan(2021, 1, 1);
        }
    }
}
");

        [TestCase("FromSeconds(42)")]
        [TestCase("FromMinutes(42)")]
        [TestCase("FromHours(42)")]
        [TestCase("FromDays(42)")]
        public void No_issue_is_reported_for_TimeSpan_From_method_(string value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan Create()
        {
            return TimeSpan." + value + @";
        }
    }
}
");

        [TestCase("FromRgb(127, 42, 17)")]
        [TestCase("FromArgb(127, 42, 17, 815)")]
        public void No_issue_is_reported_for_Color_From_method_(string value) => No_issue_is_reported_for(@"
using System;
using System.Windows.Media;

namespace Bla
{
    public class TestMe
    {
        public Color Create()
        {
            return Color." + value + @";
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Progress_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            ReportProgress(100);
        }

        public void ReportProgress(int progress)
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Progress_ctor() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyProgress
    {
        public MyProgress(int progress) => Progress = progress;

        public int Progress { get; }
    }

    public class TestMe
    {
        public void DoSomethingA()
        {
            var progress = new MyProgress(100);

            ReportProgress(progress);
        }

        public void DoSomethingB()
        {
            ReportProgress(new MyProgress(100));
        }

        public void ReportProgress(MyProgress progress)
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_screen_resolution_number_([Values(320, 200, 640, 480, 800, 600, 1024, 768, 1920, 1080, 1280, 1440, 1600, 1200)] int number) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var i = " + number + @";
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_pragma_directive() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
#pragma warning disable 618
            var i = 0;
#pragma warning restore 618
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
        public void No_issue_is_reported_for_field_([Values(-42, 42)] int value) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int i = " + value + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_array_access_at_([Values(2, 3, 4, 5, 10, 20, 42)] int value) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int[] values)
        {
            int value = values[" + value + @"];
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DateTime_ctor_with_ticks() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public DateTime Create()
        {
            return new DateTime(2021);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DateTime_ctor_with_ticks_and_DateTimeKind() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public DateTime Create()
        {
            return new DateTime(2021, DateTimeKind.Utc);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_array_rank_specifier() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            byte[] buffer = new byte[4];
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3038_DoNotUseMagicNumbersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3038_DoNotUseMagicNumbersAnalyzer();
    }
}