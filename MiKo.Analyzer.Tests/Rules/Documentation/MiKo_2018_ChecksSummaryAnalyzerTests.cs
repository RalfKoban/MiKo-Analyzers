using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2018_ChecksSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AmbiguousSynchronousPhrases = ["Check if ", "Checks if ", "Test if ", "Tests if ", "Check that ", "Checks that ", "Test that ", "Tests that "];

        private static readonly string[] AmbiguousPhrases = CreateAmbiguousPhrases(AmbiguousSynchronousPhrases);

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
        public void No_issue_is_reported_for_method_that_starts_documentation_with_see_XML() => No_issue_is_reported_for(@"
/// <summary>
/// <see cref=""TestMe""/> documentation.
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
        public void An_issue_is_reported_for_class_with_ambiguous_phrase_([ValueSource(nameof(AmbiguousPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_class_with_ambiguous_phrase_in_para_tag_([ValueSource(nameof(AmbiguousPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_method_with_ambiguous_phrase_([ValueSource(nameof(AmbiguousPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_method_with_ambiguous_phrase_in_para_tag_([ValueSource(nameof(AmbiguousPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_phrase_([ValueSource(nameof(AmbiguousSynchronousPhrases))] string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_phrase_with_leading_dot_after_XML_tag_([ValueSource(nameof(AmbiguousSynchronousPhrases))] string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>.
    /// " + phrase + @"it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>.
    /// Determines whether it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Asynchronously_phrase_([ValueSource(nameof(AmbiguousSynchronousPhrases))] string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously " + phrase.ToLowerCaseAt(0) + @"it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Asynchronously determines whether it is there.
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_phrase_with_spaces_on_same_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary> </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_phrase_without_contents_on_same_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary></summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_phrase_without_contents_on_different_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_phrase_without_leading_space()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    ///
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_phrase_with_leading_space()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// 
    /// </summary>
    public bool IsSomething() => true;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Determines whether
    /// </summary>
    public bool IsSomething() => true;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2018_ChecksSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2018_ChecksSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2018_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateAmbiguousPhrases(string[] phrases)
        {
            var results = new HashSet<string>(phrases);
            results.AddRange(phrases.Select(_ => _.ToUpper(CultureInfo.CurrentCulture)));
            results.AddRange(phrases.Select(_ => "Asynchronously " + _.ToLowerCaseAt(0)));
            results.AddRange(phrases.Select(_ => "Asynchronously " + _.ToUpperCaseAt(0)));

            return [.. results];
        }
    }
}