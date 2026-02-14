using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2045_InvalidParameterReferenceInSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_without_parameter_references_in_summary() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The parameter</param>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_tag_in_summary_([Values("param", "paramref")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @" name=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void A_single_issue_is_reported_for_param_tag_in_summary() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <param name=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void Code_gets_fixed_by_replacing_self_closing_tag_with_parameter_name_([Values("param", "paramref")] string tag)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with ###.
    /// </summary>
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(Template.Replace("###", $"<{tag} name=\"i\" />"), Template.Replace("###", "i"));
        }

        [Test]
        public void Code_gets_fixed_by_replacing_tag_pair_with_parameter_name_([Values("param", "paramref")] string tag)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with ###.
    /// </summary>
    public void DoSomething(int i) { }
}
";

            VerifyCSharpFix(Template.Replace("###", $"<{tag} name=\"i\"></{tag}>"), Template.Replace("###", "i"));
        }

        protected override string GetDiagnosticId() => MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2045_InvalidParameterReferenceInSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2045_CodeFixProvider();
    }
}