using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1025_EventNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Events);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Events);

        [Test]
        public void No_issue_is_reported_for_event_with_fitting_length([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("public event EventHandler " + name + ";");

        [Test]
        public void An_issue_is_reported_for_event_with_exceeding_length([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("public event EventHandler " + name + ";");

        protected override string GetDiagnosticId() => MiKo_1025_EventNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1025_EventNameLengthAnalyzer();
    }
}