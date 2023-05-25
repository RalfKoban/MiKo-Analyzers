using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Tags =
                                                {
                                                    Constants.XmlTag.Example,
                                                    Constants.XmlTag.Exception,
                                                    Constants.XmlTag.List,
                                                    Constants.XmlTag.Note,
                                                    Constants.XmlTag.Overloads,
                                                    Constants.XmlTag.Para,
                                                    Constants.XmlTag.Param,
                                                    Constants.XmlTag.Remarks,
                                                    Constants.XmlTag.Returns,
                                                    Constants.XmlTag.Summary,
                                                    Constants.XmlTag.TypeParam,
                                                    Constants.XmlTag.Value,
                                                };

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_documented_method_when_start_and_end_tag_are_on_separate_lines_([ValueSource(nameof(Tags))] string tag) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_documented_method_when_start_and_end_tag_are_on_separate_lines_and_there_is_an_empty_line_in_between_([ValueSource(nameof(Tags))] string tag) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// 
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_([ValueSource(nameof(Tags))] string tag) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// Does something.
    /// </" + tag + @">
    /// <returns>
    /// Some data.
    /// </returns>
    public object DoSomething()
    {
        return null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_with_para_or_para() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <returns>
    /// <see langword=""null""/>
    /// <para>-or-</para>
    /// Some null value.
    /// </returns>
    public object DoSomething()
    {
        return null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_documented_method_when_start_and_end_tag_are_on_same_lines_([ValueSource(nameof(Tags))] string tag, [Values("", " ", "  ")] string gap) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + "> " + gap + "</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_text_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + ">Does something.</" + tag + @">
    public void DoSomething()
    {
    }
}
",
2);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_see_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + "><see cref=\"TestMe/></" + tag + @">
    public void DoSomething()
    {
    }
}
",
2);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_see_is_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + @"><see cref=""TestMe/>
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
",
1);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_see_is_on_same_line_as_end_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// <see cref=""TestMe/></" + tag + @">
    public void DoSomething()
    {
    }
}
",
1);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_text_is_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + @">Does something.
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
",
1);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_text_is_on_same_line_as_end_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(
@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// Does something.</" + tag + @">
    public void DoSomething()
    {
    }
}
",
1);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_some_inner_XML_elements_are_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @"><para>
    /// Does something.
    /// </para>
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_some_inner_XML_elements_are_on_same_line_as_end_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// <para>
    /// Does something.
    /// </para></" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_some_XML_elements_are_on_same_line_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// Does something.
    /// </" + tag + @"><returns>
    /// Some data.
    /// </returns>
    public object DoSomething()
    {
        return null;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer();
    }
}