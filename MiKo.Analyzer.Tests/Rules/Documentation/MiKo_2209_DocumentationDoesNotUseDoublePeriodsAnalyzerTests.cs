using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };

        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_single_period_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">Some text.</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_single_period_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">
    /// Some text.
    /// Some more text.
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_triple_period_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">
    /// Some text...
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_triple_period_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">
    /// Some text...
    /// ... and even more text.
    /// </" + tag + @">
    public void DoSomething()
    {
        
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_relative_file_path_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">
    /// Some \..\..\path
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_relative_file_path_([ValueSource(nameof(XmlTags))] string tag) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    /// <" + tag + @">
    /// Some \..\..\path
    /// Some /../../other/path
    /// </" + tag + @">
    public void DoSomething()
    {
        
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_double_period_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"

public class TestMe
{
    /// <" + tag + @">Some text..</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multi_line_comment_with_double_period_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"

public class TestMe
{
    /// <" + tag + @">
    /// Some text..
    /// Some more text.
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer();
    }
}