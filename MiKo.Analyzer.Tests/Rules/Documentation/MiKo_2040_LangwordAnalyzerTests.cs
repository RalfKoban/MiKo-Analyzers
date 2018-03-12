using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2040_LangwordAnalyzerTests : CodeFixVerifier
    {
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

        protected override string GetDiagnosticId() => MiKo_2040_LangwordAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2040_LangwordAnalyzer();

        private static IEnumerable<string> WrongItems()
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
                results.Add(" " + token + " ");
                results.Add("(" + token + " ");
                results.Add(token + ",");
                results.Add(token + ";");
                results.Add(token + ".");
            }

            return results;
        }

        private static IEnumerable<string> CorrectItems() => new[] { "<see langword=\"true\" />", "<see langword=\"false\" />", "<see langword=\"null\" />", string.Empty };
    }
}