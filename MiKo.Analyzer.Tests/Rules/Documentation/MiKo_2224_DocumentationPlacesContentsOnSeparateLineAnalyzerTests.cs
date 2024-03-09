using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_correctly_documented_method_with_empty_para_tag() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_documented_method_when_start_and_end_tag_are_on_same_lines_([ValueSource(nameof(Tags))] string tag, [Values("", " ", "  ")] string gap) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">" + gap + "</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_text_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <" + tag + ">Does something.</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_with_see_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <" + tag + "><see cref=\"TestMe\"/></" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_see_is_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @"><see cref=""TestMe""/>
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_see_is_on_same_line_as_end_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// <see cref=""TestMe""/></" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_text_is_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">Does something.
    /// </" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_text_is_on_same_line_as_end_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">
    /// Does something.</" + tag + @">
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_some_inner_XML_elements_are_on_same_line_as_start_tag_([ValueSource(nameof(Tags))] string tag) => An_issue_is_reported_for(2, @"
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

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_text_after_it() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/> This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_XML_element_after_it() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/></summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_text_before_it() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text. <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_element_before_it() => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <summary><para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_another_empty_element() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/><para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_with_text()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_text_is_on_same_line_as_start_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_text_is_on_same_line_as_end_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.</summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_with_see()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary><see cref=""TestMe"" /></summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe"" />
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_see_is_on_same_line_as_start_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary><see cref=""TestMe""/>
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/>
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_see_is_on_same_line_as_end_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/></summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/>
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_same_line_with_text_after()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_line_with_text_before()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.<para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_line_with_text_before_and_after()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.<para/>This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_line_as_start_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary><para/>
    /// This is some text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <para/>
    /// This is some text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_line_as_end_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/></summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_if_empty_para_tag_is_on_line_as_another_empty_tag()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/><para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text.
    /// <para/>
    /// <para/>
    /// This is some other text.
    /// </summary>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2224_CodeFixProvider();
    }
}