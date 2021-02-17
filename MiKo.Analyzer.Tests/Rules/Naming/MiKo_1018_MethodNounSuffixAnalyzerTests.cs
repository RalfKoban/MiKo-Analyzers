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
                                                                               "CalculateLocation",
                                                                               "CancelOperation",
                                                                               "CanRemoveConnection",
                                                                               "ClearDocumentation",
                                                                               "CloneDocumentation",
                                                                               "CloseConnection",
                                                                               "Compare",
                                                                               "CompileTimeValidateCollection",
                                                                               "CreateDocumentation",
                                                                               "DeleteDocumentation",
                                                                               "DeregisterAction",
                                                                               "DeselectConnection",
                                                                               "DoSomething",
                                                                               "EnsureLocation",
                                                                               "FindBison",
                                                                               "FindDocumentation",
                                                                               "FreeConfiguration",
                                                                               "GetDocumentation",
                                                                               "HandleSelection",
                                                                               "HasConnection",
                                                                               "Install",
                                                                               "InvertSelection",
                                                                               "LoadConfiguration",
                                                                               "LogException",
                                                                               "LogInformation",
                                                                               "Manipulate",
                                                                               "OpenConnection",
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
                                                                               "RequestConfirmation",
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
                                                                               "WithInformation",
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

        protected override string GetDiagnosticId() => MiKo_1018_MethodNounSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1018_MethodNounSuffixAnalyzer();
    }
}