using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1538_EventNamePrefixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("class", "MyBlaBla")]
        [TestCase("class", "Off")]
        [TestCase("class", "On")]
        [TestCase("class", "OnlineBlaBla")]
        [TestCase("interface", "MyBlaBla")]
        [TestCase("interface", "Off")]
        [TestCase("interface", "On")]
        [TestCase("interface", "OnlineBlaBla")]
        public void No_issue_is_reported_for_correctly_named_event_(string type, string eventName) => No_issue_is_reported_for(@"
using System;

public " + type + @" TestMe
{
    public event EventHandler " + eventName + @";
}
");

        [TestCase("class", "OnStuff")]
        [TestCase("interface", "OnStuff")]
        public void An_issue_is_reported_for_incorrectly_named_event_(string type, string eventName) => An_issue_is_reported_for(@"
using System;

public " + type + @" TestMe
{
    public event EventHandler " + eventName + @";
}
");

        [TestCase("class", "OnStuff", "Stuff")]
        [TestCase("interface", "OnStuff", "Stuff")]
        public void Code_gets_fixed_(string type, string originalEventName, string fixedEventName)
        {
            const string Template = @"
using System;

public #1# TestMe
{
    public event EventHandler #2#;
}";

            VerifyCSharpFix(Template.Replace("#1#", type).Replace("#2#", originalEventName), Template.Replace("#1#", type).Replace("#2#", fixedEventName));
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

            VerifyCSharpFix(Template.Replace("#", "OnStuff"), Template.Replace("#", "Stuff"));
        }

        protected override string GetDiagnosticId() => MiKo_1538_EventNamePrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1538_EventNamePrefixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1538_CodeFixProvider();
    }
}