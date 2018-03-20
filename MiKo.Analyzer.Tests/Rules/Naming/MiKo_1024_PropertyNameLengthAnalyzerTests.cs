using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture(Explicit = !NamingLengthAnalyzer.EnabledPerDefault)]
    public sealed class MiKo_1024_PropertyNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_property_with_fitting_length([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("public bool " + name + " { get; set; }");

        [Test]
        public void An_issue_is_reported_for_property_with_exceeding_length([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("public bool " + name + " { get; set; }");

        protected override string GetDiagnosticId() => MiKo_1024_PropertyNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1024_PropertyNameLengthAnalyzer();

        private static IEnumerable<string> Fitting() => GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Properties);

        private static IEnumerable<string> NonFitting() => GetAllAboveLengthOf(Constants.MaxNamingLengths.Properties);
    }
}