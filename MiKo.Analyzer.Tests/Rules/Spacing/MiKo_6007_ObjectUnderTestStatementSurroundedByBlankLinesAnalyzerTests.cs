using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6007_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_ObjectUnderTest_call_as_only_statement() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            GC.Collect();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_ObjectUnderTest_call_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    GC.Collect();

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectUnderTest_parameter_as_only_statement() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            objectUnderTest.DoSomething();
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectUnderTest_parameter_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest, int something)
        {
            switch (something)
            {
                case 0:
                    objectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectUnderTest_property_call_as_only_statement() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            ObjectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectUnderTest_property_call_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    ObjectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_objectUnderTest_field_call_as_only_statement() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            m_objectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_objectUnderTest_field_call_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    m_objectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectUnderTest_variable() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            objectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_parameter_preceded_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            GC.Collect();
            objectUnderTest.DoSomething();
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_parameter_preceded_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest, int something)
        {
            switch (something)
            {
                case 0:
                    GC.Collect();
                    objectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_parameter_followed_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            objectUnderTest.DoSomething();
            GC.Collect();
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_parameter_followed_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest, int something)
        {
            switch (something)
            {
                case 0:
                    objectUnderTest.DoSomething();
                    GC.Collect();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_preceded_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            GC.Collect();
            ObjectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_preceded_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    GC.Collect();
                    ObjectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_preceded_by_another_statement_and_is_awaited() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public async Task DoSomethingAsync()
        {
            GC.Collect();
            await ObjectUnderTest.DoSomethingAsync().ConfigureAwait(false);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_preceded_by_another_statement_and_is_not_awaited() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public Task DoSomethingAsync()
        {
            GC.Collect();
            ObjectUnderTest.DoSomethingAsync().ConfigureAwait(false);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_followed_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            ObjectUnderTest.DoSomething();
            GC.Collect();
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_followed_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    ObjectUnderTest.DoSomething();
                    GC.Collect();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_followed_by_another_statement_and_is_awaited() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public async Task DoSomethingAsync()
        {
            await ObjectUnderTest.DoSomethingAsync().ConfigureAwait(false);
            GC.Collect();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_property_call_followed_by_another_statement_and_is_not_awaited() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public async Task DoSomethingAsync()
        {
            ObjectUnderTest.DoSomethingAsync().ConfigureAwait(false);
            GC.Collect();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_field_call_preceded_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            GC.Collect();
            m_objectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_field_call_preceded_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    GC.Collect();
                    m_objectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_field_call_followed_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            m_objectUnderTest.DoSomething();
            GC.Collect();
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_field_call_followed_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    m_objectUnderTest.DoSomething();
                    GC.Collect();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_variable_preceded_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            GC.Collect();
            objectUnderTest.DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_variable_preceded_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    var objectUnderTest = new TestMe();

                    GC.Collect();
                    objectUnderTest.DoSomething();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectUnderTest_variable_followed_by_another_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            objectUnderTest.DoSomething();
            GC.Collect();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_objectUnderTest_variable_followed_by_another_statement_in_switch_section() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    var objectUnderTest = new TestMe();

                    objectUnderTest.DoSomething();
                    GC.Collect();

                    break;
            }
        }

        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_parameter_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            GC.Collect();
            objectUnderTest.DoSomething();
        }

        public void DoSomething()
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            GC.Collect();

            objectUnderTest.DoSomething();
        }

        public void DoSomething()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_parameter_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            objectUnderTest.DoSomething();
            GC.Collect();
        }

        public void DoSomething()
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(TestMe objectUnderTest)
        {
            objectUnderTest.DoSomething();

            GC.Collect();
        }

        public void DoSomething()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ObjectUnderTest_property_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            GC.Collect();
            ObjectUnderTest.DoSomething();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            GC.Collect();

            ObjectUnderTest.DoSomething();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ObjectUnderTest_property_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            ObjectUnderTest.DoSomething();
            GC.Collect();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe ObjectUnderTest { get; set; }

        public void DoSomething()
        {
            ObjectUnderTest.DoSomething();

            GC.Collect();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_field_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            GC.Collect();
            m_objectUnderTest.DoSomething();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            GC.Collect();

            m_objectUnderTest.DoSomething();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_field_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            m_objectUnderTest.DoSomething();
            GC.Collect();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private TestMe m_objectUnderTest;

        public void DoSomething()
        {
            m_objectUnderTest.DoSomething();

            GC.Collect();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_variable_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            GC.Collect();
            objectUnderTest.DoSomething();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            GC.Collect();

            objectUnderTest.DoSomething();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_objectUnderTest_variable_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            objectUnderTest.DoSomething();
            GC.Collect();
        }
    }
}
";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            objectUnderTest.DoSomething();

            GC.Collect();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6007_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6007_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6007_CodeFixProvider();
    }
}