using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1004_EventNameSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyBlaBla;
}
");

        [TestCase("interface", "MyEvent")]
        [TestCase("class", "MyEvent")]
        public void An_issue_is_reported_for_incorrectly_named_event_(string type, string eventName) => An_issue_is_reported_for(@"
using System;

public " + type + @" TestMe
{
    public event EventHandler " + eventName + @";
}
");

        [TestCase("interface", "MyEvent", "My")]
        [TestCase("class", "MyEvent", "My")]
        public void Code_gets_fixed_(string type, string originalEventName, string fixedEventName)
        {
            const string Template = @"
using System;

public #1 TestMe
{
    public event EventHandler #2;
}";

            var originalCode = Template.Replace("#1", type).Replace("#2", originalEventName);
            var fixedCode = Template.Replace("#1", type).Replace("#2", fixedEventName);

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_with_EventDeclaration_gets_fixed()
        {
            const string Template = @"
using System;

public class TestMe
{
    public event EventHandler #
    {
        add { }
        remove { }
    }
}";

            var originalCode = Template.Replace("#", "MyEvent");
            var fixedCode = Template.Replace("#", "My");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1004_EventNameSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1004_EventNameSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1004_CodeFixProvider();
    }
}