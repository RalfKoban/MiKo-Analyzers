using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_recursive_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_mock_method_invocation_([Values("IsAny", "IsIn", "IsNotIn", "IsInRange", "IsNotNull", "IsRegex")] string method) => No_issue_is_reported_for(@"
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

            ObjectUnderTest.Setup(_ => _.DoSomething(It." + method + @"<string>(_ => _ == ""42"")).Returns(true);
            ObjectUnderTest.Verify(_ => _.DoSomething(It." + method + @"<string>(_ => _ == ""42""), Times.Once);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_mock_method_invocation_using_property() => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public record Dto(string Name)
    {
    }

    public interface ITestMe
    {
        void DoSomething(Dto dto);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Setup(_ => _.DoSomething(It.Is<Dto>(_ => _.Name == ""42""))).Returns(true);
            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<Dto>(_ => _.Name == ""42"")), Times.Once);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_mock_method_invocation_using_direct_comparison_(
                                                                                         [Values("object", "bool", "string", "int")] string type,
                                                                                         [Values("!=", ">=", "<=", ">", "<")] string comparison,
                                                                                         [Values("null", "true", "false", "42", @"""42""")]string value)
            => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(" + type + @" data);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<" + type + @">(_ => _ " + comparison + " " + value + @")), Times.Once);
        }
    }
}
");

        [TestCase("object", "is not", "null")]
        [TestCase("bool", "is not", "true")]
        [TestCase("bool", "is not", "false")]
        public void No_issue_is_reported_for_mock_method_invocation_using_pattern_comparison_(string type, string comparison, string value) => No_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(" + type + @" data);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<" + type + @">(_ => _ " + comparison + " " + value + @")), Times.Once);
        }
    }
}
");

        [TestCase("object", "is", "null")]
        [TestCase("bool", "is", "true")]
        [TestCase("bool", "is", "false")]
        [TestCase("object", "==", "null")]
        [TestCase("bool", "==", "true")]
        [TestCase("bool", "==", "false")]
        [TestCase("string", "==", @"""42""")]
        [TestCase("int", "==", @"42")]
        public void An_issue_is_reported_for_mock_method_invocation_using_direct_comparison_(string type, string comparison, string value) => An_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(" + type + @" data);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<" + type + @">(_ => _ " + comparison + " " + value + @")), Times.Once);
        }
    }
}
");

        [TestCase("object", "==", "null")]
        [TestCase("bool", "==", "true")]
        [TestCase("bool", "==", "false")]
        [TestCase("string", "==", @"""42""")]
        [TestCase("int", "==", @"42")]
        public void An_issue_is_reported_for_mock_method_invocation_using_direct_comparison_on_constant_(string type, string comparison, string value) => An_issue_is_reported_for(@"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(" + type + @" data);
    }

    public class TestMeTests
    {
        private const " + type + " myConstant = " + value + @";

        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<" + type + @">(_ => _ " + comparison + @" myConstant)), Times.Once);
        }
    }
}
");

        [TestCase("object", "is", "null")]
        [TestCase("bool", "is", "true")]
        [TestCase("bool", "is", "false")]
        [TestCase("object", "==", "null")]
        [TestCase("bool", "==", "true")]
        [TestCase("bool", "==", "false")]
        [TestCase("string", "==", @"""42""")]
        [TestCase("int", "==", @"42")]
        public void Code_gets_fixed_for_mock_method_invocation_using_direct_comparison_(string type, string comparison, string value)
        {
            const string OriginalCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<#1#>(_ => _ #2# #3#)), Times.Once);
        }
    }
}
";

            const string FixedCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(#3#), Times.Once);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode.Replace("#1#", type).Replace("#2#", comparison).Replace("#3#", value), FixedCode.Replace("#1#", type).Replace("#3#", value));
        }

        [TestCase("object", "==", "null")]
        [TestCase("bool", "==", "true")]
        [TestCase("bool", "==", "false")]
        [TestCase("string", "==", @"""42""")]
        [TestCase("int", "==", @"42")]
        public void Code_gets_fixed_for_mock_method_invocation_using_direct_comparison_on_constant_(string type, string comparison, string value)
        {
            const string OriginalCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text);
    }

    public class TestMeTests
    {
        private const #1# myConstant = #3#;

        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(It.Is<#1#>(_ => _ #2# myConstant)), Times.Once);
        }
    }
}
";

            const string FixedCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text);
    }

    public class TestMeTests
    {
        private const #1# myConstant = #3#;

        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(myConstant), Times.Once);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode.Replace("#1#", type).Replace("#2#", comparison).Replace("#3#", value), FixedCode.Replace("#1#", type).Replace("#3#", value));
        }

        [TestCase("object", "is", "null")]
        [TestCase("bool", "is", "true")]
        [TestCase("bool", "is", "false")]
        [TestCase("object", "==", "null")]
        [TestCase("bool", "==", "true")]
        [TestCase("bool", "==", "false")]
        [TestCase("string", "==", @"""42""")]
        [TestCase("int", "==", @"42")]
        public void Code_gets_fixed_for_mock_method_invocation_using_direct_comparison_when_placed_on_new_line_(string type, string comparison, string value)
        {
            const string OriginalCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text, int value);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(
                                                  It.Is<#1#>(_ => _ #2# #3#),
                                                  123456789),
                                   Times.Once);
        }
    }
}
";

            const string FixedCode = @"
using System;

using Moq;

namespace Bla
{
    public interface ITestMe
    {
        void DoSomething(#1# text, int value);
    }

    public class TestMeTests
    {
        private Mock<ITestMe> ObjectUnderTest { get; set; }

        public void PrepareTest()
        {
            ObjectUnderTest = new Mock<ITestMe>();

            ObjectUnderTest.Verify(_ => _.DoSomething(
                                                  #3#,
                                                  123456789),
                                   Times.Once);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode.Replace("#1#", type).Replace("#2#", comparison).Replace("#3#", value), FixedCode.Replace("#1#", type).Replace("#3#", value));
        }

        protected override string GetDiagnosticId() => MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3120_CodeFixProvider();
    }
}