using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1049_RequirementTermAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Marker = ["Must", "Need", "Shall", "Should", "Will", "Would"];

        [Test]
        public void No_issue_is_reported_for_correctly_named_symbols() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_something;

    public event EventHandler SomethingEvent;

    public bool Something { get; set;}

    public void DoSomething()
    {
        void SomethingCore() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_([Values("RefreshAllChildren", "CreateShallowCopy", "NeedsLicense", "Something_needs_license")] string methodName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + methodName + @"()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class " + marker + @"TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_something" + marker + @";
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_const_field_([ValueSource(nameof(Marker))] string marker) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string Bla" + marker + @" = ""something"";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_event_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Something" + marker + @"Event;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool " + marker + @"Something { get; set;}
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + marker + @"DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_local_function_([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        void " + marker + @"DoSomethingCore() { }
    }
}
");

        [TestCase("SomethingShouldFail", "SomethingFails")]
        [TestCase("SomethingShouldCallAnything", "SomethingCallsAnything")]
        [TestCase("SomethingShould_CallAnything", "SomethingCallsAnything")]
        [TestCase("SomethingShouldCreateAnything", "SomethingCreatesAnything")]
        [TestCase("SomethingShould_CreateAnything", "SomethingCreatesAnything")]
        [TestCase("SomethingShouldHaveAnything", "SomethingHaveAnything")]
        [TestCase("SomethingShouldNotHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldNtHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldntHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldAnything", "SomethingDoesAnything")]
        [TestCase("SomethingShouldBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShouldNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShouldNtBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShouldntBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShould_BeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShouldRunAnything", "SomethingRunsAnything")]
        [TestCase("SomethingShould_RunAnything", "SomethingRunsAnything")]
        [TestCase("SomethingShouldReturnAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShouldReturnsAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShould_ReturnAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShould_ReturnsAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShouldThrowAnything", "SomethingThrowsAnything")]
        [TestCase("SomethingShould_ThrowAnything", "SomethingThrowsAnything")]
        [TestCase("SomethingWillBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingWillNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShallBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShallNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("Something_should_call", "Something_calls")]
        [TestCase("Something_should_create", "Something_creates")]
        [TestCase("Something_should_fail", "Something_fails")]
        [TestCase("Something_should_Fail", "Something_fails")]
        [TestCase("Something_should_have_Anything", "Something_has_Anything")]
        [TestCase("Something_should_not_have_Anything", "Something_does_not_have_Anything")]
        [TestCase("Something_should_be_Anything", "Something_is_Anything")]
        [TestCase("Something_should_Be_Anything", "Something_is_Anything")]
        [TestCase("Something_should_not_be_Anything", "Something_is_not_Anything")]
        [TestCase("Something_should_return_Anything", "Something_returns_Anything")]
        [TestCase("Something_should_returns_Anything", "Something_returns_Anything")]
        [TestCase("Something_should_throw_Anything", "Something_throws_Anything")]
        [TestCase("Something_should_run_Anything", "Something_runs_Anything")]
        [TestCase("Something_should_run", "Something_runs")]
        [TestCase("Something_ShouldDoNothingForReasons", "Something_DoesNothingForReasons")]
        [TestCase("Something_ShouldDocumentNothingForReasons", "Something_DoesDocumentNothingForReasons")]
        [TestCase("Something_ShouldHandleStuff", "Something_HandlesStuff")]
        [TestCase("Something_Should_HandleStuff", "Something_HandlesStuff")]
        public void Code_gets_fixed_for_method_(string method, string wanted) => VerifyCSharpFix(
                                                                                             "using System; class TestMe { void " + method + "() { } }",
                                                                                             "using System; class TestMe { void " + wanted + "() { } }");

        [TestCase("AppsShouldBeHandled", "AppsAreHandled")]
        [TestCase("AppShouldBeHandled", "AppIsHandled")]
        [TestCase("AccessShouldBeHandled", "AccessIsHandled")]
        [TestCase("AreasShouldBeHandled", "AreasAreHandled")]
        [TestCase("ApisShouldBeAvailable", "ApisAreAvailable")]
        [TestCase("APIsShouldBeAvailable", "APIsAreAvailable")]
        [TestCase("UrisShouldBeAvailable", "UrisAreAvailable")]
        [TestCase("URIsShouldBeAvailable", "URIsAreAvailable")]
        [TestCase("Apps_should_be_handled", "Apps_are_handled")]
        [TestCase("Access_should_be_handled", "Access_is_handled")]
        [TestCase("Apis_should_be_available", "Apis_are_available")]
        [TestCase("APIs_should_be_available", "APIs_are_available")]
        [TestCase("Uris_should_be_available", "Uris_are_available")]
        [TestCase("URIs_should_be_available", "URIs_are_available")]
        [TestCase("Areas_should_be_large", "Areas_are_large")]
        [TestCase("BusShouldBeLate", "BusIsLate")]
        [TestCase("Bus_should_be_late", "Bus_is_late")]
        [TestCase("GasShouldBeOn", "GasIsOn")]
        [TestCase("Gas_should_be_on", "Gas_is_on")]
        [TestCase("TestRunsShouldBeOK", "TestRunsAreOK")]
        [TestCase("Test_runs_should_Be_OK", "Test_runs_are_OK")]
        [TestCase("ThisShouldBeOK", "ThisIsOK")]
        [TestCase("This_should_be_OK", "This_is_OK")]
        public void Code_gets_fixed_for_method_in_plural_(string method, string wanted) => VerifyCSharpFix(
                                                                                                       "using System; class TestMe { void " + method + "() { } }",
                                                                                                       "using System; class TestMe { void " + wanted + "() { } }");

        [TestCase("SomethingShouldFail", "SomethingFails")]
        [TestCase("SomethingShouldHaveAnything", "SomethingHaveAnything")]
        [TestCase("SomethingShouldNotHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldNtHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldntHaveAnything", "SomethingDoNotHaveAnything")]
        [TestCase("SomethingShouldAnything", "SomethingDoesAnything")]
        [TestCase("SomethingShouldBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShouldNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShouldNtBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShouldntBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShould_BeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShouldReturnAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShould_ReturnAnything", "SomethingReturnsAnything")]
        [TestCase("SomethingShouldThrowAnything", "SomethingThrowsAnything")]
        [TestCase("SomethingShould_ThrowAnything", "SomethingThrowsAnything")]
        [TestCase("SomethingWillBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingWillNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("SomethingShallBeAnything", "SomethingIsAnything")]
        [TestCase("SomethingShallNotBeAnything", "SomethingIsNotAnything")]
        [TestCase("Something_should_fail", "Something_fails")]
        [TestCase("Something_should_have_Anything", "Something_has_Anything")]
        [TestCase("Something_should_not_have_Anything", "Something_does_not_have_Anything")]
        [TestCase("Something_should_be_Anything", "Something_is_Anything")]
        [TestCase("Something_should_not_be_Anything", "Something_is_not_Anything")]
        [TestCase("Something_should_not_do_Anything", "Something_does_not_do_Anything")]
        [TestCase("Something_should_return_Anything", "Something_returns_Anything")]
        [TestCase("Something_should_throw_Anything", "Something_throws_Anything")]
        [TestCase("Something_should_handle_Anything", "Something_handles_Anything")]
        public void Code_gets_fixed_for_local_function_(string method, string wanted) => VerifyCSharpFix(
                                                                                                     "using System; class TestMe { void DoSomething() { void " + method + "() { } } }",
                                                                                                     "using System; class TestMe { void DoSomething() { void " + wanted + "() { } } }");

        protected override string GetDiagnosticId() => MiKo_1049_RequirementTermAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1049_RequirementTermAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1049_CodeFixProvider();
    }
}