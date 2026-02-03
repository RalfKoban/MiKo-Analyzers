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
                                                                "Triggers",
                                                            ];

        private static readonly string[] Constructors =
                                                        [
                                                            "constructor",
                                                            "Constructor",
                                                            "Ctor",
                                                            "ctor",
                                                            "Copy constructor",
                                                            "Copy Constructor",
                                                            "Copy ctor",
                                                            "Copy Ctor",
                                                            "copy constructor",
                                                            "copy Constructor",
                                                            "copy ctor",
                                                            "copy Ctor",
                                                            "Default constructor",
                                                            "Default Constructor",
                                                            "Default ctor",
                                                            "Default Ctor",
                                                            "default constructor",
                                                            "default Constructor",
                                                            "default ctor",
                                                            "default Ctor",
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
        [TestCase("This class is responsible for collecting something", "Collects something")]
        [TestCase("This interface is responsible for collecting something", "Collects something")]
        [TestCase("Is responsible for collecting something", "Collects something")]
        [TestCase("Responsible for collecting something", "Collects something")]
        [TestCase("Class containing extension methods for something", @"Provides a set of <see langword=""static""/> methods for something")]
        [TestCase("Class for byte value conversation.", "Converts byte value.", Ignore = "Currently unclear how to fix")] // TODO RKN
        [TestCase("Class for checking something", "Determines something")]
        [TestCase("Class for extension methods that extend something", @"Provides a set of <see langword=""static""/> methods for something")]
        [TestCase("Class for extension methods which extend something", @"Provides a set of <see langword=""static""/> methods for something")]
        [TestCase("Class for extensions", @"Provides a set of <see langword=""static""/> methods for ")]
        [TestCase("Class for representing something", "Represents something")]
        [TestCase("Class in charge of getting something", "Gets something")]
        [TestCase("Class part of the something", "Represents the part of the something")]
        [TestCase("Class provides something", "Provides something")]
        [TestCase("Class providing functionality to calculate something", "Calculates something")]
        [TestCase("Class providing functionality to populate something", "Populates something")]
        [TestCase("Class providing something", "Provides something")]
        [TestCase("Class representing something", "Represents something")]
        [TestCase("Class Representing something", "Represents something")] // 'Representing' is written upper-case by intent, as there is a real-world-scenario
        [TestCase("Class retrieving something", "Provides something")]
        [TestCase("Class specialized in creating something", "Creates something")]
        [TestCase("Class that contains something", "Provides something")]
        [TestCase("Class that holds something", "Holds something")]
        [TestCase("Class that is used as helper class for something", "Provides something")]
        [TestCase("Class that offers something", "Provides something")]
        [TestCase("Class to deserialize something", "Deserializes something")]
        [TestCase("Class to help with something", "Helps with something")]
        [TestCase("Class to retrieve something", "Provides something")]
        [TestCase("Class to store something", "Stores something")]
        [TestCase("Class used for loading something", "Loads something")]
        [TestCase("Class which holds something", "Holds something")]
        [TestCase("Classes implementing this interfaces, will be called with their something", "Provides a something")]
        [TestCase("This class reads something", "Reads something")]
        [TestCase("This is an adapter for something", "Adapts something")]
        [TestCase("This is an adapter between something", "Adapts between something")]
        [TestCase("Base for all view models", "Represents view models")]
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

        [TestCase(@"Implementation of ")]
        [TestCase(@"Implementation for ")]
        [TestCase(@"Implementation class of ")]
        [TestCase(@"Implementation class for ")]
        public void Code_gets_fixed_for_implementation_class_text_(string originalText)
        {
            var originalCode = @"
using System;

/// <summary>
/// " + originalText + @" <see cref=""IDisposable""/>
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
using System;

/// <inheritdoc cref=""IDisposable""/>
public class TestMe
{
}
";

            VerifyCSharpFix(originalCode, FixedCode);
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

        [TestCase("Simple structure to do stuff", "Represents a simple structure to do stuff")]
        [TestCase("Complex structure to do stuff", "Represents a complex structure to do stuff")]
        [TestCase("Health check to do stuff", "Represents a health check to do stuff")]
        [TestCase("Additional information to do stuff", "Represents an additional information to do stuff")]
        [TestCase("Addtional information to do stuff", "Represents an addtional information to do stuff")] // typo
        [TestCase("Highest version to do stuff", "Represents the highest version to do stuff")]
        [TestCase("Lowest version to do stuff", "Represents the lowest version to do stuff")]
        [TestCase("High version to do stuff", "Represents the high version to do stuff")]
        [TestCase("Medium version to do stuff", "Represents the medium version to do stuff")]
        [TestCase("Middle version to do stuff", "Represents the middle version to do stuff")]
        [TestCase("Low version to do stuff", "Represents the low version to do stuff")]
        [TestCase("Minimum version to do stuff", "Represents the minimum version to do stuff")]
        [TestCase("Maximum version to do stuff", "Represents the maximum version to do stuff")]
        [TestCase("Min version to do stuff", "Represents the min version to do stuff")]
        [TestCase("Max version to do stuff", "Represents the max version to do stuff")]
        [TestCase("Major version to do stuff", "Represents the major version to do stuff")]
        [TestCase("Minor version to do stuff", "Represents the minor version to do stuff")]
        [TestCase("Optimum version to do stuff", "Represents the optimum version to do stuff")]
        [TestCase("Optimal version to do stuff", "Represents the optimal version to do stuff")]
        [TestCase("Absolute version to do stuff", "Represents the absolute version to do stuff")]
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

        [TestCase("Set to true")]
        [TestCase("Set to true,")]
        [TestCase("Set to true if")]
        [TestCase("Get/Set flag that indicates if")]
        [TestCase("Get/Set flag that indicates whether")]
        [TestCase("Get/Set flag that indicates that")]
        [TestCase("Get/Set flag which indicates if")]
        [TestCase("Get/Set flag which indicates whether")]
        [TestCase("Get/Set flag which indicates that")]
        [TestCase("Get/Set flag indicating if")]
        [TestCase("Get/Set flag indicating whether")]
        [TestCase("Get/Set flag indicating that")]
        [TestCase("Get/Set a flag that indicates if")]
        [TestCase("Get/Set a flag that indicates whether")]
        [TestCase("Get/Set a flag that indicates that")]
        [TestCase("Get/Set a flag which indicates if")]
        [TestCase("Get/Set a flag which indicates whether")]
        [TestCase("Get/Set a flag which indicates that")]
        [TestCase("Get/Set a flag indicating if")]
        [TestCase("Get/Set a flag indicating whether")]
        [TestCase("Get/Set a flag indicating that")]
        [TestCase("Get/Set value that indicates if")]
        [TestCase("Get/Set value that indicates whether")]
        [TestCase("Get/Set value that indicates that")]
        [TestCase("Get/Set value which indicates if")]
        [TestCase("Get/Set value which indicates whether")]
        [TestCase("Get/Set value which indicates that")]
        [TestCase("Get/Set value indicating if")]
        [TestCase("Get/Set value indicating whether")]
        [TestCase("Get/Set value indicating that")]
        [TestCase("Get/Set a value that indicates if")]
        [TestCase("Get/Set a value that indicates whether")]
        [TestCase("Get/Set a value that indicates that")]
        [TestCase("Get/Set a value which indicates if")]
        [TestCase("Get/Set a value which indicates whether")]
        [TestCase("Get/Set a value which indicates that")]
        [TestCase("Get/Set a value indicating if")]
        [TestCase("Get/Set a value indicating whether")]
        [TestCase("Get/Set a value indicating that")]
        [TestCase("Get/Set")]
        [TestCase("Get/set")]
        [TestCase("get/Set")]
        [TestCase("Set/Get")]
        [TestCase("Set/get")]
        [TestCase("set/Get")]
        [TestCase("Describe whether")]
        [TestCase("Specify whether")]
        [TestCase("This will return")]
        [TestCase("This returns")]
        [TestCase("This property will return")]
        [TestCase("This property returns")]
        [TestCase("This Property will return")]
        [TestCase("This Property returns")]
        [TestCase("If set to true then")]
        [TestCase("If set to True then")]
        [TestCase("If set to TRUE then")]
        [TestCase("If set to true, then")]
        [TestCase("If set to True, then")]
        [TestCase("If set to TRUE, then")]
        [TestCase("true if")]
        [TestCase("True if")]
        [TestCase("TRUE if")]
        [TestCase("true when")]
        [TestCase("True when")]
        [TestCase("TRUE when")]
        [TestCase("When set to true then")]
        [TestCase("When set to True then")]
        [TestCase("When set to TRUE then")]
        [TestCase("When set to true, then")]
        [TestCase("When set to True, then")]
        [TestCase("When set to TRUE, then")]
        public void Code_gets_fixed_for_boolean_property_text_(string originalText)
        {
            const string Template = @"
using System;

public interface TestMe
{
    /// <summary>
    /// ### to inform about something
    /// </summary>
    public bool SomeProperty { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", "Gets or sets a value indicating whether"));
        }

        [TestCase("Get/Set")]
        [TestCase("Get/set")]
        [TestCase("get/Set")]
        [TestCase("Set/Get")]
        [TestCase("Set/get")]
        [TestCase("set/Get")]
        [TestCase("This will return")]
        [TestCase("This returns")]
        [TestCase("This property will return")]
        [TestCase("This property returns")]
        [TestCase("This Property will return")]
        [TestCase("This Property returns")]
        [TestCase("get information about")]
        public void Code_gets_fixed_for_non_boolean_property_text_(string originalText)
        {
            const string Template = @"
using System;

public interface TestMe
{
    /// <summary>
    /// ### some text to inform about something
    /// </summary>
    public string SomeProperty { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", "Gets or sets"));
        }

        [TestCase("Called to inform about something", "Informs about something")]
        [TestCase("Called to save something", "Saves something")]
        [TestCase("Save something", "Saves something")]
        [TestCase("Whether something is there", "Determines whether something is there")]
        [TestCase("If something is there", "Determines whether something is there")]
        [TestCase("Asynchronously invoked at some time", "Asynchronously runs at some time")]
        [TestCase("Asynchronously called at some time", "Asynchronously runs at some time")]
        [TestCase("Recursively invoked at some time", "Recursively runs at some time")]
        [TestCase("Recursively called at some time", "Recursively runs at some time")]
        [TestCase("This method returns something important", "Returns something important")]
        [TestCase("This method is responsible for collecting something important", "Collects something important")]
        [TestCase("Is responsible for collecting something", "Collects something")]
        [TestCase("Responsible for collecting something important", "Collects something important")]
        [TestCase("This will initialize something", "Initializes something")]
        [TestCase("This callback cleans stuff", "Cleans stuff")]
        [TestCase("This call-back cleans stuff", "Cleans stuff")]
        [TestCase("This Callback cleans stuff", "Cleans stuff")]
        [TestCase("This Call-back cleans stuff", "Cleans stuff")]
        [TestCase("This callback will clean stuff", "Cleans stuff")]
        [TestCase("This call-back will clean stuff", "Cleans stuff")]
        [TestCase("This Callback will clean stuff", "Cleans stuff")]
        [TestCase("This Call-back will clean stuff", "Cleans stuff")]
        [TestCase("Use this Method, to change something", "Changes something")]
        [TestCase("Use this Method to change something", "Changes something")]
        [TestCase("Use this method to change something", "Changes something")]
        [TestCase("Use this method, to change something", "Changes something")]
        [TestCase("This will start to do something", "Starts to do something")]
        [TestCase("This method will start to do something", "Starts to do something")]
        [TestCase("This starts to do something", "Starts to do something")]
        [TestCase("Trigger something", "Triggers something")]
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

        [TestCase("Convert the given", "Converts the given")]
        public void Code_gets_fixed_for_method_text_with_see_link_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

public interface TestMe
{
    /// <summary>
    /// ### <see cref=""int""/> to a <see cref=""string""/>.
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [TestCase("Given anything, does something")]
        [TestCase("When anything, does something")]
        public void Code_is_not_fixed_for_special_phrase_(string phrase)
        {
            var code = @"
using System;

public interface TestMe
{
    /// <summary>
    /// " + phrase + @".
    /// </summary>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(code, code);
        }

        [Test]
        public void Code_gets_fixed_for_public_constructor_([ValueSource(nameof(Constructors))] string originalText)
        {
            const string Template = """

                                    using System;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// ###.
                                        /// </summary>
                                        public TestMe() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", """Initializes a new instance of the <see cref="TestMe"/> class"""));
        }

        [Test]
        public void Code_gets_fixed_for_public_constructor_with_additional_information_([ValueSource(nameof(Constructors))] string originalText)
        {
            const string Template = """

                                    using System;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// ###.
                                        /// </summary>
                                        public TestMe() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText + " for something"), Template.Replace("###", """Initializes a new instance of the <see cref="TestMe"/> class with something"""));
        }

        [Test]
        public void Code_gets_fixed_for_private_constructor_([ValueSource(nameof(Constructors))] string originalText)
        {
            const string Template = """

                                    using System;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// ###.
                                        /// </summary>
                                        private TestMe() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", """Prevents a default instance of the <see cref="TestMe"/> class from being created"""));
        }

        [Test]
        public void Code_gets_fixed_for_public_constructor_for_generic_type_([ValueSource(nameof(Constructors))] string originalText)
        {
            const string Template = """

                                    using System;

                                    public class TestMe<T1, T2>
                                    {
                                        /// <summary>
                                        /// ###.
                                        /// </summary>
                                        public TestMe() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", """Initializes a new instance of the <see cref="TestMe{T1,T2}"/> class"""));
        }

        [TestCase("A callback that is called", "Gets called")]
        [TestCase("A callback which is called", "Gets called")]
        [TestCase("A method that gets called", "Gets called")]
        [TestCase("A method that is called", "Gets called")]
        [TestCase("A method which gets called", "Gets called")]
        [TestCase("A method which is called", "Gets called")]
        [TestCase("Callback that is called", "Gets called")]
        [TestCase("Callback which is called", "Gets called")]
        [TestCase("Method that gets called", "Gets called")]
        [TestCase("Method that is called", "Gets called")]
        [TestCase("Method which gets called", "Gets called")]
        [TestCase("Method which is called", "Gets called")]
        [TestCase("The callback that is called", "Gets called")]
        [TestCase("The callback which is called", "Gets called")]
        [TestCase("The method gets called", "Gets called")]
        [TestCase("The method is called", "Gets called")]
        [TestCase("The method that gets called", "Gets called")]
        [TestCase("The method that is called", "Gets called")]
        [TestCase("The method which gets called", "Gets called")]
        [TestCase("The method which is called", "Gets called")]
        [TestCase("This method gets called", "Gets called")]
        [TestCase("This method is called", "Gets called")]
        [TestCase("Called by someone", "Gets called by someone")]
        [TestCase("Called if someone tries", "Gets called if someone tries")]
        public void Code_gets_fixed_for_(string originalText, string fixedText)
        {
            const string Template = """

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// ### to do something.
                                        /// </summary>
                                        public void DoSomething() { }
                                    }
                                    """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        protected override string GetDiagnosticId() => MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2019_CodeFixProvider();
    }
}