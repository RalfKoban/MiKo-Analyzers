using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly IEnumerable<string> MeaninglessTextPhrases = new[]
                                                                                 {
                                                                                     "does implement",
                                                                                     "implements",
                                                                                     "that is called ",
                                                                                     "that is used for ",
                                                                                     "that is used to ",
                                                                                     "used for ",
                                                                                     "used to ",
                                                                                     "which is called ",
                                                                                     "which is used for ",
                                                                                     "which is used to ",
                                                                                 };

        private static readonly IEnumerable<string> MeaninglessPhrases = CreateMeaninglessPhrases();

        private static readonly IEnumerable<string> MeaninglessFieldPhrases = CreateMeaninglessPhrases().Except(new[] { "A ", "An ", "The " }).ToHashSet();

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
        public void No_issue_is_reported_for_exception_class_with_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// The exception to be thrown.
/// </summary>
public class TestMeException : System.Exception
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
        public void An_issue_is_reported_for_class_with_meaningless_phrase_([Values("Contains", "Contain", "Has")] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// " + phrase + @" whatever
/// </summary>
public class TestMe
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
        public void No_issue_is_reported_for_method_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Some documentation.
    /// </para>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase + @" whatever
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase_in_para_tag([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// <para>
    /// " + phrase + @" whatever
    /// </para>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase_in_text([ValueSource(nameof(MeaninglessTextPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// Some text " + phrase + @" whatever
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_field_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    private int DoSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    private int DoSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Some documentation.
    /// </para>
    /// </summary>
    private int DoSomething;
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase([ValueSource(nameof(MeaninglessFieldPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase + @" whatever
    /// </summary>
    private int DoSomething;
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase_in_para_tag([ValueSource(nameof(MeaninglessFieldPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// <para>
    /// " + phrase + @" whatever
    /// </para>
    /// </summary>
    private int DoSomething;
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase_in_text([ValueSource(nameof(MeaninglessTextPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// Some text " + phrase + @" whatever
    /// </summary>
    private int DoSomething;
}
");

        protected override string GetDiagnosticId() => MiKo_2012_MeaninglessSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2012_MeaninglessSummaryAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateMeaninglessPhrases()
        {
            var types = new[]
                            {
                                "Base", "Class", "Interface", "Method", "Field", "Property", "Event",
                                "Constructor", "Ctor", "Delegate", "Factory", "Creator", "Builder",
                                "Entity", "Model", "ViewModel", "Command", "Action", "Func", "Converter",
                            };

            var phrases = MeaninglessTextPhrases;

            var results = new List<string>
                              {
                                  "TestMe",
                                  "ITestMe",
                                  "A ",
                                  "An ",
                                  "Called ",
                                  "Does implement ",
                                  "Implement ",
                                  "Implements ",
                                  "Interaction logic ",
                                  "Implementation of ",
                                  "Default-Implementation of ",
                                  "Default implementation of ",
                                  "Impl ",
                                  "Default impl ",
                                  "Default-Impl ",
                                  "Is for ",
                                  "Is to ",
                                  "Is used for ",
                                  "Is used to ",
                                  "The ",
                                  "This ",
                                  "That ",
                                  "Used for ",
                                  "Used to ",
                                  "Uses ",
                              };

            results.AddRange(types);
            results.AddRange(phrases);
            results.AddRange(from type in types from phrase in phrases select type + " " + phrase);
            results.AddRange(phrases.Select(_ => _.ToLower()));
            results.AddRange(phrases.Select(_ => _.ToUpper()));

            results.Add("<see cref=\"ITestMe\"/>");
            results.Add("<see cref=\"ITestMe\" />");

            return results.ToHashSet();
        }
    }
}