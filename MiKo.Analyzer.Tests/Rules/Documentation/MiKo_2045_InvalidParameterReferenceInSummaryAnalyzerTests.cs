using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void No_issue_is_reported_for_correctly_documented_method_without_parameters() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("param", "paramref")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @" name=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void A_single_issue_is_reported_for_incorrectly_documented_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <param name=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
", 1);

        [Test]
        public void Code_gets_fixed_for_incorrectly_documented_method_with_empty_([Values("param", "paramref")] string tag)
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
        public void Code_gets_fixed_for_incorrectly_documented_method_with_([Values("param", "paramref")] string tag)
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