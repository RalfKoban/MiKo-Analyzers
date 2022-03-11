using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3107_OnlyMocksUseConditionMatchersAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames =
            {
                "Is",
                "IsAny",
                "IsIn",
                "IsNotIn",
                "IsInRange",
                "IsNotNull",
                "IsRegex",
            };

        [Test]
        public void No_issue_is_reported_for_correct_object_creation_on_field() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe(string text) { }

        public void DoSomething(string text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe(string.Empty);

        public void PrepareTest()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_object_creation() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe(string text) { }

        public void DoSomething(string text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new TestMe(string.Empty);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_mock_method_invocation_([ValueSource(nameof(MethodNames))] string method) => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(string text);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Setup(_ => _.DoSomething(It." + method + @"<string>()).Returns(true);
            ObjectUnderTest.Verify(_ => _.DoSomething(It." + method + @"<string>());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_chained_mock_method_invocation_([ValueSource(nameof(MethodNames))] string method) => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        bool DoSomething(string text);
    }

    public interface ITestMe2
    {
        ITestMe TestMe { get; }
    }

    public class TestMeTests
    {
        private Mock<ITestMe2> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe2>();

            ObjectUnderTest.Setup(_ => _.TestMe.DoSomething(It." + method + @"<string>()).Returns(true);
            ObjectUnderTest.Verify(_ => _.TestMe.DoSomething(It." + method + @"<string>());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Mock_Of_method_invocation_([ValueSource(nameof(MethodNames))] string method) => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        bool DoSomething(string text);
    }

    public interface ITestMe2
    {
        ITestMe TestMe { get; }
    }

    public class TestMeTests
    {
        private Mock<ITestMe2> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            Mock.Of<ITestMe>(_ => _.DoSomething(It." + method + @"<string>()) == true)
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_mock_method_invocation_([ValueSource(nameof(MethodNames))] string method) => An_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe(string text) { }

        public void DoSomething(string text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new TestMe(string.Empty);

            ObjectUnderTest.DoSomething(It." + method + @"<string>());
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_problem_on_object_creation_([ValueSource(nameof(MethodNames))] string method) => An_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe(string text) { }

        public void DoSomething(string text) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new TestMe(It." + method + @"<string>());
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_with_correct_object_creation() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe() { }

        public void DoSomething(string text) { }

        public int Value { get; set; }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest() => ObjectUnderTest = new TestMe
                                                           {
                                                               Value = 42,
                                                           };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_body_with_wrong_object_creation_([ValueSource(nameof(MethodNames))] string method) => An_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe() { }

        public void DoSomething(string text) { }

        public int Value { get; set; }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest() => ObjectUnderTest = new TestMe
                                                           {
                                                               Value = It." + method + @"<int>(),
                                                           };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_correct_object_initialization() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe() { }

        public void DoSomething(string text) { }

        public int Value { get; set; }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest = new TestMe
                                             {
                                                 Value = 42,
                                             };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_correct_object_initialization() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public TestMe() { }

        public void DoSomething(string text) { }

        public int Value { get; set; }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; } = new TestMe
                                                      {
                                                          Value = 42,
                                                      };
    }
}
");

        [TestCase("It.IsAny<bool>()", "false")]
        [TestCase("It.IsAny<byte>()", "0")]
        [TestCase("It.IsAny<char>()", @"'\0'")]
        [TestCase("It.IsAny<decimal>()", "0M")]
        [TestCase("It.IsAny<double>()", "double.NaN")]
        [TestCase("It.IsAny<float>()", "float.NaN")]
        [TestCase("It.IsAny<int>()", "0")]
        [TestCase("It.IsAny<uint>()", "0")]
        [TestCase("It.IsAny<object>()", "null")]
        [TestCase("It.IsAny<string>()", "null")]
        [TestCase("It.IsAny<TestMe>()", "null")]
        public void Code_gets_fixed_for_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new TestMe();

            ObjectUnderTest.DoSomething(###);
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_multiline()
        {
            const string Template = @"
using System;

using Moq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o1, object o2) { }
    }

    public class TestMeTests
    {
        private TestMe ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new TestMe();

            ObjectUnderTest.DoSomething(
                                    ###,
                                    new object());
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", "It.IsAny<object>()"), Template.Replace("###", "null"));
        }

        protected override string GetDiagnosticId() => MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3107_CodeFixProvider();
    }
}