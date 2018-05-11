using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture(Explicit = !NamingLengthAnalyzer.EnabledPerDefault)]
    public sealed class MiKo_1020_TypeNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        [Test, Combinatorial]
        public void No_issue_is_reported_for_type_with_fitting_length(
            [ValueSource(nameof(FittingTypes))] string type,
            [ValueSource(nameof(FittingNames))] string name) => No_issue_is_reported_for(@"

public " + type + " " + name + @"
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_type_with_exceeding_length(
            [ValueSource(nameof(FittingTypes))] string type,
            [ValueSource(nameof(ExceedingNames))] string name) => An_issue_is_reported_for(@"

public " + type + " " + name + @"
{
}
");

        [TestCase("TestFixture")]
        [TestCase("TestFixture()")]
        [TestCase("TestClass")]
        [TestCase("TestClass()")]
        public void No_issue_is_reported_for_test_class_with_exceeding_length(string attributeName) => No_issue_is_reported_for(@"

[" + attributeName + @"]
public class Abcdefghijklmnopqrstuvwxyz_abcdefghijklmnopqrstuvwxyz
{
}
");

        protected override string GetDiagnosticId() => MiKo_1020_TypeNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1020_TypeNameLengthAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> FittingTypes() => new[] { "interface", "class", "enum" };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> FittingNames() => GetAllWithMaxLengthOf(Constants.MaxNamingLengths.Types);

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> ExceedingNames() => GetAllAboveLengthOf(Constants.MaxNamingLengths.Types);
    }
}