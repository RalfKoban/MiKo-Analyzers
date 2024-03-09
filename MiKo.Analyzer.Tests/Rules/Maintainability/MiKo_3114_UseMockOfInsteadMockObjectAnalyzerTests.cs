using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3114_UseMockOfInsteadMockObjectAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correct_usage() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(Mock.Of<IEnumerable<string>>());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrect_usage() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(new Mock<IEnumerable<string>>().Object);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrect_usage_with_parenthesis() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething((new Mock<IEnumerable<string>>()).Object);
        }
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(new Mock<IEnumerable<string>>().Object);
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(Mock.Of<IEnumerable<string>>());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> texts, IEnumerable<string> words) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(
                                    new Mock<IEnumerable<string>>().Object,
                                    new Mock<IEnumerable<string>>().Object);
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> texts, IEnumerable<string> words) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(
                                    Mock.Of<IEnumerable<string>>(),
                                    Mock.Of<IEnumerable<string>>());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parenthesis()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething((new Mock<IEnumerable<string>>()).Object);
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<string> text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe();

        public void Test()
        {
            ObjectUnderTest.DoSomething(Mock.Of<IEnumerable<string>>());
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3114_UseMockOfInsteadMockObjectAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3114_UseMockOfInsteadMockObjectAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3114_CodeFixProvider();
    }
}