using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3131_DoNotUseMocksInProductionCodeAnalyzerTests : CodeFixVerifier
    {
        private const string ProjectName = "MiKoSolutions.Analyzers.AdHoc.Project";

        [Test]
        public void No_issue_is_reported_for_when_no_mock_is_used_in_type() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void SomeTest()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_project_using_([Values(Constants.NSubstitute.Namespace, Constants.Moq.Namespace)] string namespaceName) => No_issue_is_reported_for(@"
using System;

using " + namespaceName + @";

namespace Bla
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_using_([Values(Constants.NSubstitute.Namespace, Constants.Moq.Namespace)] string namespaceName) => An_issue_is_reported_for(@"
using System;

using " + namespaceName + @";

namespace Bla
{
}
", testProjectName: ProjectName);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_NSubstitute_invocation() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private IDisposable Disposable = NSubstitute.Substitute.For<IDisposable>();
    }
}
", testProjectName: ProjectName);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_Moq_invocation() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private IDisposable Disposable = Moq.Mock.Of<IDisposable>();
    }
}
", testProjectName: ProjectName);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_Moq_creation() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private Moq.Mock<IDisposable> Disposable = new Moq.Mock<IDisposable>();
    }
}
", testProjectName: ProjectName);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_Moq_Object_creation() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private IDisposable Disposable = new Moq.Mock<IDisposable>().Object;
    }
}
", testProjectName: ProjectName);

        [Test]
        public void No_issue_is_reported_for_test_project_using_RhinoMocks_namespace() => No_issue_is_reported_for(@"
using System;

using " + Constants.Rhino.Namespace + @";

namespace Bla
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_using_RhinoMocks_namespace() => An_issue_is_reported_for(@"
using System;

using " + Constants.Rhino.Namespace + @";

namespace Bla
{
}
", testProjectName: ProjectName);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [TestCase("CreateMock")]
        [TestCase("CreateMockObject")]
        [TestCase("CreateMockWithRemoting")]
        [TestCase("CreateMultiMock")]
        [TestCase("DynamicMock")]
        [TestCase("DynamicMockWithRemoting")]
        [TestCase("DynamicMultiMock")]
        [TestCase("GenerateDynamicMockWithRemoting")]
        [TestCase("GenerateMock")]
        [TestCase("GeneratePartialMock")]
        [TestCase("GenerateStrictMock")]
        [TestCase("GenerateStrictMockWithRemoting")]
        [TestCase("GenerateStub")]
        [TestCase("PartialMock")]
        [TestCase("PartialMultiMock")]
        [TestCase("RemotingMock")]
        [TestCase("StrictMock")]
        [TestCase("StrictMockWithRemoting")]
        [TestCase("StrictMultiMock")]
        public void An_issue_is_reported_for_RhinoMocks_invocation_of_(string method) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private IDisposable Disposable = Rhino.Mocks.MockRepository." + method + @"<IDisposable>();
    }
}
", testProjectName: ProjectName);

        protected override string GetDiagnosticId() => MiKo_3131_DoNotUseMocksInProductionCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3131_DoNotUseMocksInProductionCodeAnalyzer();
    }
}