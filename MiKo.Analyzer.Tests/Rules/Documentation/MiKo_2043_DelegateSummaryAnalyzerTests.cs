using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2043_DelegateSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_documented_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_delegate() => No_issue_is_reported_for(@"
using System;

public delegate void TestMe();

");

        [Test]
        public void No_issue_is_reported_for_delegate_with_standard_summary_phrase() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Encapsulates a method that does something.
/// </summary>
public delegate void TestMe();

");

        [Test]
        public void An_issue_is_reported_for_delegate_with_non_standard_summary_phrase() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// My delegate.
/// </summary>
public delegate void TestMe();

");

        [Test]
        public void An_issue_is_reported_for_delegate_starting_with_see_cref() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// <see cref=""TestMe""/>
/// </summary>
public delegate void TestMe();

");

        [TestCase("An application-defined callback function used with the", "Encapsulates a method that handles callbacks for")]
        [TestCase("Application-defined callback function used with the", "Encapsulates a method that handles callbacks for")]
        [TestCase("Builder method that wraps", "Encapsulates a method that wraps")]
        [TestCase("Builder method which wraps", "Encapsulates a method that wraps")]
        [TestCase("Delegate for the callback by which the application gives feedback about", "Encapsulates a method that provides feedback about")]
        [TestCase("Delegate for the callback by which the application informs", "Encapsulates a method that notifies")]
        [TestCase("Delegate for the callback by which the application tells", "Encapsulates a method that notifies")]
        [TestCase("Delegate for", "Encapsulates a method that handles callbacks for")]
        [TestCase("Delegate in which the application closes", "Encapsulates a method that closes")]
        [TestCase("Delegate in which the application opens", "Encapsulates a method that opens")]
        [TestCase("Delegate in which the application writes", "Encapsulates a method that writes")]
        [TestCase("Delegate to handle", "Encapsulates a method that handles")]
        [TestCase("Delegate used for handling", "Encapsulates a method that handles")]
        [TestCase("Delegate used for filtering", "Encapsulates a method that filters")]
        [TestCase("Delegate used for notifying", "Encapsulates a method that notifies")]
        [TestCase("Delegate used for validation", "Encapsulates a method that validates")]
        [TestCase("""Delegate used for <see cref="String"/>""", """Encapsulates a method that <see cref="String"/>""")]
        [TestCase("Delegate used to handle", "Encapsulates a method that handles")]
        [TestCase("Describes a function that is called when", "Encapsulates a method that is called when")]
        [TestCase("Describes a function which is called when", "Encapsulates a method that is called when")]
        [TestCase("Does", "Encapsulates a method that does")]
        [TestCase("Event fired by the", "Encapsulates a method that is called by the")]
        [TestCase("Event raised by the", "Encapsulates a method that is called by the")]
        [TestCase("Event triggered by the", "Encapsulates a method that is called by the")]
        [TestCase("Event Handler for", "Encapsulates a method that handles")]
        [TestCase("Eventhandler for", "Encapsulates a method that handles")]
        [TestCase("Event-handler for", "Encapsulates a method that handles")]
        [TestCase("EventHandler for", "Encapsulates a method that handles")]
        [TestCase("Event-Handler for", "Encapsulates a method that handles")]
        [TestCase("Callback to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Callbacks to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Callback to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("Callbacks to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("Call-back to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Call-backs to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Call-back to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("Call-backs to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("Call-Back to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Call-Backs to notify about", "Encapsulates a method that notifies about")]
        [TestCase("Call-Back to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("Call-Backs to notfy about", "Encapsulates a method that notifies about")] // typo
        [TestCase("References a method to be called to notify", "Encapsulates a method that notifies")]
        [TestCase("References the method to be called to notify", "Encapsulates a method that notifies")]
        [TestCase("References a method to be called to notfy", "Encapsulates a method that notifies")]
        [TestCase("References the method to be called to notfy", "Encapsulates a method that notifies")]
        [TestCase("References a method to be invoked to notify", "Encapsulates a method that notifies")]
        [TestCase("References the method to be invoked to notify", "Encapsulates a method that notifies")]
        [TestCase("References a method to be invoked to notfy", "Encapsulates a method that notifies")]
        [TestCase("References the method to be invoked to notfy", "Encapsulates a method that notifies")]
        [TestCase("References a method to call to notify", "Encapsulates a method that notifies")]
        [TestCase("References the method to call to notify", "Encapsulates a method that notifies")]
        [TestCase("References a method to call to notfy", "Encapsulates a method that notifies")]
        [TestCase("References the method to call to notfy", "Encapsulates a method that notifies")]
        [TestCase("References a method to invoke to notify", "Encapsulates a method that notifies")]
        [TestCase("References the method to invoke to notify", "Encapsulates a method that notifies")]
        [TestCase("References a method to invoke to notfy", "Encapsulates a method that notifies")]
        [TestCase("References the method to invoke to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to be called to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to be called to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to be called to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to be called to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to be invoked to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to be invoked to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to be invoked to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to be invoked to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to call to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to call to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to call to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to call to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to invoke to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to invoke to notify", "Encapsulates a method that notifies")]
        [TestCase("Represents a method to invoke to notfy", "Encapsulates a method that notifies")]
        [TestCase("Represents the method to invoke to notfy", "Encapsulates a method that notifies")]
        [TestCase("""Represents the <see cref="ToString"/> method""", """Encapsulates a method that handles a call to the <see cref="ToString"/> method""")]
        [TestCase("""Represents a method that handles the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("""Represents a method which handles the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("""Represents a method handling the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("""Represents the method that handles the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("""Represents the method which handles the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("""Represents the method handling the <see cref="Whatever"/> event""", """Encapsulates a method that handles the <see cref="Whatever"/> event""")]
        [TestCase("Represents a method that handles one of the following events:", "Encapsulates a method that handles one of the following events:")]
        [TestCase("Represents the getter of", "Encapsulates a method that gets the")]
        [TestCase("Represents the getter of the", "Encapsulates a method that gets the")]
        [TestCase("Represents the setter of", "Encapsulates a method that sets the")]
        [TestCase("Represents the setter of the", "Encapsulates a method that sets the")]
        public void Code_gets_fixed_for_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

/// <summary>
/// ### something.
/// </summary>
public delegate void TestMe();
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [Test]
        public void Code_gets_fixed_when_summary_only_contains_delegate_name()
        {
            const string OriginalCode = @"
using System;

/// <summary>
/// TestMe delegate
/// </summary>
public delegate void TestMe();
";

            const string FixedCode = @"
using System;

/// <summary>
/// Encapsulates a method that TODO
/// </summary>
public delegate void TestMe();
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2043_DelegateSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2043_DelegateSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2043_CodeFixProvider();
    }
}