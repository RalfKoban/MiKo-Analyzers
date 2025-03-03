﻿using System.Collections.Generic;
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
    public sealed class MiKo_2041_InvalidXmlInSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AmbiguousPhrases = CreateAmbiguousPhrases();

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
        public void No_issue_is_reported_for_class_with_documentation_with_typeparamref_tag() => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation for <typeparamref name=""T"" />.
/// </summary>
public class TestMe<T>
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
        public void Code_gets_fixed_for_([ValueSource(nameof(AmbiguousPhrases))] string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + phrase + @" whatever.
    /// </summary>
    public void DoSomething() { }
}
";

            var fixedCode = @"
public class TestMe
{
    /// <summary>
    ///  whatever.
    /// </summary>
    /// " + phrase + @"
    public void DoSomething() { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_summary_with_([ValueSource(nameof(AmbiguousPhrases))] string phrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// " + phrase + @"
    /// </summary>
    public void DoSomething() { }
}
";

            var fixedCode = @"
public class TestMe
{
    /// " + phrase + @"
    public void DoSomething() { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2041_InvalidXmlInSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2041_InvalidXmlInSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2041_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateAmbiguousPhrases()
        {
            string[] phrases =
                               [
                                   "<example />",
                                   "<exception />",
                                   "<include />",
                                   "<inheritdoc />",
                                   "<overloads />",
                                   "<param />",
                                   "<paramref />",
                                   "<permission />",
                                   "<remarks />",
                                   "<returns />",
                                   "<seealso />",
                                   "<summary />",
                                   "<typeparam />",
                                   "<value />",
                                   "<seealso>Bla</seealso>",
                               ];

            var results = new HashSet<string>(phrases);
            results.AddRange(phrases.Select(_ => _.Replace(" ", string.Empty)));

            return [.. results];
        }
    }
}