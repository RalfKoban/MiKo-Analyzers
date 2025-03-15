using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ThirdPersonVerbs =
                                                            [
                                                                "Accesses",
                                                                "Allows",
                                                                "Breaks",
                                                                "Contains",
                                                                "Converts",
                                                                "Describes",
                                                                "Gets",
                                                                "Occurs",
                                                                "Performs",
                                                                "Stops",
                                                                "Tells",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Age { get; set; }

    public void DoSomething() { }

    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_documented_exception_class() => No_issue_is_reported_for(@"
/// <summary>
/// The exception to be thrown.
/// </summary>
public class TestMeException : System.Exception
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_class() => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Performs some test data.
        /// </summary>
        public virtual void DoSomething() { }

        /// <summary>
        /// Allows to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Breaks to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Asynchronously stops some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }

    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe2
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <inheritdoc />
        public override void DoSomething() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_passive_form_([ValueSource(nameof(ThirdPersonVerbs))] string verb) => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// " + verb + @" some test data.
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_para_tag_in_passive_form_([ValueSource(nameof(ThirdPersonVerbs))] string verb) => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// <para>
    /// " + verb + @" some test data.
    /// </para>
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_starting_verb_in_passive_form_followed_by_comma() => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// Represents, provides or includes some test data.
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class_([Values("Provide", "This are")] string start) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + start + @" some test data.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class_in_para_tag_([Values("Provide", "This are")] string start) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// <para>
/// " + start + @" some test data.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("Perform", "Miss", "Mixs", "Buzzs", "Enrichs", "This are")] string verb) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + verb + @" some test data.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Asynchronously perform some test data.
    /// </summary>
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Get or sets some test data.
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_starting_with_term_([Values("Recursively")] string term) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + term + @" loops over some test data.
    /// </summary>
    public void Loop() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_private_class_with_see_cref_as_second_word() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Converts <see cref=""string""/> into something else.
    /// </summary>
    private class Converter
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_private_class_with_see_cref_as_second_word() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Convert <see cref=""string""/> into something else.
    /// </summary>
    private class Converter
    { }
}
");

        [TestCase("Class that allows to update something", "Updates something")]
        [TestCase("Class that can be used to update something", "Updates something")]
        [TestCase("Class that could be used to update something", "Updates something")]
        [TestCase("Class that may be used to update something", "Updates something")]
        [TestCase("Class that shall be used to update something", "Updates something")]
        [TestCase("Class that should be used to update something", "Updates something")]
        [TestCase("Class that will be used to update something", "Updates something")]
        [TestCase("Class that would be used to update something", "Updates something")]
        [TestCase("Class which can be used to update something", "Updates something")]
        [TestCase("Class which could be used to update something", "Updates something")]
        [TestCase("Class which may be used to update something", "Updates something")]
        [TestCase("Class which shall be used to update something", "Updates something")]
        [TestCase("Class which should be used to update something", "Updates something")]
        [TestCase("Class which will be used to update something", "Updates something")]
        [TestCase("Class which would be used to update something", "Updates something")]
        [TestCase("Class which allows to update something", "Updates something")]
        [TestCase("A class that allows to update something", "Updates something")]
        [TestCase("A class that can be used to update something", "Updates something")]
        [TestCase("A class that could be used to update something", "Updates something")]
        [TestCase("A class that may be used to update something", "Updates something")]
        [TestCase("A class that shall be used to update something", "Updates something")]
        [TestCase("A class that should be used to update something", "Updates something")]
        [TestCase("A class that will be used to update something", "Updates something")]
        [TestCase("A class that would be used to update something", "Updates something")]
        [TestCase("A class which can be used to update something", "Updates something")]
        [TestCase("A class which could be used to update something", "Updates something")]
        [TestCase("A class which may be used to update something", "Updates something")]
        [TestCase("A class which shall be used to update something", "Updates something")]
        [TestCase("A class which should be used to update something", "Updates something")]
        [TestCase("A class which will be used to update something", "Updates something")]
        [TestCase("A class which would be used to update something", "Updates something")]
        [TestCase("A class which allows to update something", "Updates something")]
        [TestCase("An class that allows to update something", "Updates something")]
        [TestCase("An class that can be used to update something", "Updates something")]
        [TestCase("An class that could be used to update something", "Updates something")]
        [TestCase("An class that may be used to update something", "Updates something")]
        [TestCase("An class that shall be used to update something", "Updates something")]
        [TestCase("An class that should be used to update something", "Updates something")]
        [TestCase("An class that will be used to update something", "Updates something")]
        [TestCase("An class that would be used to update something", "Updates something")]
        [TestCase("An class which can be used to update something", "Updates something")]
        [TestCase("An class which could be used to update something", "Updates something")]
        [TestCase("An class which may be used to update something", "Updates something")]
        [TestCase("An class which shall be used to update something", "Updates something")]
        [TestCase("An class which should be used to update something", "Updates something")]
        [TestCase("An class which will be used to update something", "Updates something")]
        [TestCase("An class which would be used to update something", "Updates something")]
        [TestCase("An class which allows to update something", "Updates something")]
        [TestCase("The class that allows to update something", "Updates something")]
        [TestCase("The class that can be used to update something", "Updates something")]
        [TestCase("The class that could be used to update something", "Updates something")]
        [TestCase("The class that may be used to update something", "Updates something")]
        [TestCase("The class that shall be used to update something", "Updates something")]
        [TestCase("The class that should be used to update something", "Updates something")]
        [TestCase("The class that will be used to update something", "Updates something")]
        [TestCase("The class that would be used to update something", "Updates something")]
        [TestCase("The class which can be used to update something", "Updates something")]
        [TestCase("The class which could be used to update something", "Updates something")]
        [TestCase("The class which may be used to update something", "Updates something")]
        [TestCase("The class which shall be used to update something", "Updates something")]
        [TestCase("The class which should be used to update something", "Updates something")]
        [TestCase("The class which will be used to update something", "Updates something")]
        [TestCase("The class which would be used to update something", "Updates something")]
        [TestCase("The class which allows to update something", "Updates something")]
        [TestCase("This class allows to update something", "Updates something")]
        public void Code_gets_fixed_for_class_text_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

/// <summary>
/// ###
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [TestCase("Interface that allows to update something", "Updates something")]
        [TestCase("Interface that can be used to update something", "Updates something")]
        [TestCase("Interface that could be used to update something", "Updates something")]
        [TestCase("Interface that may be used to update something", "Updates something")]
        [TestCase("Interface that shall be used to update something", "Updates something")]
        [TestCase("Interface that should be used to update something", "Updates something")]
        [TestCase("Interface that will be used to update something", "Updates something")]
        [TestCase("Interface that would be used to update something", "Updates something")]
        [TestCase("Interface which can be used to update something", "Updates something")]
        [TestCase("Interface which could be used to update something", "Updates something")]
        [TestCase("Interface which may be used to update something", "Updates something")]
        [TestCase("Interface which shall be used to update something", "Updates something")]
        [TestCase("Interface which should be used to update something", "Updates something")]
        [TestCase("Interface which will be used to update something", "Updates something")]
        [TestCase("Interface which would be used to update something", "Updates something")]
        [TestCase("Interface which allows to update something", "Updates something")]
        [TestCase("A interface that allows to update something", "Updates something")]
        [TestCase("A interface that can be used to update something", "Updates something")]
        [TestCase("A interface that could be used to update something", "Updates something")]
        [TestCase("A interface that may be used to update something", "Updates something")]
        [TestCase("A interface that shall be used to update something", "Updates something")]
        [TestCase("A interface that should be used to update something", "Updates something")]
        [TestCase("A interface that will be used to update something", "Updates something")]
        [TestCase("A interface that would be used to update something", "Updates something")]
        [TestCase("A interface which can be used to update something", "Updates something")]
        [TestCase("A interface which could be used to update something", "Updates something")]
        [TestCase("A interface which may be used to update something", "Updates something")]
        [TestCase("A interface which shall be used to update something", "Updates something")]
        [TestCase("A interface which should be used to update something", "Updates something")]
        [TestCase("A interface which will be used to update something", "Updates something")]
        [TestCase("A interface which would be used to update something", "Updates something")]
        [TestCase("A interface which allows to update something", "Updates something")]
        [TestCase("An interface that allows to update something", "Updates something")]
        [TestCase("An interface that can be used to update something", "Updates something")]
        [TestCase("An interface that could be used to update something", "Updates something")]
        [TestCase("An interface that may be used to update something", "Updates something")]
        [TestCase("An interface that shall be used to update something", "Updates something")]
        [TestCase("An interface that should be used to update something", "Updates something")]
        [TestCase("An interface that will be used to update something", "Updates something")]
        [TestCase("An interface that would be used to update something", "Updates something")]
        [TestCase("An interface which can be used to update something", "Updates something")]
        [TestCase("An interface which could be used to update something", "Updates something")]
        [TestCase("An interface which may be used to update something", "Updates something")]
        [TestCase("An interface which shall be used to update something", "Updates something")]
        [TestCase("An interface which should be used to update something", "Updates something")]
        [TestCase("An interface which will be used to update something", "Updates something")]
        [TestCase("An interface which would be used to update something", "Updates something")]
        [TestCase("An interface which allows to update something", "Updates something")]
        [TestCase("The interface that allows to update something", "Updates something")]
        [TestCase("The interface that can be used to update something", "Updates something")]
        [TestCase("The interface that could be used to update something", "Updates something")]
        [TestCase("The interface that may be used to update something", "Updates something")]
        [TestCase("The interface that shall be used to update something", "Updates something")]
        [TestCase("The interface that should be used to update something", "Updates something")]
        [TestCase("The interface that will be used to update something", "Updates something")]
        [TestCase("The interface that would be used to update something", "Updates something")]
        [TestCase("The interface which can be used to update something", "Updates something")]
        [TestCase("The interface which could be used to update something", "Updates something")]
        [TestCase("The interface which may be used to update something", "Updates something")]
        [TestCase("The interface which shall be used to update something", "Updates something")]
        [TestCase("The interface which should be used to update something", "Updates something")]
        [TestCase("The interface which will be used to update something", "Updates something")]
        [TestCase("The interface which would be used to update something", "Updates something")]
        [TestCase("The interface which allows to update something", "Updates something")]
        [TestCase("This interface allows to update something", "Updates something")]
        public void Code_gets_fixed_for_interface_text_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

/// <summary>
/// ###
/// </summary>
public interface TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [TestCase("Repository that does something", "Represents a repository that does something")]
        public void Code_gets_fixed_for_special_class_text_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

/// <summary>
/// ###
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [TestCase("Called to inform about something", "Informs about something")]
        [TestCase("Called to save something", "Saves something")]
        public void Code_gets_fixed_for_method_text_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

public interface TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        protected override string GetDiagnosticId() => MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2019_CodeFixProvider();
    }
}