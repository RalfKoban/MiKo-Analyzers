using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1011_DoMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CanDock")]
        [TestCase("CanDocument")]
        [TestCase("CanDouble")]
        [TestCase("CanDown")]
        [TestCase("CanDownload")]
        [TestCase("Dock")]
        [TestCase("Doctor")]
        [TestCase("Document")]
        [TestCase("Domain")]
        [TestCase("Done")]
        [TestCase("Dot")]
        [TestCase("Double")]
        [TestCase("Doubt")]
        [TestCase("Down")]
        [TestCase("Download")]
        [TestCase("IsDown")]
        [TestCase("IsInDoubt")]
        [TestCase("Whatever")]
        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices")]
        [TestCase("Dogfood")]
        public void No_issue_is_reported_for_correctly_named_method_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("CanDo")]
        [TestCase("CanDoWhatever")]
        [TestCase("CanDoDock")]
        [TestCase("CanDoDocument")]
        [TestCase("CanDoDouble")]
        [TestCase("CanDoDown")]
        [TestCase("CanDoDownload")]
        [TestCase("Do")]
        [TestCase("DoDock")]
        [TestCase("DoDoctor")]
        [TestCase("DoDocument")]
        [TestCase("DoDomain")]
        [TestCase("DoDone")]
        [TestCase("DoDot")]
        [TestCase("DoDouble")]
        [TestCase("DoDoubt")]
        [TestCase("DoDown")]
        [TestCase("DoDownload")]
        [TestCase("DoWhatever")]
        [TestCase("IsDoDown")]
        public void An_issue_is_reported_for_wrong_named_method_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("CanDock")]
        [TestCase("CanDocument")]
        [TestCase("CanDouble")]
        [TestCase("CanDown")]
        [TestCase("CanDownload")]
        [TestCase("Dock")]
        [TestCase("Doctor")]
        [TestCase("Document")]
        [TestCase("Domain")]
        [TestCase("Done")]
        [TestCase("Dot")]
        [TestCase("Double")]
        [TestCase("Doubt")]
        [TestCase("Down")]
        [TestCase("Download")]
        [TestCase("IsDown")]
        [TestCase("IsInDoubt")]
        [TestCase("Whatever")]
        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices")]
        [TestCase("Dogfood")]
        public void No_issue_is_reported_for_correctly_named_local_function_(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("CanDo")]
        [TestCase("CanDoWhatever")]
        [TestCase("CanDoDock")]
        [TestCase("CanDoDocument")]
        [TestCase("CanDoDouble")]
        [TestCase("CanDoDown")]
        [TestCase("CanDoDownload")]
        [TestCase("Do")]
        [TestCase("DoDock")]
        [TestCase("DoDoctor")]
        [TestCase("DoDocument")]
        [TestCase("DoDomain")]
        [TestCase("DoDone")]
        [TestCase("DoDot")]
        [TestCase("DoDouble")]
        [TestCase("DoDoubt")]
        [TestCase("DoDown")]
        [TestCase("DoDownload")]
        [TestCase("DoWhatever")]
        [TestCase("IsDoDown")]
        public void An_issue_is_reported_for_wrong_named_local_function_(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("DoesSupport")]
        public void An_issue_is_reported_for_wrong_interface_named_method_(string methodName) => An_issue_is_reported_for(@"
public interface TestMe
{
    public bool " + methodName + @"();
}
");

        [TestCase("Whatever_it_Does")]
        [TestCase("Do_whatever_you_want_to_do")]
        public void No_issue_is_reported_for_test_method_(string methodName) => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void " + methodName + @"();
}
");

        [TestCase("Whatever_it_Does")]
        [TestCase("Do_whatever_you_want_to_do")]
        public void No_issue_is_reported_for_local_function_inside_test_method_(string methodName) => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void Something()
    {
        void " + methodName + @"() { }
    }
}
");

        [TestCase("Do", "Execute")]
        [TestCase("CanDo", "CanExecute")]
        [TestCase("DoExecute", "Execute")]
        [TestCase("CanDoExecute", "CanExecute")]
        [TestCase("DoDock", "Dock")]
        [TestCase("CanDoDock", "CanDock")]
        public void Code_gets_fixed_for_method_(string method, string wanted) => VerifyCSharpFix(
                                                                                      @"using System; class TestMe { void " + method + "() { } }",
                                                                                      @"using System; class TestMe { void " + wanted + "() { } }");

        [TestCase("Do", "Execute")]
        [TestCase("CanDo", "CanExecute")]
        [TestCase("DoExecute", "Execute")]
        [TestCase("CanDoExecute", "CanExecute")]
        [TestCase("DoDock", "Dock")]
        [TestCase("CanDoDock", "CanDock")]
        public void Code_gets_fixed_for_local_function_(string method, string wanted) => VerifyCSharpFix(
                                                                                      @"using System; class TestMe { void Something() { void " + method + "() { } } }",
                                                                                      @"using System; class TestMe { void Something() { void " + wanted + "() { } } }");

        protected override string GetDiagnosticId() => MiKo_1011_DoMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1011_DoMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1011_CodeFixProvider();
    }
}