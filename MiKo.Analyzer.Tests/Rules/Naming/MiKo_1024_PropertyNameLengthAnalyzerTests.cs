using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1024_PropertyNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Properties);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Properties);

        [Test]
        public void No_issue_is_reported_for_property_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for("public bool " + name + " { get; set; }");

        [Test]
        public void An_issue_is_reported_for_property_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for("public bool " + name + " { get; set; }");

        protected override string GetDiagnosticId() => MiKo_1024_PropertyNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            return new MiKo_1024_PropertyNameLengthAnalyzer();
        }
    }
}