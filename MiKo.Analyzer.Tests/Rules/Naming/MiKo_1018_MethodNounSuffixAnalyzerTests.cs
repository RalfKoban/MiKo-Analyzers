using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidMethodNames =
                                                            [
                                                                "Act",
                                                                "ActivateSelection",
                                                                "AddDocumentation",
                                                                "Adopt",
                                                                "AnalyzeDeclaration",
                                                                "BuildConfiguration",
                                                                "CalculateLocation",
                                                                "CancelOperation",
                                                                "CanRemoveConnection",
                                                                "ChangeDirection",
                                                                "ClearDocumentation",
                                                                "CloneDocumentation",
                                                                "CloseConnection",
                                                                "CollectInstallationInformation",
                                                                "CollectSystemInformation",
                                                                "Compare",
                                                                "CompileTimeValidateCollection",
                                                                "ContinueInstallation",
                                                                "CreateDocumentation",
                                                                "DeactivateSelection",
                                                                "DebugConfiguration",
                                                                "DelayInstallation",
                                                                "DeleteDocumentation",
                                                                "DeregisterAction",
                                                                "DeselectConnection",
                                                                "DoSomething",
                                                                "EnsureLocation",
                                                                "FindBison",
                                                                "FindDocumentation",
                                                                "FreeConfiguration",
                                                                "GenerateIllustration",
                                                                "GetDocumentation",
                                                                "HandleSelection",
                                                                "HasConnection",
                                                                "InformAboutSituation",
                                                                "InformedAboutSituation",
                                                                "InformsAboutSituation",
                                                                "InitConfiguration",
                                                                "InitializeConfiguration",
                                                                "Install",
                                                                "InvertSelection",
                                                                "ItemBelongsToApplication",
                                                                "JumpToPage",
                                                                "JumpToSituation",
                                                                "LoadConfiguration",
                                                                "LogException",
                                                                "LogInformation",
                                                                "Manipulate",
                                                                "MirrorSelection",
                                                                "ModifySelection",
                                                                "OnTriggerSelection",
                                                                "OpenConnection",
                                                                "ParseIdentification",
                                                                "PauseInstallation",
                                                                "PopAnnotation",
                                                                "PrepareConnection",
                                                                "PromptForAuthentication",
                                                                "PushAnnotation",
                                                                "QueryDocumentation",
                                                                "ReadConfiguration",
                                                                "RebuildConfiguration",
                                                                "RecordConfiguration",
                                                                "RecoverInformation",
                                                                "RedoInstallation",
                                                                "RefreshDocumentation",
                                                                "RegisterAction",
                                                                "ReleaseCommunication",
                                                                "ReloadConfiguration",
                                                                "RemoveDocumentation",
                                                                "ReplaceConfiguration",
                                                                "ReportInformation",
                                                                "RequestConfirmation",
                                                                "ResetDocumentation",
                                                                "ResolveBindingInformation",
                                                                "RestartSimulation",
                                                                "RestoreConfiguration",
                                                                "ResumeInstallation",
                                                                "RetrieveInformation",
                                                                "RollbackTransaction",
                                                                "SaveConfiguration",
                                                                "SelectConnection",
                                                                "SendNotification",
                                                                "SetDocumentation",
                                                                "SetupDocumentation",
                                                                "ShowConfirmationDialog",
                                                                "SimulateConnection",
                                                                "SomethingWithParenthesis",
                                                                "SomethingWithParenthesisPosition",
                                                                "SomethingWithPosition",
                                                                "SomethingWithSituation",
                                                                "SortDocumentation",
                                                                "StartSimulation",
                                                                "StopSimulation",
                                                                "StoreConfiguration",
                                                                "SubscribeAction",
                                                                "SubtractPosition",
                                                                "SuspendProgress",
                                                                "ThrowException",
                                                                "ThrowInvalidOperation",
                                                                "ToComparison",
                                                                "TraceInformation",
                                                                "TranslateDocumentation",
                                                                "TriggerSelection",
                                                                "TryConnection",
                                                                "UndoInstallation",
                                                                "UnlockConfiguration",
                                                                "UnregisterAction",
                                                                "UnsubscribeAction",
                                                                "UpdateConfiguration",
                                                                "ValidateCollection",
                                                                "VerifyConnection",
                                                                "WithInformation",
                                                                "WrapConnection",
                                                                "WriteInformation",
                                                                "ZoomToSelection",
                                                            ];

        private static readonly string[] InvalidMethodNames =
                                                              [
                                                                  "ApplyComparison",
                                                                  "Configuration",
                                                                  "DoComparison",
                                                                  "ExecuteManipulation",
                                                                  "Initialization",
                                                                  "Installation",
                                                                  "RunAdoption",
                                                                  "RunExecution",
                                                              ];

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

        [Test]
        public void No_issue_is_reported_for_test_method_with_name_([ValueSource(nameof(InvalidMethodNames))] string name, [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void " + name + @"()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_name_([ValueSource(nameof(ValidMethodNames))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + name + @"()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_name_([ValueSource(nameof(InvalidMethodNames))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void Something()
    {
        void " + name + @"()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_local_function_with_name_([ValueSource(nameof(InvalidMethodNames))] string name, [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
public class TestMe
{
    [" + test + @"]
    public void Something()
    {
        void " + name + @"()
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void Installation()
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void Install()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1018_MethodNounSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1018_MethodNounSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1018_CodeFixProvider();
    }
}