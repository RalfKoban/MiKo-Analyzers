using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzerTests : CodeFixVerifier
    {
        private const string Marker = "###";

        private static readonly string[] Phrases = CreatePhrases(Marker);

        [Test]
        public void No_issue_is_reported_for_non_inheritable_link_in_XML_summary_of_method_([ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "<see cref=\"\"string.Format(string,object)\"\"/>") + @"
    /// </summary>
    public void Bla()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_inheritable_link_in_XML_summary_of_property_([ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "<see cref=\"\"string.Format(string,object)\"\"/>") + @"
    /// </summary>
    public int DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_non_inheritable_link_in_XML_summary_of_event_([ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "<see cref=\"\"string.Format(string,object)\"\"/>") + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test] // fields cannot be inherited and defined a second time
        public void No_issue_is_reported_for_XML_summary_of_field_([ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "bla") + @"
    /// </summary>
    private int doSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_XML_summary_of_named_enum_type([ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
/// <summary>
/// " + phrase.Replace(Marker, "bla") + @"
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_named_type_([Values("interface", "class")] string type, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ITestMe
    {
    }

    /// <summary>
    /// " + phrase.Replace(Marker, "ITestMe") + @"
    /// </summary>
    public " + type + @" TestMe : ITestMe
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_method_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
    void DoSomething();
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "ITestMe.DoSomething") + @"
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_overridden_method_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "bla") + @"
    /// </summary>
    public override string ToString() => ""something"";
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_property_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
    int DoSomething { get; set; }
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "bla") + @"
    /// </summary>
    public int DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_event_([ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
    void event EventHandler MyEvent;
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase.Replace(Marker, "bla") + @"
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void Code_gets_fixed_for_type()
        {
            const string OriginalCode = @"
/// <summary>
/// <see cref='System.Object'/>
/// </summary>
class TestMe { }";

            const string FixedCode = @"
/// <inheritdoc/>
class TestMe { }";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method()
        {
            const string OriginalCode = @"
public interface ITestMe
{
    void DoSomething();
}

/// <summary>
/// Bla
/// </summary>
class TestMe : ITestMe
{
    /// <summary>
    /// <see cref='ITestMe.DoSomething'/>
    /// </summary>
    public void DoSomething() { }
}";

            const string FixedCode = @"
public interface ITestMe
{
    void DoSomething();
}

/// <summary>
/// Bla
/// </summary>
class TestMe : ITestMe
{
    /// <inheritdoc/>
    public void DoSomething() { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_returns_and_param_documentation()
        {
            const string OriginalCode = @"
public interface ITestMe
{
    int DoSomething(int i);
}

/// <summary>
/// Bla
/// </summary>
class TestMe : ITestMe
{
    /// <summary>
    /// <see cref='ITestMe.DoSomething'/>
    /// </summary>
    /// <param name=""i"">Some comment</param>
    /// <returns>Some result</returns>
    public int DoSomething(int i) => i;
}";

            const string FixedCode = @"
public interface ITestMe
{
    int DoSomething(int i);
}

/// <summary>
/// Bla
/// </summary>
class TestMe : ITestMe
{
    /// <inheritdoc/>
    public int DoSomething(int i) => i;
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2020_InheritdocSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2020_InheritdocSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2020_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreatePhrases(string marker)
        {
            var phrases = new[]
                              {
                                  "<see cref='" + marker + "'/>",
                                  "<see cref='" + marker + "' />",
                                  "<seealso cref='" + marker + "'/>",
                                  "<seealso cref='" + marker + "' />",
                              };

            var results = new List<string>(phrases);
            results.AddRange(phrases.Select(_ => _ + "."));
            results.AddRange(phrases.Select(_ => "see " + _ + "."));
            results.AddRange(phrases.Select(_ => "See " + _ + "."));
            results.AddRange(phrases.Select(_ => "see " + _));
            results.AddRange(phrases.Select(_ => "See " + _));
            results.AddRange(phrases.Select(_ => "seealso " + _ + "."));
            results.AddRange(phrases.Select(_ => "SeeAlso " + _ + "."));
            results.AddRange(phrases.Select(_ => "seealso " + _));
            results.AddRange(phrases.Select(_ => "SeeAlso " + _));

            results.Sort();

            return results.Distinct().ToArray();
        }
    }
}