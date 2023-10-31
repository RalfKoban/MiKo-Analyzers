using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1020_TypeNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] FittingTypes = { "interface", "class", "enum" };
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Types);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.Types);

        [Test, Combinatorial]
        public void No_issue_is_reported_for_type_with_fitting_length_(
                                                                   [ValueSource(nameof(FittingTypes))] string type,
                                                                   [ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"

public " + type + " " + name + @"
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_type_with_exceeding_length_(
                                                                     [ValueSource(nameof(FittingTypes))] string type,
                                                                     [ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"

public " + type + " " + name + @"
{
}
");

        [TestCase("TestFixture")]
        [TestCase("TestFixture()")]
        [TestCase("TestClass")]
        [TestCase("TestClass()")]
        public void No_issue_is_reported_for_test_class_with_exceeding_length_(string attributeName) => No_issue_is_reported_for(@"

[" + attributeName + @"]
public class Abcdefghijklmnopqrstuvwxyz_abcdefghijklmnopqrstuvwxyz
{
}
");

        protected override string GetDiagnosticId() => MiKo_1020_TypeNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            return new MiKo_1020_TypeNameLengthAnalyzer();
        }
    }
}