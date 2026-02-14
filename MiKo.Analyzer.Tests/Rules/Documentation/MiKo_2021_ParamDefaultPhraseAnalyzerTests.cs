using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2021_ParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [TestCase("bool")]
        [TestCase("System.Boolean")]
        [TestCase("System.StringComparison")]
        [TestCase(nameof(Boolean))]
        [TestCase(nameof(StringComparison))]
        public void No_issue_is_reported_for_method_with_parameter_of_type_(string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(" + type + @" o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_out_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(out object o) { }
}
");

        [TestCase("A whatever.")]
        [TestCase("An whatever.")]
        [TestCase("The whatever.")]
        [TestCase("a whatever")]
        [TestCase("an whatever")]
        [TestCase("the whatever")]
        [TestCase("unused")]
        [TestCase("Unused")]
        [TestCase("Unused.")]
        public void No_issue_is_reported_for_parameter_starting_with_article_or_unused_(string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_with_article_followed_by_line_break_and_see_tag_([Values("A", "An", "The")] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + comment + @"
    /// <see cref=""object"">
    /// something
    /// </see>
    /// to do.
    /// </param>
    public void DoSomething(object o) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_parameter_documentation_(string xmlElement) => No_issue_is_reported_for(@"
public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(object o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        public void An_issue_is_reported_for_parameter_not_starting_with_article_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void Code_gets_fixed_for_parameter()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// Stuff with some data.
    /// </param>
    public void DoSomething(object o) { }
}";

            const string FixedCode = @"
public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// The stuff with some data.
    /// </param>
    public void DoSomething(object o) { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Determines the", "The")]
        [TestCase("Determines to which extend", "The value to which extend")]
        [TestCase("Determines to what extend", "The value to what extend")]
        [TestCase("Either a item or", "The item or")]
        [TestCase("Either an item or", "The item or")]
        [TestCase("Either the item or", "The item or")]
        [TestCase("Reference to the", "The")]
        [TestCase("Reference to a", "The")]
        [TestCase("Reference to an", "The")]
        public void Code_gets_fixed_by_replacing_common_parameter_documentation_phrases_(string originalStart, string fixedStart)
        {
            const string Template = @"
public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// ### stuff.
    /// </param>
    public void DoSomething(object o) { }
}";

            VerifyCSharpFix(Template.Replace("###", originalStart), Template.Replace("###", fixedStart));
        }

        protected override string GetDiagnosticId() => MiKo_2021_ParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2021_ParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2021_CodeFixProvider();
    }
}