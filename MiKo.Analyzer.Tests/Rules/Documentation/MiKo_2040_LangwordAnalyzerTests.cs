using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2040_LangwordAnalyzerTests : CodeFixVerifier
    {
        private static readonly IEnumerable<string> CorrectItems = new[]
                                                                       {
                                                                           "<see langword=\"true\" />",
                                                                           "<see langword=\"true\"/>",
                                                                           "<see langword=\"false\" />",
                                                                           "<see langword=\"false\"/>",
                                                                           "<see langword=\"null\" />",
                                                                           "<see langword=\"null\"/>",
                                                                           string.Empty,
                                                                       };

        private static readonly IEnumerable<string> WrongItems = CreateWrongItems();

        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler<T> MyEvent;

    public void DoSomething() { }

    public int Age { get; set; }

    private bool m_field;
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_class([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_method([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_method_returnValue([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A " + finding + @" .
    /// </returns>
    public int Malform() => 42;
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_property([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_property_value([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// A " + finding + @" .
    /// </value>
    public int Malform { get; set; }
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_event([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_field([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_class([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_method([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Correct() { }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_property([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Correct { get; set; }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_event([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Correct;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_field([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Correct;
}
");

        [Test]
        public void Valid_example_for_documentation_is_not_reported_on_class([ValueSource(nameof(WrongItems))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <example>
/// <code>" + finding + @"</code>
/// </example>
public sealed class TestMe
{
}
");

        [Test]
        public void Wrong_example_for_documentation_is_reported_on_class([ValueSource(nameof(WrongItems))] string finding) => An_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <example>
/// " + finding + @"
/// </example>
public sealed class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2040_LangwordAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2040_LangwordAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateWrongItems()
        {
            var tokens = new List<string>();
            foreach (var token in new[] { "true", "false", "null" })
            {
                tokens.Add(token);
                tokens.Add(token.ToUpperInvariant());
                tokens.Add(char.ToUpperInvariant(token[0]) + token.Substring(1));
            }

            var results = new HashSet<string>();
            foreach (var token in tokens)
            {
                results.Add("<c>" + token + "</c>");
                results.Add("<see langref=\"" + token + "\" />");
                results.Add("<see langref=\"" + token + "\"/>");
                results.Add("<see langref=\"" + token + "\"></see>");
                results.Add("<see langref=\"" + token + "\" ></see>");
                results.Add("<seealso langref=\"" + token + "\" />");
                results.Add("<seealso langref=\"" + token + "\"/>");
                results.Add("<seealso langref=\"" + token + "\"></seealso>");
                results.Add("<seealso langref=\"" + token + "\" ></seealso>");
                results.Add(" " + token + " ");
                results.Add("(" + token + " ");
                results.Add("(" + token + ")");
                results.Add(" " + token + ")");
                results.Add(token + ",");
                results.Add(token + ";");
                results.Add(token + ".");
            }

            return results.OrderBy(_ => _).ToArray();
        }
    }
}