using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1022_ParameterNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Parameters);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Parameters);

        [Test]
        public void No_issue_is_reported_for_parameter_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("class TestMe { bool DoSomething(int " + name + ") => true; }");

        [Test]
        public void An_issue_is_reported_for_parameter_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("class TestMe { bool DoSomething(int " + name + ") => true; }");

        [Test]
        public void No_issue_is_reported_for_ctor_parameter_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("class TestMe { public TestMe(int " + name + ") { } }");

        [Test]
        public void An_issue_is_reported_for_ctor_parameter_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("class TestMe { public TestMe(int " + name + ") { } }");

        [Test]
        public void No_issue_is_reported_for_local_function_parameter_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("class TestMe { public void Something() { void Local(int " + name + ") { } } }");

        [Test]
        public void An_issue_is_reported_for_local_function_parameter_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("class TestMe { public void Something() { void Local(int " + name + ") { } } }");

        protected override string GetDiagnosticId() => MiKo_1022_ParameterNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            return new MiKo_1022_ParameterNameLengthAnalyzer();
        }
    }
}