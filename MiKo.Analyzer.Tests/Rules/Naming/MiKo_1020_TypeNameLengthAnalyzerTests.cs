using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1020_TypeNameLengthAnalyzerTests : CodeFixVerifier
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

        private static IEnumerable<string> FittingTypes() => new[] { "interface", "class", "enum" };

        private static IEnumerable<string> FittingNames() => new[]
                                                          {
                                                              "A",
                                                              "Ab",
                                                              "Abc",
                                                              "Abcd",
                                                              "Abcde",
                                                              "Abcdef",
                                                              "Abcdefg",
                                                              "Abcdefgh",
                                                              "Abcdefghi",
                                                              "Abcdefghij",
                                                              "Abcdefghijk",
                                                              "Abcdefghijkl",
                                                              "Abcdefghijklm",
                                                              "Abcdefghijklmn",
                                                              "Abcdefghijklmno",
                                                              "Abcdefghijklmnop",
                                                              "Abcdefghijklmnopq",
                                                              "Abcdefghijklmnopqr",
                                                              "Abcdefghijklmnopqrs",
                                                              "Abcdefghijklmnopqrst",
                                                              "Abcdefghijklmnopqrstu",
                                                              "Abcdefghijklmnopqrstuv",
                                                              "Abcdefghijklmnopqrstuvw",
                                                              "Abcdefghijklmnopqrstuvwx",
                                                              "Abcdefghijklmnopqrstuvwxy",
                                                              "Abcdefghijklmnopqrstuvwxyz01234567890äöü",
                                                          };

        private static IEnumerable<string> ExceedingNames() => new[]
                                                          {
                                                              "Abcdefghijklmnopqrstuvwxyz01234567890äöüß",
                                                          };
    }
}