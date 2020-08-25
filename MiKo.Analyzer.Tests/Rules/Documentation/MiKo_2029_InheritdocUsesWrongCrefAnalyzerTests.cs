using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2029_InheritdocUsesWrongCrefAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void No_issue_is_reported_for_undocumented_named_type_(string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("enum")]
        public void An_issue_is_reported_for_wrong_inherited_XML_of_named_type_(string type) => An_issue_is_reported_for(@"
namespace Bla
{
    /// <inheritdoc cref='TestMe' />
    public " + type + @" TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc cref='DoSomething' />
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_XML_summary_of_property() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc cref='DoSomething' />
        public int DoSomething { get; set; }
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalText = @"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc cref='DoSomething' />
        public int DoSomething { get; set; }
    }
}
";

            const string FixedText = @"
namespace Bla
{
    public class TestMe
    {
        /// <inheritdoc/>
        public int DoSomething { get; set; }
    }
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2029_InheritdocUsesWrongCrefAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2029_InheritdocUsesWrongCrefAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2029_CodeFixProvider();
    }
}