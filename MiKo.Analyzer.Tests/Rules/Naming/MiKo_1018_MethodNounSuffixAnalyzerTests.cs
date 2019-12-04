using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly IEnumerable<string> ValidMethodNames = new[]
                                                                           {
                                                                               "Act",
                                                                               "AddDocumentation",
                                                                               "Adopt",
                                                                               "AnalyzeDeclaration",
                                                                               "CancelOperation",
                                                                               "CanRemoveConnection",
                                                                               "ClearDocumentation",
                                                                               "Compare",
                                                                               "CompileTimeValidateCollection",
                                                                               "CreateDocumentation",
                                                                               "DeleteDocumentation",
                                                                               "DeselectConnection",
                                                                               "DoSomething",
                                                                               "EnsureLocation",
                                                                               "FindBison",
                                                                               "FindDocumentation",
                                                                               "GetDocumentation",
                                                                               "HandleSelection",
                                                                               "HasConnection",
                                                                               "Install",
                                                                               "InvertSelection",
                                                                               "LoadConfiguration",
                                                                               "LogException",
                                                                               "Manipulate",
                                                                               "ParseIdentification",
                                                                               "PopAnnotation",
                                                                               "PrepareConnection",
                                                                               "PushAnnotation",
                                                                               "QueryDocumentation",
                                                                               "ReadConfiguration",
                                                                               "RedoInstallation",
                                                                               "RefreshDocumentation",
                                                                               "RegisterAction",
                                                                               "RemoveDocumentation",
                                                                               "ResetDocumentation",
                                                                               "RestartSimulation",
                                                                               "RestoreConfiguration",
                                                                               "RollbackTransaction",
                                                                               "SaveConfiguration",
                                                                               "SelectConnection",
                                                                               "SetDocumentation",
                                                                               "StartSimulation",
                                                                               "StopSimulation",
                                                                               "StoreConfiguration",
                                                                               "SubscribeAction",
                                                                               "ToComparison",
                                                                               "TraceInformation",
                                                                               "TranslateDocumentation",
                                                                               "TryConnection",
                                                                               "UndoInstallation",
                                                                               "UnregisterAction",
                                                                               "UnsubscribeAction",
                                                                               "UpdateConfiguration",
                                                                               "ValidateCollection",
                                                                               "VerifyConnection",
                                                                               "WrapConnection",
                                                                               "WriteInformation",
                                                                           };

        private static readonly IEnumerable<string> InvalidMethodNames = new[]
                                                                             {
                                                                                 "ApplyComparison",
                                                                                 "Configuration",
                                                                                 "DoComparison",
                                                                                 "ExecuteManipulation",
                                                                                 "Initialization",
                                                                                 "Installation",
                                                                                 "RunAdoption",
                                                                             };

        [Test]
        public void No_issue_is_reported_for_method_with_name_([ValueSource(nameof(ValidMethodNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_name_([ValueSource(nameof(InvalidMethodNames))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + name + @"()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_name_(
                                                                [ValueSource(nameof(Tests))] string test,
                                                                [ValueSource(nameof(InvalidMethodNames))] string name)
            => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void " + name + @"()
    {
    }
}
");

        [TestCase(null, ExpectedResult = null, Description = "There is no verb available")]
        [TestCase("", ExpectedResult = "", Description = "There is no verb available")]
        [TestCase(" ", ExpectedResult = " ", Description = "There is no verb available")]
        [TestCase("Caption", ExpectedResult = "Caption", Description = "There is no verb available")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("Adaptation", ExpectedResult = "Adapt")]
        [TestCase("Adoption", ExpectedResult = "Adopt")]
        [TestCase("Comparison", ExpectedResult = "Compare")]
        [TestCase("Configuration", ExpectedResult = "Configure")]
        [TestCase("Connection", ExpectedResult = "Connect")]
        [TestCase("Creation", ExpectedResult = "Create")]
        [TestCase("Documentation", ExpectedResult = "Document")]
        [TestCase("Estimation", ExpectedResult = "Estimate")]
        [TestCase("Exception", ExpectedResult = "Exception", Description = "The noun is most-probably meant in such case")]
        [TestCase("Information", ExpectedResult = "Inform")]
        [TestCase("Initialisation", ExpectedResult = "Initialise")]
        [TestCase("Initialization", ExpectedResult = "Initialize")]
        [TestCase("Installation", ExpectedResult = "Install")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Registration", ExpectedResult = "Register")]
        [TestCase("Stabilization", ExpectedResult = "Stabilize")]
        [TestCase("Uninstallation", ExpectedResult = "Uninstall")]
        [TestCase("Action", ExpectedResult = "Action", Description = "There is no verb available")]
        [TestCase("DoAction", ExpectedResult = "DoAction")]
        [TestCase("HasAction", ExpectedResult = "HasAction")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("Function", ExpectedResult = "Function", Description = "There is no verb available")]
        [TestCase("DoFunction", ExpectedResult = "DoFunction")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("Func", ExpectedResult = "Function")]
        [TestCase("DoFunc", ExpectedResult = "DoFunction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        public string A_proper_name_is_found(string name)
        {
            MiKo_1018_MethodNounSuffixAnalyzer.TryFindBetterName(name, out var result);
            return result;
        }

        protected override string GetDiagnosticId() => MiKo_1018_MethodNounSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1018_MethodNounSuffixAnalyzer();
    }
}