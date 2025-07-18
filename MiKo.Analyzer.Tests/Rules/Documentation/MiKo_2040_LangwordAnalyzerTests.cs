﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2040_LangwordAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Terms = ["true", "false", "null",];

        private static readonly string[] CorrectItems =
                                                        [
                                                            "<see langword=\"true\" />",
                                                            "<see langword=\"true\"/>",
                                                            "<see langword=\"false\" />",
                                                            "<see langword=\"false\"/>",
                                                            "<see langword=\"null\" />",
                                                            "<see langword=\"null\"/>",
                                                            string.Empty,
                                                        ];

        private static readonly string[] WrongItemsWithoutCode = [.. CreateWrongItems(false, Terms).Take(TestLimit)];
        private static readonly string[] WrongItemsWithCode = [.. CreateWrongItems(true, Terms).Take(TestLimit)];

        private static readonly TestCaseData[] CodeFixData = [.. CreateCodeFixData().Take(TestLimit)];

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
        public void Wrong_documentation_is_reported_on_class_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_method_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_method_returnValue_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
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
        public void Wrong_documentation_is_reported_on_property_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_property_value_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
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
        public void Wrong_documentation_is_reported_on_event_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Malform;
}
");

        [Test]
        public void Wrong_documentation_is_reported_on_field_([ValueSource(nameof(WrongItemsWithCode))] string finding) => An_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Malform;
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void Wrong_documentation_is_reported_on_parameter() => An_issue_is_reported_for(2, @"
public sealed class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""flag"">True, if something. Otherwise False</param>
    private void Malform(bool flag) { }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_class_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something. " + finding + @"
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_method_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public void Correct() { }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_property_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public int Correct { get; set; }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_event_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    public event EventHandler Correct;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_field_([ValueSource(nameof(CorrectItems))] string finding) => No_issue_is_reported_for(@"
public sealed class TestMe
{
    /// <summary>
    /// Does something. " + finding + @"
    /// </summary>
    private string Correct;
}
");

        [Test]
        public void Valid_example_for_documentation_is_not_reported_on_class_([ValueSource(nameof(Terms))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <example>
/// <code>
///     if (xyz is " + finding + @")
///     {
///         return " + finding + @";
///     }
/// </code>
/// </example>
public sealed class TestMe
{
}
");

        [Test]
        public void Wrong_example_for_documentation_is_reported_on_class_([ValueSource(nameof(WrongItemsWithoutCode))] string finding) => An_issue_is_reported_for(@"
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

        [Test]
        public void Valid_code_inside_remarks_section_for_documentation_is_not_reported_on_class_([ValueSource(nameof(Terms))] string finding) => No_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
/// <remarks>
/// <para>
/// <code>
///     if (xyz is " + finding + @")
///     {
///         return " + finding + @";
///     }
/// <code>
/// </para>
/// </remarks>
public sealed class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_in_summary_([ValueSource(nameof(CodeFixData))] TestCaseData data)
        {
            const string Template = @"
/// <summary>
/// Does something. ### with some follow-up comment.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", data.Wrong), Template.Replace("###", data.Fixed));
        }

        [Test]
        public void Code_gets_fixed_in_combined_summary()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something. True with some follow-up comment, False else.
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Does something. <see langword=""true""/> with some follow-up comment, <see langword=""false""/> else.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void CodeFix_keeps_space_after_see_comment()
        {
            const string OriginalCode = @"
/// <returns><see langword=""true""/> if something, false else.</returns>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <returns><see langword=""true""/> if something, <see langword=""false""/> else.</returns>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void CodeFix_keeps_source_code_comment_untouched()
        {
            const string OriginalCode = @"
/// <example>
/// true and false are compared via null.
/// <code>
/// bool b1 = true;
/// bool b2 = false;
/// object o = null.
/// </code>
/// </example>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <example>
/// <see langword=""true""/> and <see langword=""false""/> are compared via <see langword=""null""/>.
/// <code>
/// bool b1 = true;
/// bool b2 = false;
/// object o = null.
/// </code>
/// </example>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_wrong_documentation_on_parameter_([Values(":", ",", "")] string delimiter)
        {
            var originalCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""flag"">True" + delimiter + @" if something. Otherwise False</param>
    private void Malform(bool flag) { }
}
";

            var fixedCode = @"
public sealed class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""flag""><see langword=""true""/>" + delimiter + @" if something. Otherwise <see langword=""false""/></param>
    private void Malform(bool flag) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2040_LangwordAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2040_LangwordAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2040_CodeFixProvider();

//// ncrunch: no coverage start

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateWrongItems(in bool withCode, params string[] tokens)
        {
            var words = new HashSet<string>();

            foreach (var token in tokens)
            {
                words.Add(token);
                words.Add(token.ToUpperCase());
                words.Add(token.ToUpperCaseAt(0));
            }

            var results = new HashSet<string>();

            if (withCode)
            {
                foreach (var token in words)
                {
                    results.Add("<code>" + token + "</code>");
                }
            }

            foreach (var token in words)
            {
                results.Add("<b>" + token + "</b>");
                results.Add("<c>" + token + "</c>");
                results.Add("<value>" + token + "</value>");
                results.Add(" " + token + " ");
                results.Add("(" + token + " ");
                results.Add("(" + token + ")");
                results.Add(" " + token + ")");
                results.Add("'" + token + "'");
                results.Add(token + ",");
                results.Add(token + ";");
                results.Add(token + ".");
                results.Add(token + "?");
                results.Add(token + "!");
                results.Add(token + ":");
                results.Add("<see langref=\"" + token + "\" />");
                results.Add("<see langref=\"" + token + "\"/>");
                results.Add("<see langref=\"" + token + "\"></see>");
                results.Add("<see langref=\"" + token + "\" ></see>");
                results.Add("<see langowrd=\"" + token + "\"/>"); // find typos
                results.Add("<see langwrod=\"" + token + "\"/>"); // find typos
                results.Add("<see langwowd=\"" + token + "\"/>"); // find typos
                results.Add("<seealso langref=\"" + token + "\" />");
                results.Add("<seealso langref=\"" + token + "\"/>");
                results.Add("<seealso langref=\"" + token + "\"></seealso>");
                results.Add("<seealso langref=\"" + token + "\" ></seealso>");
            }

            return results.OrderBy(_ => _);
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<TestCaseData> CreateCodeFixData()
        {
            foreach (var word in Terms)
            {
                foreach (var phrase in CreateWrongItems(true, word))
                {
                    // distinguish between XML and non-XML
                    var fixedPhrase = phrase.StartsWith('<')
                                      ? $@"<see langword=""{word}""/>"
                                      : phrase.Replace(word, $@"<see langword=""{word}""/>", StringComparison.OrdinalIgnoreCase);

                    yield return new TestCaseData
                                     {
                                         Wrong = phrase,
                                         Fixed = fixedPhrase,
                                     };
                }
            }
        }

//// ncrunch: no coverage end
    }
}