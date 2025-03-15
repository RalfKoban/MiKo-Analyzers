using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MeaninglessTextPhrases =
                                                                  [
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
                                                                  ];

        private static readonly string[] MeaninglessPhrases = CreateMeaninglessPhrases(MeaninglessTextPhrases);

        private static readonly string[] MeaninglessFieldPhrases = MeaninglessPhrases.Except(Constants.Comments.FieldStartingPhrase).ToArray();

#if NCRUNCH

        [OneTimeSetUp]
        public static void PrepareTestEnvironment() => MiKo_2060_CodeFixProvider.LoadData();

#else

        private static int s_testNumber;

        [OneTimeSetUp]
        public static void PrepareTestEnvironment() => s_testNumber = 0;

        [OneTimeTearDown]
        public static void CleanupTestEnvironment() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

        [SetUp]
        public void PrepareTest() => Interlocked.Increment(ref s_testNumber);

        [TearDown]
        public void TearDown()
        {
            if (s_testNumber % 5000 == 0)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            }
        }

#endif

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
        public void An_issue_is_reported_for_class_with_meaningless_phrase_([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_class_with_meaningless_special_phrase_([Values("Contains", "Contain", "Has", "Is")] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_class_with_meaningless_phrase_in_para_tag_([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_method_with_meaningless_phrase_([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_method_with_meaningless_phrase_in_para_tag_([ValueSource(nameof(MeaninglessPhrases))] string phrase) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_field_with_meaningless_phrase_([ValueSource(nameof(MeaninglessFieldPhrases))] string phrase) => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_field_with_meaningless_phrase_in_para_tag_([ValueSource(nameof(MeaninglessFieldPhrases))] string phrase) => An_issue_is_reported_for(@"
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

        [TestCase("Able to render something", "Renders something")]
        [TestCase("Capable to render something", "Renders something")]
        [TestCase("A class that adopts", "Adopts")]
        [TestCase("A interface that adopts", "Adopts")]
        [TestCase("An interface that adopts", "Adopts")]
        [TestCase("Class that adopts", "Adopts")]
        [TestCase("Class that allows to do", "Does")]
        [TestCase("Class that creates", "Creates")]
        [TestCase("Class that describes", "Describes")]
        [TestCase("Class that enhances", "Enhances")]
        [TestCase("Class that extends", "Extends")]
        [TestCase("Class that represents", "Represents")]
        [TestCase("Class that serves as", "Represents a")]
        [TestCase("Class that serves", "Provides")]
        [TestCase("Class that will represent", "Represents")]
        [TestCase("Class to provide", "Provides")]
        [TestCase("Class which serves as", "Represents a")]
        [TestCase("Class which serves", "Provides")]
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
        [TestCase("Every class that implements the interface can", "Allows to")]
        [TestCase("Every class that implements this interface can do", "Allows to do")]
        [TestCase("Every class that implements this interface can", "Allows to")]
        [TestCase("Extension of", "Extends the")]
        [TestCase("Factory method creating", "Creates")]
        [TestCase("Factory method that creates", "Creates")]
        [TestCase("Factory method to create", "Creates")]
        [TestCase("Factory method which creates", "Creates")]
        [TestCase("Function that generates", "Generates")]
        [TestCase("Function to generate", "Generates")]
        [TestCase("Function which generates", "Generates")]
        [TestCase("Help function that generates", "Generates")]
        [TestCase("Help function to generate", "Generates")]
        [TestCase("Help function which generates", "Generates")]
        [TestCase("Help method that generates", "Generates")]
        [TestCase("Help method to generate", "Generates")]
        [TestCase("Help method which generates", "Generates")]
        [TestCase("Helper class that manipulates", "Manipulates")]
        [TestCase("Helper class to manipulate", "Manipulates")]
        [TestCase("Helper class which manipulates", "Manipulates")]
        [TestCase("Helper function that generates", "Generates")]
        [TestCase("Helper function to generate", "Generates")]
        [TestCase("Helper function which generates", "Generates")]
        [TestCase("Helper method that generates", "Generates")]
        [TestCase("Helper method to generate", "Generates")]
        [TestCase("Helper method which generates", "Generates")]
        [TestCase("Interface definition for a something", "Represents a something")]
        [TestCase("Interface definition for an something", "Represents an something")]
        [TestCase("Interface definition for something", "Represents something")]
        [TestCase("Interface definition for the something", "Represents the something")]
        [TestCase("Interface definition of a helper which provides", "Provides")]
        [TestCase("Interface definition of an helper which provides", "Provides")]
        [TestCase("Interface definition of helper which provides", "Provides")]
        [TestCase("Interface definition of the helper which provides", "Provides")]
        [TestCase("Interface describing", "Describes")]
        [TestCase("Interface for a", "Represents a")]
        [TestCase("Interface for an", "Represents an")]
        [TestCase("Interface for classes that can provide", "Provides")]
        [TestCase("Interface for classes that provide", "Provides")]
        [TestCase("Interface for classes that represent", "Represents")]
        [TestCase("Interface for classes which can provide", "Provides")]
        [TestCase("Interface for elements that provide", "Provides")]
        [TestCase("Interface for items that perform", "Performs")]
        [TestCase("Interface for items that provide", "Provides")]
        [TestCase("Interface for items that represent", "Represents")]
        [TestCase("Interface for items which perform", "Performs")]
        [TestCase("Interface for items which provide", "Provides")]
        [TestCase("Interface for items which represent", "Represents")]
        [TestCase("Interface for objects that can provide", "Provides")]
        [TestCase("Interface for objects that provide", "Provides")]
        [TestCase("Interface for objects that represent", "Represents")]
        [TestCase("Interface for objects which can provide", "Provides")]
        [TestCase("Interface for objects which represent", "Represents")]
        [TestCase("Interface for processing", "Processes")]
        [TestCase("Interface for storing", "Stores")]
        [TestCase("Interface for the", "Represents a")]
        [TestCase("Interface for view models describing", "Describes")]
        [TestCase("Interface for view models representing", "Represents")]
        [TestCase("Interface for work flows that perform", "Performs")]
        [TestCase("Interface for work flows which perform", "Performs")]
        [TestCase("Interface for workflows that perform", "Performs")]
        [TestCase("Interface for workflows which perform", "Performs")]
        [TestCase("Interface for wrapping", "Wraps")]
        [TestCase("Interface implemented to detect", "Detects")]
        [TestCase("Interface of a view model", "Represents a view model")]
        [TestCase("Interface providing", "Provides")]
        [TestCase("Interface representing", "Represents")]
        [TestCase("Interface that serves", "Provides")]
        [TestCase("Interface to describe", "Describes")]
        [TestCase("Interface to represent", "Represents")]
        [TestCase("Interface which serves", "Provides")]
        [TestCase("Method that generates", "Generates")]
        [TestCase("Method to generate", "Generates")]
        [TestCase("Method which generates", "Generates")]
        [TestCase("The class adopts", "Adopts")]
        [TestCase("The class implementing this interface provides", "Provides")]
        [TestCase("The class offers", "Provides")]
        [TestCase("The class that adopts", "Adopts")]
        [TestCase("The class which adopts", "Adopts")]
        [TestCase("The interface offers", "Provides")]
        [TestCase("The interface that adopts", "Adopts")]
        [TestCase("The interface which adopts", "Adopts")]
        [TestCase("The interface that may be used to adopt", "Adopts")]
        [TestCase("The interface which may be used to adopt", "Adopts")]
        [TestCase("The interface that might be used to adopt", "Adopts")]
        [TestCase("The interface which might be used to adopt", "Adopts")]
        [TestCase("The interface that can be used to adopt", "Adopts")]
        [TestCase("The interface which can be used to adopt", "Adopts")]
        [TestCase("The interface that could be used to adopt", "Adopts")]
        [TestCase("The interface which could be used to adopt", "Adopts")]
        [TestCase("This class adopts", "Adopts")]
        [TestCase("This class offers", "Provides")]
        [TestCase("This class provides", "Provides")]
        [TestCase("This class represents", "Represents")]
        [TestCase("This interface offers", "Provides")]
        [TestCase("This interface represents", "Represents")]
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

        [TestCase("A class containing factory methods for building", "Provides support for creating")]
        [TestCase("A class containing factory methods for constructing", "Provides support for creating")]
        [TestCase("A class containing methods for building", "Provides support for creating")]
        [TestCase("A class containing methods for constructing", "Provides support for creating")]
        [TestCase("Factory class building", "Provides support for creating")]
        [TestCase("Factory class constructing", "Provides support for creating")]
        [TestCase("Factory class creating", "Provides support for creating")]
        [TestCase("Factory class that builds", "Provides support for creating")]
        [TestCase("Factory class that constructs", "Provides support for creating")]
        [TestCase("Factory class that creates", "Provides support for creating")]
        [TestCase("Factory class to build", "Provides support for creating")]
        [TestCase("Factory class to construct", "Provides support for creating")]
        [TestCase("Factory class to create", "Provides support for creating")]
        [TestCase("Factory for", "Provides support for creating")]
        [TestCase("Interface for factories creating", "Provides support for creating")]
        [TestCase("Interface for factories that create", "Provides support for creating")]
        [TestCase("Interface for factories which create", "Provides support for creating")]
        [TestCase("Interface of a factory creating", "Provides support for creating")]
        [TestCase("Interface of a factory that creates", "Provides support for creating")]
        [TestCase("Interface of a factory which creates", "Provides support for creating")]
        [TestCase("The class contains factory methods for building", "Provides support for creating")]
        [TestCase("The class contains factory methods for constructing", "Provides support for creating")]
        [TestCase("The class contains methods for building", "Provides support for creating")]
        [TestCase("The class contains methods for constructing", "Provides support for creating")]
        [TestCase("This class contains factory methods for building", "Provides support for creating")]
        [TestCase("This class contains factory methods for constructing", "Provides support for creating")]
        [TestCase("This class contains methods for building", "Provides support for creating")]
        [TestCase("This class contains methods for constructing", "Provides support for creating")]
        [TestCase("Used to create something", "Provides support for creating something")]
        [TestCase(@"Used to create <see cref=""string""/> instances", @"Provides support for creating <see cref=""string""/> instances")]
        [TestCase("Used for creating something", "Provides support for creating something")]
        [TestCase(@"Used for creating <see cref=""string""/> instances", @"Provides support for creating <see cref=""string""/> instances")]
        public void Code_gets_fixed_for_factory_types_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ### something.
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

        [TestCase("A class that can be used to")]
        [TestCase("A class that could be used to")]
        [TestCase("A class that is used to")]
        [TestCase("A class that may be used to")]
        [TestCase("A class that might be used to")]
        [TestCase("A class that shall be used to")]
        [TestCase("A class that should be used to")]
        [TestCase("A class that will be used to")]
        [TestCase("A class that would be used to")]
        [TestCase("A class to")]
        [TestCase("A class which is used to")]
        [TestCase("Abstract base class that can be used to")]
        [TestCase("Abstract base class that could be used to")]
        [TestCase("Abstract base class that is used to")]
        [TestCase("Abstract base class that may be used to")]
        [TestCase("Abstract base class that might be used to")]
        [TestCase("Abstract base class that shall be used to")]
        [TestCase("Abstract base class that should be used to")]
        [TestCase("Abstract base class that will be used to")]
        [TestCase("Abstract base class that would be used to")]
        [TestCase("Abstract base class to")]
        [TestCase("Abstract base class which is used to")]
        [TestCase("An abstract base class that can be used to")]
        [TestCase("An abstract base class that could be used to")]
        [TestCase("An abstract base class that is used to")]
        [TestCase("An abstract base class that may be used to")]
        [TestCase("An abstract base class that might be used to")]
        [TestCase("An abstract base class that shall be used to")]
        [TestCase("An abstract base class that should be used to")]
        [TestCase("An abstract base class that will be used to")]
        [TestCase("An abstract base class that would be used to")]
        [TestCase("An abstract base class to")]
        [TestCase("An abstract base class which is used to")]
        [TestCase("An base class that can be used to")]
        [TestCase("An base class that could be used to")]
        [TestCase("An base class that is used to")]
        [TestCase("An base class that may be used to")]
        [TestCase("An base class that might be used to")]
        [TestCase("An base class that shall be used to")]
        [TestCase("An base class that should be used to")]
        [TestCase("An base class that will be used to")]
        [TestCase("An base class that would be used to")]
        [TestCase("An base class to")]
        [TestCase("An base class which is used to")]
        [TestCase("An class that is used to")]
        [TestCase("An class to")]
        [TestCase("An class which is used to")]
        [TestCase("Base class that can be used to")]
        [TestCase("Base class that could be used to")]
        [TestCase("Base class that is used to")]
        [TestCase("Base class that may be used to")]
        [TestCase("Base class that might be used to")]
        [TestCase("Base class that shall be used to")]
        [TestCase("Base class that should be used to")]
        [TestCase("Base class that will be used to")]
        [TestCase("Base class that would be used to")]
        [TestCase("Base class to")]
        [TestCase("Base class which is used to")]
        [TestCase("Class to")]
        [TestCase("Class used to")]
        [TestCase("The abstract base class that can be used to")]
        [TestCase("The abstract base class that could be used to")]
        [TestCase("The abstract base class that is used to")]
        [TestCase("The abstract base class that may be used to")]
        [TestCase("The abstract base class that might be used to")]
        [TestCase("The abstract base class that shall be used to")]
        [TestCase("The abstract base class that should be used to")]
        [TestCase("The abstract base class that will be used to")]
        [TestCase("The abstract base class that would be used to")]
        [TestCase("The abstract base class to")]
        [TestCase("The abstract base class which is used to")]
        [TestCase("The base class that can be used to")]
        [TestCase("The base class that could be used to")]
        [TestCase("The base class that is used to")]
        [TestCase("The base class that may be used to")]
        [TestCase("The base class that might be used to")]
        [TestCase("The base class that shall be used to")]
        [TestCase("The base class that should be used to")]
        [TestCase("The base class that will be used to")]
        [TestCase("The base class that would be used to")]
        [TestCase("The base class to")]
        [TestCase("The base class which is used to")]
        [TestCase("The class can be used to")]
        [TestCase("The class could be used to")]
        [TestCase("The class is used to")]
        [TestCase("The class may be used to")]
        [TestCase("The class might be used to")]
        [TestCase("The class shall be used to")]
        [TestCase("The class should be used to")]
        [TestCase("The class that is used to")]
        [TestCase("The class will be used to")]
        [TestCase("The class would be used to")]
        [TestCase("This class is used to")]
        [TestCase("Used to")]
        public void Code_gets_fixed_for_class_(string startingPhrase)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", startingPhrase + " render"), Template.Replace("###", "Renders"));
        }

        [TestCase("A workflow that updates something.", "Represents a workflow that updates something.")]
        public void Code_gets_fixed_for_workflow_class_(string originalComment, string fixedComment)
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

        [TestCase("A interface to")]
        [TestCase("An interface to")]
        [TestCase("A interface that is used to")]
        [TestCase("An interface that is used to")]
        [TestCase("An interface that can be used to")]
        [TestCase("An interface that could be used to")]
        [TestCase("An interface that may be used to")]
        [TestCase("An interface that might be used to")]
        [TestCase("An interface that shall be used to")]
        [TestCase("An interface that should be used to")]
        [TestCase("An interface that will be used to")]
        [TestCase("An interface that would be used to")]
        [TestCase("A interface which is used to")]
        [TestCase("An interface which is used to")]
        [TestCase("Interface to")]
        [TestCase("Interface used to")]
        [TestCase("The interface is used to")]
        [TestCase("The interface that is used to")]
        [TestCase("This interface is used to")]
        [TestCase("Used to")]
        public void Code_gets_fixed_for_interface_(string startingPhrase)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public interface ITestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", startingPhrase + " render"), Template.Replace("###", "Renders"));
        }

        [TestCase("Attribute that allows to do something", "Does something")]
        [TestCase("Attribute which allows to do something", "Does something")]
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

        [TestCase("View model for a something", "Represents the view model of a something")]
        [TestCase("View Model for a something", "Represents the view model of a something")]
        [TestCase("View model for an anything", "Represents the view model of an anything")]
        [TestCase("View Model for an anything", "Represents the view model of an anything")]
        [TestCase("View model for the something", "Represents the view model of the something")]
        [TestCase("View Model for the something", "Represents the view model of the something")]
        [TestCase("View model for something", "Represents the view model of something")]
        [TestCase("View Model for something", "Represents the view model of something")]
        [TestCase("ViewModel for something", "Represents the view model of something")]

        [TestCase("View model representing a something", "Represents the view model of a something")]
        [TestCase("View Model representing a something", "Represents the view model of a something")]
        [TestCase("View model representing an anything", "Represents the view model of an anything")]
        [TestCase("View Model representing an anything", "Represents the view model of an anything")]
        [TestCase("View model representing the something", "Represents the view model of the something")]
        [TestCase("View Model representing the something", "Represents the view model of the something")]
        [TestCase("View model representing something", "Represents the view model of something")]
        [TestCase("View Model representing something", "Represents the view model of something")]
        [TestCase("ViewModel representing something", "Represents the view model of something")]

        [TestCase("View model that represents a something", "Represents the view model of a something")]
        [TestCase("View Model that represents a something", "Represents the view model of a something")]
        [TestCase("View model that represents an anything", "Represents the view model of an anything")]
        [TestCase("View Model that represents an anything", "Represents the view model of an anything")]
        [TestCase("View model that represents the something", "Represents the view model of the something")]
        [TestCase("View Model that represents the something", "Represents the view model of the something")]
        [TestCase("View model that represents something", "Represents the view model of something")]
        [TestCase("View Model that represents something", "Represents the view model of something")]
        [TestCase("ViewModel that represents something", "Represents the view model of something")]

        [TestCase("View model of a something", "Represents the view model of a something")]
        [TestCase("View Model of a something", "Represents the view model of a something")]
        [TestCase("View model of an anything", "Represents the view model of an anything")]
        [TestCase("View Model of an anything", "Represents the view model of an anything")]
        [TestCase("View model of the something", "Represents the view model of the something")]
        [TestCase("View Model of the something", "Represents the view model of the something")]
        [TestCase("View model of something", "Represents the view model of something")]
        [TestCase("View Model of something", "Represents the view model of something")]
        [TestCase("ViewModel of something", "Represents the view model of something")]

        [TestCase(@"ViewModel for <see cref=""string""/>", "Represents the view model of <see cref=\"string\"/>")]
        [TestCase(@"ViewModel of <see cref=""string""/>", "Represents the view model of <see cref=\"string\"/>")]
        public void Code_gets_fixed_for_view_model_(string originalComment, string fixedComment)
        {
            const string Template = @"
/// <summary>
/// ###.
/// </summary>
public class TestMeViewModel
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("A component to")]
        [TestCase("A component that is used to")]
        [TestCase("A component which is used to")]
        [TestCase("A component used to")]
        [TestCase("A component that is able to")]
        [TestCase("A component which is able to")]
        [TestCase("A component that is capable to")]
        [TestCase("A component which is capable to")]
        [TestCase("A component able to")]
        [TestCase("A component capable to")]
        [TestCase("An component to")]
        [TestCase("An component that is used to")]
        [TestCase("An component which is used to")]
        [TestCase("An component used to")]
        [TestCase("An component that is able to")]
        [TestCase("An component which is able to")]
        [TestCase("An component that is capable to")]
        [TestCase("An component which is capable to")]
        [TestCase("An component able to")]
        [TestCase("An component capable to")]
        [TestCase("Component to")]
        [TestCase("Component that is used to")]
        [TestCase("Component which is used to")]
        [TestCase("Component used to")]
        [TestCase("Component that is able to")]
        [TestCase("Component which is able to")]
        [TestCase("Component that is capable to")]
        [TestCase("Component which is capable to")]
        [TestCase("Component able to")]
        [TestCase("Component capable to")]
        [TestCase("The component to")]
        [TestCase("The component that is used to")]
        [TestCase("The component which is used to")]
        [TestCase("The component used to")]
        [TestCase("The component is used to")]
        [TestCase("The component is able to")]
        [TestCase("The component is capable to")]
        [TestCase("The component that is able to")]
        [TestCase("The component which is able to")]
        [TestCase("The component that is capable to")]
        [TestCase("The component which is capable to")]
        [TestCase("The component able to")]
        [TestCase("The component capable to")]
        [TestCase("This component is used to")]
        [TestCase("This component is able to")]
        [TestCase("This component is capable to")]
        public void Code_gets_fixed_for_component_text_(string startingPhrase)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public class TestMe
{
}
";

            VerifyCSharpFix(Template.Replace("###", startingPhrase + " render"), Template.Replace("###", "Renders"));
        }

        [TestCase("Used to set something", "Sets something")]
        [TestCase("Used to get something", "Gets something")]
        public void Code_gets_fixed_for_property_text_(string originalComment, string fixedComment)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    public int Property { get; set; }
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2012_MeaninglessSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2012_MeaninglessSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2012_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static string[] CreateMeaninglessPhrases(string[] phrases)
        {
            string[] types =
                             [
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
                             ];

            var results = new HashSet<string>
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
            results.AddRange(from type in types from phrase in phrases select string.Concat(type, " ", phrase));
            results.AddRange(phrases.Select(_ => _.ToLowerInvariant()));
            results.AddRange(phrases.Select(_ => _.ToUpperInvariant()));

            results.Add("<see cref=\"ITestMe\"/>");
            results.Add("<see cref=\"ITestMe\" />");

            return [.. results];
        }
    }
}