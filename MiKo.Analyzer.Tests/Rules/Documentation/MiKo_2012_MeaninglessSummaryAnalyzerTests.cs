using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NCrunch.Framework;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture, Isolated]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MeaninglessTextPhrases =
            {
                "does implement",
                "implements",
                "that is called ",
                "that is used for ",
                "that is used to ",
                "used for ",
                "used to ",
                "which is called ",
                "which is used for ",
                "which is used to ",
            };

        private static readonly string[] MeaninglessPhrases = CreateMeaninglessPhrases();

        private static readonly string[] MeaninglessFieldPhrases = MeaninglessPhrases.Except(new[] { "A ", "An ", "The " }).ToArray();

        [Test]
        public void No_issue_is_reported_for_class_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_exception_class_with_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// The exception to be thrown.
/// </summary>
public class TestMeException : System.Exception
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Some documentation.
/// </para>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_phrase()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

/// <summary>
/// " + phrase + @" whatever
/// </summary>
public class TestMe : ITestMe
{
}
");
                                    }
                                });
        }

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_special_phrase_([Values("Contains", "Contain", "Has")] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// " + phrase + @" whatever
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Contains_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Contains documentation.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_phrase_in_para_tag()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

/// <summary>
/// <para>
/// " + phrase + @" whatever
/// </para>
/// </summary>
public class TestMe : ITestMe
{
}
");
                                    }
                                });
        }

        [Test]
        public void No_issue_is_reported_for_method_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Some documentation.
    /// </para>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase + @" whatever
    /// </summary>
    public void DoSomething() { }
}
");
                                    }
                                });
        }

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase_in_para_tag()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// <para>
    /// " + phrase + @" whatever
    /// </para>
    /// </summary>
    public void DoSomething() { }
}
");
                                    }
                                });
        }

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase_in_text_([ValueSource(nameof(MeaninglessTextPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// Some text " + phrase + @" whatever
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_field_without_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    private int DoSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_documentation() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    private int DoSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_documentation_in_para_tag() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// <para>
    /// Some documentation.
    /// </para>
    /// </summary>
    private int DoSomething;
}
");

        [TestCase("Some")]
        [TestCase("All")]
        public void No_issue_is_reported_for_enum_member_with_documentation_(string firstWord) => No_issue_is_reported_for(@"
public enum TestMe
{
    /// <summary>
    /// " + firstWord + @" documentation.
    /// </summary>
    None = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessFieldPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// " + phrase + @" whatever
    /// </summary>
    private int DoSomething;
}
");
                                    }
                                });
        }

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase_in_para_tag()
        {
            Assert.Multiple(() =>
                                {
                                    foreach (var phrase in MeaninglessFieldPhrases)
                                    {
                                        An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// <para>
    /// " + phrase + @" whatever
    /// </para>
    /// </summary>
    private int DoSomething;
}
");
                                    }
                                });
        }

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase_in_text_([ValueSource(nameof(MeaninglessTextPhrases))] string phrase) => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe : ITestMe
{
    /// <summary>
    /// Some text " + phrase + @" whatever
    /// </summary>
    private int DoSomething;
}
");

        [TestCase("Class that adopts", "Adopts")]
        [TestCase("Class that allows", "Allows")]
        [TestCase("Class that creates", "Creates")]
        [TestCase("Class that describes", "Describes")]
        [TestCase("Class that enhances", "Enhances")]
        [TestCase("Class that extends", "Extends")]
        [TestCase("Class that represents", "Represents")]
        [TestCase("Class that serves", "Provides")]
        [TestCase("Class that serves as", "Represents a")]
        [TestCase("Class that will represent", "Represents")]
        [TestCase("Class to provide", "Provides")]
        [TestCase("Class which serves", "Provides")]
        [TestCase("Class which serves as", "Represents a")]
        [TestCase("Class which will represent", "Represents")]
        [TestCase("Classes implementing the interfaces provide", "Provides")]
        [TestCase("Classes implementing the interfaces will provide", "Provides")]
        [TestCase("Classes implementing the interfaces, will provide", "Provides")]
        [TestCase("Contain", "Provides")]
        [TestCase("Contains", "Provides")]
        [TestCase("Event argument for", "Provides data for the")]
        [TestCase("Event argument that is used in the", "Provides data for the")]
        [TestCase("Event argument that provides information", "Provides data for the")]
        [TestCase("Event argument which is used in the", "Provides data for the")]
        [TestCase("Event argument which provides information", "Provides data for the")]
        [TestCase("Event arguments for", "Provides data for the")]
        [TestCase("Event arguments that provide information", "Provides data for the")]
        [TestCase("Event arguments which provide information", "Provides data for the")]
        [TestCase("Event is fired", "Occurs")]
        [TestCase("Event that is published", "Occurs")]
        [TestCase("Event that is published,", "Occurs")]
        [TestCase("Event which is published", "Occurs")]
        [TestCase("Event which is published,", "Occurs")]
        [TestCase("Every class that implements this interface can do", "Allows to do")]
        [TestCase("Every class that implements this interface can", "Allows to")]
        [TestCase("Every class that implements the interface can", "Allows to")]
        [TestCase("Extension of", "Extends the")]
        [TestCase("Factory class creating", "Provides support for creating")]
        [TestCase("Factory class that creates", "Provides support for creating")]
        [TestCase("Factory class to create", "Provides support for creating")]
        [TestCase("Factory for", "Provides support for creating")]
        [TestCase("Factory method creating", "Creates")]
        [TestCase("Factory method that creates", "Creates")]
        [TestCase("Factory method to create", "Creates")]
        [TestCase("Factory method which creates", "Creates")]
        [TestCase("Function to generate", "Generates")]
        [TestCase("Function that generates", "Generates")]
        [TestCase("Function which generates", "Generates")]
        [TestCase("Helper class that manipulates", "Manipulates")]
        [TestCase("Helper class to manipulate", "Manipulates")]
        [TestCase("Helper class which manipulates", "Manipulates")]
        [TestCase("Helper method to generate", "Generates")]
        [TestCase("Helper method that generates", "Generates")]
        [TestCase("Helper method which generates", "Generates")]
        [TestCase("Helper function to generate", "Generates")]
        [TestCase("Helper function that generates", "Generates")]
        [TestCase("Helper function which generates", "Generates")]
        [TestCase("Help function to generate", "Generates")]
        [TestCase("Help function that generates", "Generates")]
        [TestCase("Help function which generates", "Generates")]
        [TestCase("Help method to generate", "Generates")]
        [TestCase("Help method that generates", "Generates")]
        [TestCase("Help method which generates", "Generates")]
        [TestCase("Interface describing", "Describes")]
        [TestCase("Interface definition for something", "Represents something")]
        [TestCase("Interface definition for a something", "Represents a something")]
        [TestCase("Interface definition for an something", "Represents an something")]
        [TestCase("Interface definition for the something", "Represents the something")]
        [TestCase("Interface definition of helper which provides", "Provides")]
        [TestCase("Interface definition of a helper which provides", "Provides")]
        [TestCase("Interface definition of an helper which provides", "Provides")]
        [TestCase("Interface definition of the helper which provides", "Provides")]
        [TestCase("Interface for classes that represent", "Represents")]
        [TestCase("Interface for classes that provide", "Provides")]
        [TestCase("Interface for classes that can provide", "Provides")]
        [TestCase("Interface for classes which can provide", "Provides")]
        [TestCase("Interface for elements that provide", "Provides")]
        [TestCase("Interface for factories creating", "Provides support for creating")]
        [TestCase("Interface for factories that create", "Provides support for creating")]
        [TestCase("Interface for factories which create", "Provides support for creating")]
        [TestCase("Interface for items that represent", "Represents")]
        [TestCase("Interface for items which represent", "Represents")]
        [TestCase("Interface for items that perform", "Performs")]
        [TestCase("Interface for items which perform", "Performs")]
        [TestCase("Interface for items that provide", "Provides")]
        [TestCase("Interface for items which provide", "Provides")]
        [TestCase("Interface for objects that can provide", "Provides")]
        [TestCase("Interface for objects which can provide", "Provides")]
        [TestCase("Interface for objects that provide", "Provides")]
        [TestCase("Interface for objects that represent", "Represents")]
        [TestCase("Interface for objects which represent", "Represents")]
        [TestCase("Interface for processing", "Processes")]
        [TestCase("Interface for storing", "Stores")]
        [TestCase("Interface for a", "Represents a")]
        [TestCase("Interface for an", "Represents an")]
        [TestCase("Interface for the", "Represents a")]
        [TestCase("Interface for view models describing", "Describes")]
        [TestCase("Interface for view models representing", "Represents")]
        [TestCase("Interface for workflows that perform", "Performs")]
        [TestCase("Interface for workflows which perform", "Performs")]
        [TestCase("Interface for work flows that perform", "Performs")]
        [TestCase("Interface for work flows which perform", "Performs")]
        [TestCase("Interface for wrapping", "Wraps")]
        [TestCase("Interface implemented to detect", "Detects")]
        [TestCase("Interface of a factory creating", "Provides support for creating")]
        [TestCase("Interface of a factory that creates", "Provides support for creating")]
        [TestCase("Interface of a factory which creates", "Provides support for creating")]
        [TestCase("Interface of a view model", "Represents a view model")]
        [TestCase("Interface providing", "Provides")]
        [TestCase("Interface representing", "Represents")]
        [TestCase("Interface that serves", "Provides")]
        [TestCase("Interface to describe", "Describes")]
        [TestCase("Interface to represent", "Represents")]
        [TestCase("Interface which serves", "Provides")]
        [TestCase("Method to generate", "Generates")]
        [TestCase("Method that generates", "Generates")]
        [TestCase("Method which generates", "Generates")]
        [TestCase("The class offers", "Provides")]
        [TestCase("The interface offers", "Provides")]
        [TestCase("This class offers", "Provides")]
        [TestCase("This class provides", "Provides")]
        [TestCase("This class represents", "Represents")]
        [TestCase("This interface offers", "Provides")]
        [TestCase("This interface represents", "Represents")]
        [TestCase("The class implementing this interface provides", "Provides")]
        public void Code_gets_fixed_for_term_(string originalCode, string fixedCode)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_inheritdoc()
        {
            const string OriginalCode = @"
/// <summary>
/// <inheritdoc />
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc />
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_inheritdoc_for_default_implementation_(
                                                                        [Values("A", "The", "")] string startPhrase,
                                                                        [Values("Default implementation", "Default-implementation", "Default impl", "Default-impl", "Implementation")] string text,
                                                                        [Values("for", "of")] string middlePart)
        {
            var originalCode = @"
/// <summary>
/// " + startPhrase + " " + text + " " + middlePart + @" <see cref=""ITestMe"" />.
/// </summary>
public class TestMe
{
}

public interface ITestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc cref=""ITestMe""/>
public class TestMe
{
}

public interface ITestMe
{
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_inheritdoc_for_summary_that_contains_only_a_see_cref()
        {
            const string OriginalCode = @"
/// <summary>
/// <see cref=""TestMe""/>
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <inheritdoc/>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_XAML()
        {
            const string OriginalCode = @"
/// <summary>
/// Interaction logic for TestMe.xaml
/// </summary>
public class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Represents a TODO
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_declarations()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// The value.
    /// </summary>
    public int Something1 { get; set; }

    /// <summary>
    /// A value.
    /// </summary>
    public int Something2 { get; }

    /// <summary>
    /// An value.
    /// </summary>
    public int Something3 { set; }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Something1 { get; set; }

    /// <summary>
    /// Gets a value.
    /// </summary>
    public int Something2 { get; }

    /// <summary>
    /// Sets an value.
    /// </summary>
    public int Something3 { set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Initialize_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Initialize something.
    /// </summary>
    public void Initialize() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Initializes something.
    /// </summary>
    public void Initialize() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("A command that can", "Represents a command that can")]
        [TestCase("Command that can", "Represents a command that can")]
        [TestCase("Command that does", "Represents a command that can do")]
        [TestCase("Command that opens", "Represents a command that can open")]
        [TestCase("Command which does", "Represents a command that can do")]
        [TestCase("Command which opens", "Represents a command that can open")]
        [TestCase("Command for", "Represents a command that can")]
        [TestCase("Command to", "Represents a command that can")]
        [TestCase("command to", "Represents a command that can")]
        public void Code_gets_fixed_for_command_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public class TestMeCommand
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Used to create something", "Provides support for creating something")]
        [TestCase(@"Used to create <see cref=""string""/> instances", @"Provides support for creating <see cref=""string""/> instances")]
        [TestCase("Used for creating something", "Provides support for creating something")]
        [TestCase(@"Used for creating <see cref=""string""/> instances", @"Provides support for creating <see cref=""string""/> instances")]
        public void Code_gets_fixed_for_factory_types_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ###
/// </summary>
public class TestMeFactory
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Handler for event", "Handles the event")]
        [TestCase("Handler for the event", "Handles the event")]
        [TestCase(@"Handler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Handler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase("EventHandler for event", "Handles the event")]
        [TestCase("EventHandler for the event", "Handles the event")]
        [TestCase(@"EventHandler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"EventHandler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase("Event handler for event", "Handles the event")]
        [TestCase("Event handler for the event", "Handles the event")]
        [TestCase(@"Event handler for <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        [TestCase(@"Event handler for the <see cref=""string""/> event", @"Handles the <see cref=""string""/> event")]
        public void Code_gets_fixed_for_event_handler_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    void OnSomething(object sender, EventArgs e);
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Extension method that initializes the given value.", "Initializes the given value.")]
        [TestCase("Extension method that will initialize the given value.", "Initializes the given value.")]
        [TestCase("Extension method which initializes the given value.", "Initializes the given value.")]
        [TestCase("Extension method which will initialize the given value.", "Initializes the given value.")]
        public void Code_gets_fixed_for_extension_method_(string originalComment, string fixedComment)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    public static void DoSomething(this TestMe value) { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("A class to render something", "Renders something")]
        [TestCase("An class to render something", "Renders something")]
        [TestCase("A class that is used to render something", "Renders something")]
        [TestCase("An class that is used to render something", "Renders something")]
        [TestCase("A class which is used to render something", "Renders something")]
        [TestCase("An class which is used to render something", "Renders something")]
        [TestCase("Class to render something", "Renders something")]
        [TestCase("Class used to render something", "Renders something")]
        [TestCase("The class is used to render something", "Renders something")]
        [TestCase("The class that is used to render something", "Renders something")]
        [TestCase("This class is used to render something", "Renders something")]
        [TestCase("Used to render something", "Renders something")]
        public void Code_gets_fixed_for_class_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ###.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("A interface to render something", "Renders something")]
        [TestCase("An interface to render something", "Renders something")]
        [TestCase("A interface that is used to render something", "Renders something")]
        [TestCase("An interface that is used to render something", "Renders something")]
        [TestCase("A interface which is used to render something", "Renders something")]
        [TestCase("An interface which is used to render something", "Renders something")]
        [TestCase("Interface to render something", "Renders something")]
        [TestCase("Interface used to render something", "Renders something")]
        [TestCase("The interface is used to render something", "Renders something")]
        [TestCase("The interface that is used to render something", "Renders something")]
        [TestCase("This interface is used to render something", "Renders something")]
        [TestCase("Used to render something", "Renders something")]
        public void Code_gets_fixed_for_interface_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ###.
/// </summary>
public interface ITestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Attribute that allows to render something", "Allows to render something")]
        public void Code_gets_fixed_for_attribute_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ###.
/// </summary>
public class TestMeAttribute : System.Attribute
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2012_MeaninglessSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2012_MeaninglessSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2012_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateMeaninglessPhrases()
        {
            var types = new[]
                            {
                                "Attribute",
                                "Base",
                                "Class",
                                "Interface",
                                "Method",
                                "Field",
                                "Property",
                                "Event",
                                "EventHandler",
                                "Handler",
                                "Component",
                                "Constructor",
                                "Ctor",
                                "Delegate",
                                "Creator",
                                "Entity",
                                "Model",
                                "View",
                                "ViewModel",
                                "Command",
                                "Action",
                                "Func",
                                "Converter",
                                "Adapter ",
                                "Builder",
                                "Factory",
                                "Proxy ",
                                "Wrapper ",
                            };

            var phrases = MeaninglessTextPhrases;

            var results = new List<string>
                              {
                                  "TestMe",
                                  "ITestMe",
                                  "A ",
                                  "An ",
                                  "Builder ",
                                  "Called ",
                                  "Does implement ",
                                  "Extension class of ",
                                  "Extension method that will ",
                                  "Extension method which will ",
                                  "Extension method that ",
                                  "Extension method which ",
                                  "Extension of ",
                                  "Helper class",
                                  "Implement ",
                                  "Implements ",
                                  "Interaction logic ",
                                  "Implementation of ",
                                  "Default-implementation of ",
                                  "Default implementation of ",
                                  "Default-Implementation for ",
                                  "Default Implementation for ",
                                  "Impl ",
                                  "Default impl ",
                                  "Default-Impl ",
                                  "Is for ",
                                  "Is to ",
                                  "Is used for ",
                                  "Is used to ",
                                  "The ",
                                  "This ",
                                  "That ",
                                  "Used for ",
                                  "Used to ",
                                  "Use this ",
                                  "Uses ",
                                  "It ",
                                  "Its ",
                                  "It's ",
                                  "Public ",
                                  "Protected ",
                                  "Internal ",
                                  "Private ",
                                  "Testclass ",
                                  "Mock ",
                                  "Fake ",
                                  "Stub ",
                              };

            results.AddRange(types);
            results.AddRange(phrases);
            results.AddRange(from type in types from phrase in phrases select type + " " + phrase);
            results.AddRange(phrases.Select(_ => _.ToLower()));
            results.AddRange(phrases.Select(_ => _.ToUpper()));

            results.Add("<see cref=\"ITestMe\"/>");
            results.Add("<see cref=\"ITestMe\" />");

            return results.Distinct().ToArray();
        }
    }
}