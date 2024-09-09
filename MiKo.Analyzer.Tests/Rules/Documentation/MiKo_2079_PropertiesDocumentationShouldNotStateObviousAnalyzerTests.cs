using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ObviousStartingPhrases =
                                                                  [
                                                                      "Get",
                                                                      "Set",
                                                                      "Get/Set",
                                                                      "Get or Set",
                                                                      "Get Or Set",
                                                                      "Get OR Set",
                                                                      "Get and Set",
                                                                      "Get And Set",
                                                                      "Get AND Set",
                                                                      "Gets",
                                                                      "Sets",
                                                                      "Gets/Sets",
                                                                      "Gets or Sets",
                                                                      "Gets Or Sets",
                                                                      "Gets OR Sets",
                                                                      "Gets and Sets",
                                                                      "Gets And Sets",
                                                                      "Gets AND Sets",
                                                                  ];

        [Test]
        public void No_issue_is_reported_for_undocumented_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_documented_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_obvious_documentation_([ValueSource(nameof(ObviousStartingPhrases))] string obvious, [Values("", ".")] string ending) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + obvious + " Age" + ending + @"
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_obvious_documentation_and_empty_line_([ValueSource(nameof(ObviousStartingPhrases))] string obvious, [Values("", ".")] string ending) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + obvious + " Age" + ending + @"
    /// </summary>

    public int Age { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_for_property_with_obvious_documentation_([ValueSource(nameof(ObviousStartingPhrases))] string obvious, [Values("", ".")] string ending)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// " + obvious + " Age" + ending + @"
    /// </summary>
    public int Age { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Age { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_obvious_documentation_and_empty_line_([ValueSource(nameof(ObviousStartingPhrases))] string obvious, [Values("", ".")] string ending)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// " + obvious + " Age" + ending + @"
    /// </summary>

    public int Age { get; set; }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public int Age { get; set; }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2079_PropertiesDocumentationShouldNotStateObviousAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2079_CodeFixProvider();
    }
}