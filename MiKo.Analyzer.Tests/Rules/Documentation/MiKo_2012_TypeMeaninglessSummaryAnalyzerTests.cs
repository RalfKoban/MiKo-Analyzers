using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2012_TypeMeaninglessSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Some documentation.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_phrase([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

/// <summary>
/// " + phrase + @" whatever
/// </summary>
public class TestMe : ITestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_phrase_in_para_tag([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

/// <summary>
/// <para>
/// " + phrase + @" whatever
/// </para>
/// </summary>
public class TestMe : ITestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_see_cref_link([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

/// <summary>
/// <see cref='" + phrase + @"' />.
/// </summary>
public class TestMe : ITestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2012_TypeMeaninglessSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2012_TypeMeaninglessSummaryAnalyzer();

        private static IEnumerable<string> MeaninglessPhrases()
        {
            var types = new[] { "Class", "Interface", "Factory", "Creator", "Builder", "Entity", "Model", "ViewModel", "Command"};

            var phrases = new[]
                              {
                                  "that is used for ",
                                  "that is used to ",
                                  "used for ",
                                  "used to ",
                                  "for ",
                                  "to ",
                                  "which is used for ",
                                  "which is used to ",
                              };

            var results = new List<string>
                              {
                                  "TestMe",
                                  "ITestMe",
                                  "A ",
                                  "An ",
                                  "Does implement ",
                                  "Implement ",
                                  "Implements ",
                                  "Is for ",
                                  "Is to ",
                                  "Is used for ",
                                  "Is used to ",
                                  "The ",
                                  "This ",
                                  "That ",
                                  "Used for ",
                                  "Used to ",
                              };

            results.AddRange(types);
            results.AddRange(phrases);
            results.AddRange(from type in types from phrase in phrases select type + " " + phrase);
            results.AddRange(phrases.Select(_ => _.ToLower()));
            results.AddRange(phrases.Select(_ => _.ToUpper()));
            return new HashSet<string>(results);
        }
    }
}