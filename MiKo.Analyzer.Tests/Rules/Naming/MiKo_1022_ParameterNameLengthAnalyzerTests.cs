using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1022_ParameterNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_parameter_with_fitting_length([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("private bool DoSomething(int " + name + ") => true;");

        [Test]
        public void An_issue_is_reported_for_parameter_with_exceeding_length([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("private bool DoSomething(int " + name + ") => true;");

        [Test]
        public void No_issue_is_reported_for_ctor_parameter_with_fitting_length([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        [Test]
        public void An_issue_is_reported_for_ctor_parameter_with_exceeding_length([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("private DoSomething(int " + name + ") { }");

        protected override string GetDiagnosticId() => MiKo_1022_ParameterNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1022_ParameterNameLengthAnalyzer();

        private static IEnumerable<string> Fitting() => GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Parameters);

        private static IEnumerable<string> NonFitting() => GetAllAboveLengthOf(Constants.MaxNamingLengths.Parameters);
    }
}