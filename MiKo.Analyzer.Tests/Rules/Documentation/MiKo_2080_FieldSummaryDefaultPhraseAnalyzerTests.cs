using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongBooleanPhrases = [.. CreateWrongBooleanPhrases().Take(TestLimit)];

        private static readonly string[] WrongCollectionPhrases =
                                                                  [
                                                                      "A cache for",
                                                                      "A Cache for",
                                                                      "A collection containing",
                                                                      "A collection for",
                                                                      "A collection holding",
                                                                      "A collection of",
                                                                      "A collection storing",
                                                                      "A collection that contains",
                                                                      "A collection that holds",
                                                                      "A collection that is containing",
                                                                      "A collection that is holding",
                                                                      "A collection that is storing",
                                                                      "A collection that stores",
                                                                      "A collection that's containing",
                                                                      "A collection that's holding",
                                                                      "A collection that's storing",
                                                                      "A collection which contains",
                                                                      "A collection which holds",
                                                                      "A collection which is containing",
                                                                      "A collection which is holding",
                                                                      "A collection which is storing",
                                                                      "A collection which stores",
                                                                      "A container for the array of",
                                                                      "A Container for the array of",
                                                                      "A container for the collection of",
                                                                      "A Container for the collection of",
                                                                      "A container for the dictionary of",
                                                                      "A Container for the dictionary of",
                                                                      "A container for the list of",
                                                                      "A Container for the list of",
                                                                      "A container for the queue of",
                                                                      "A Container for the queue of",
                                                                      "A container for the set of",
                                                                      "A Container for the set of",
                                                                      "A container for the stack of",
                                                                      "A Container for the stack of",
                                                                      "A dictionary for",
                                                                      "A dictionary of",
                                                                      "A list for",
                                                                      "A list of",
                                                                      "A queue that contains",
                                                                      "A Queue that contains",
                                                                      "A stack that contains",
                                                                      "A Stack that contains",
                                                                      "An array for",
                                                                      "An array of",
                                                                      "An array that holds",
                                                                      "An array which holds",
                                                                      "An enumerable containing",
                                                                      "An Enumerable containing",
                                                                      "Array for",
                                                                      "Array holding",
                                                                      "Array of",
                                                                      "Array that holds",
                                                                      "Array which holds",
                                                                      "Cache for",
                                                                      "Collection for",
                                                                      "Collection holding",
                                                                      "Collection of",
                                                                      "Collection remembering",
                                                                      "Collection that holds",
                                                                      "Collection that is holding",
                                                                      "Collection that is remembering",
                                                                      "Collection that remembers",
                                                                      "Collection which holds",
                                                                      "Collection which is holding",
                                                                      "Collection which is remembering",
                                                                      "Collection which remembers",
                                                                      "Contain",
                                                                      "Container for the array of",
                                                                      "Container for the collection of",
                                                                      "Container for the dictionary of",
                                                                      "Container for the list of",
                                                                      "Container for the queue of",
                                                                      "Container for the set of",
                                                                      "Container for the stack of",
                                                                      "Containing",
                                                                      "Dictionary for",
                                                                      "Dictionary of",
                                                                      "Enumerable for",
                                                                      "Hold",
                                                                      "Holding",
                                                                      "Holds",
                                                                      "List for",
                                                                      "List of",
                                                                      "Queue for",
                                                                      "Remember",
                                                                      "Remembering",
                                                                      "Remembers",
                                                                      "Stack for",
                                                                      "Store",
                                                                      "Stores",
                                                                      "Storing",
                                                                      "The collection that holds",
                                                                      "The collection which holds",
                                                                      "The container for the array of",
                                                                      "The Container for the array of",
                                                                      "The container for the collection of",
                                                                      "The Container for the collection of",
                                                                      "The container for the dictionary of",
                                                                      "The Container for the dictionary of",
                                                                      "The container for the list of",
                                                                      "The Container for the list of",
                                                                      "The container for the queue of",
                                                                      "The Container for the queue of",
                                                                      "The container for the set of",
                                                                      "The Container for the set of",
                                                                      "The container for the stack of",
                                                                      "The Container for the stack of",
                                                                      "This cache holds",
                                                                      "This collection holds",
                                                                      "This dictionary holds",
                                                                      "This is a array for",
                                                                      "This is a array of",
                                                                      "This is a cache for",
                                                                      "This is a cache of",
                                                                      "This is a dictionary for",
                                                                      "This is a dictionary of",
                                                                      "This is a list for",
                                                                      "This is a list of",
                                                                      "This list holds",
                                                                      "This member variable contains",
                                                                      "This member variable holds",
                                                                      "This member variable remembers",
                                                                  ];

        [OneTimeSetUp]
        public static void PrepareTestEnvironment() => MiKo_2080_CodeFixProvider.LoadData();

        [Test]
        public void No_issue_is_reported_for_undocumented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_field_starting_with_The() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_collection_field_starting_with_Contains() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Contains the data for the field.
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_field_starting_with_Indicates_whether() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Indicates whether something is the data for the field.
    /// </summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_Guid_field_starting_with_unique_identifier_phrase() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The unique identifier for something that is the data for the field.
    /// </summary>
    private Guid m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_const_field_starting_with_The() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    public const string Field;
}
");

        [Test]
        public void No_issue_is_reported_for_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Bla bla.
    /// </summary>
    Field = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_wrong_starting_phrase_([Values("Bla bla.", "Contains something.", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_collection_field_with_wrong_starting_phrase_([Values("Bla bla", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_field_with_wrong_starting_phrase_([Values("Bla bla", "The field", "Contains something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_field_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property. This field is read-only.
    /// </summary>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_RoutedEvent_field_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event. This field is read-only.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_constant_boolean_field_starting_with_The() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The Bla bla
    /// </summary>
    private const bool m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_field_starting_with_see_cref() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// <see cref=""TestMe""/>
    /// </summary>
    private const bool m_field;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_constant_boolean_field_with_wrong_starting_phrase_(
                                                                                            [Values("Bla bla", "Indicates whether the field", "Contains something.")] string comment,
                                                                                            [Values("bool")] string fieldType)
            => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private const " + fieldType + @" m_field;
}
");

        [TestCase("Some comment", "The some comment")]
        [TestCase("Specifies a flag that indicates whether some comment", "The some comment")]
        [TestCase("specifies a flag that indicates whether some comment", "The some comment")]
        [TestCase("Specifies a flag that indicates some comment", "The some comment")]
        [TestCase("specifies a flag that indicates some comment", "The some comment")]
        public void Code_gets_fixed_for_constant_boolean_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private const bool m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [Test]
        public void Code_gets_fixed_for_non_constant_boolean_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private bool m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Indicates whether some comment.
    /// </summary>
    private bool m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_constant_boolean_field_with_various_phrases_([ValueSource(nameof(WrongBooleanPhrases))] string originalComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### some comment.
    /// </summary>
    private bool m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", "Indicates whether"));
        }

        [Test]
        public void Code_gets_fixed_for_nullable_boolean_field_with_various_phrases_([ValueSource(nameof(WrongBooleanPhrases))] string originalComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### some comment.
    /// </summary>
    private bool? m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", "Indicates whether"));
        }

        [TestCase("A Guid of some comment", "The unique identifier for some comment")]
        [TestCase("A GUID of some comment", "The unique identifier for some comment")]
        [TestCase("A unique identifier for some comment", "The unique identifier for some comment")]
        [TestCase("An unique identifier for some comment", "The unique identifier for some comment")]
        [TestCase("Get some comment", "The unique identifier for some comment")]
        [TestCase("Gets some comment", "The unique identifier for some comment")]
        [TestCase("Gets the some comment", "The unique identifier for the some comment")]
        [TestCase("Guid of some comment", "The unique identifier for some comment")]
        [TestCase("GUID of some comment", "The unique identifier for some comment")]
        [TestCase("Guid of the comment", "The unique identifier for the comment")]
        [TestCase("Guids of some comment", "The unique identifier for some comment")]
        [TestCase("Guids of the comment", "The unique identifier for the comment")]
        [TestCase("Some comment", "The unique identifier for some comment")]
        [TestCase("The Guid of some comment", "The unique identifier for some comment")]
        [TestCase("The GUID of some comment", "The unique identifier for some comment")]
        [TestCase("Factory for some stuff", "The unique identifier for a factory for some stuff")]
        [TestCase("Guid for some comment", "The unique identifier for some comment")]
        [TestCase("GUID for some comment", "The unique identifier for some comment")]
        public void Code_gets_fixed_for_Guid_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private Guid m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("A TypeGuid for some comment", "The unique identifier for the type of some comment")]
        [TestCase("A TypeGuid of some comment", "The unique identifier for the type of some comment")]
        [TestCase("A Type Guid for some comment", "The unique identifier for the type of some comment")]
        [TestCase("A Type Guid of some comment", "The unique identifier for the type of some comment")]
        [TestCase("A Type GUID for some comment", "The unique identifier for the type of some comment")]
        [TestCase("A Type GUID of some comment", "The unique identifier for the type of some comment")]
        [TestCase("A TypeGuids for some comment", "The unique identifier for the type of some comment")] // typo
        [TestCase("A TypeGuids of some comment", "The unique identifier for the type of some comment")] // typo
        [TestCase("A unique identifier for the type of some comment", "The unique identifier for the type of some comment")]
        [TestCase("An unique identifier for the type of some comment", "The unique identifier for the type of some comment")]
        [TestCase("The TypeGuid for some comment", "The unique identifier for the type of some comment")]
        [TestCase("The TypeGuid of some comment", "The unique identifier for the type of some comment")]
        [TestCase("The TypeGuids for some comment", "The unique identifier for the type of some comment")]
        [TestCase("The TypeGuids of some comment", "The unique identifier for the type of some comment")]
        [TestCase("TypeGuid for some comment", "The unique identifier for the type of some comment")]
        [TestCase("TypeGuid of some comment", "The unique identifier for the type of some comment")]
        [TestCase("Type Guid for some comment", "The unique identifier for the type of some comment")]
        [TestCase("Type Guid of some comment", "The unique identifier for the type of some comment")]
        [TestCase("Type GUID for some comment", "The unique identifier for the type of some comment")]
        [TestCase("Type GUID of some comment", "The unique identifier for the type of some comment")]
        [TestCase("TypeGuids for some comment", "The unique identifier for the type of some comment")] // typo
        [TestCase("TypeGuids of some comment", "The unique identifier for the type of some comment")] // typo
        public void Code_gets_fixed_for_TypeGuid_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private Guid m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [Test]
        public void Code_gets_fixed_for_collection_field()
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private List<string> m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", "Some comment"), Template.Replace("###", "Contains the some comment"));
        }

        [TestCase("contain all the")]
        [TestCase("contain all")]
        [TestCase("contain the")]
        [TestCase("contains all the")]
        [TestCase("Contains all the")]
        [TestCase("contains all")]
        [TestCase("Contains all")]
        public void Code_gets_fixed_for_collection_field_with_various_phrases_(string startingPhrase)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### some comment.
    /// </summary>
    private List<string> m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", startingPhrase), Template.Replace("###", "Contains the"));
        }

        [Test]
        public void Code_gets_fixed_for_collection_field_with_various_phrases_(
                                                                           [ValueSource(nameof(WrongCollectionPhrases))] string startingPhrase,
                                                                           [Values("", " all", " all the")] string adjective)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### some comment.
    /// </summary>
    private List<string> m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", startingPhrase + adjective), Template.Replace("###", "Contains the"));
        }

        [TestCase("List for all subsets", "Contains the subsets")]
        [TestCase("List for all supersets", "Contains the supersets")]
        [TestCase("List for all sub-sets", "Contains the sub-sets")]
        [TestCase("List for all super-sets", "Contains the super-sets")]
        [TestCase("List of all subsets", "Contains the subsets")]
        [TestCase("List of all supersets", "Contains the supersets")]
        [TestCase("List of all sub-sets", "Contains the sub-sets")]
        [TestCase("List of all super-sets", "Contains the super-sets")]
        public void Code_gets_fixed_for_collection_field_with_subset_superset_phrases_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### within some data.
    /// </summary>
    private List<string> m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Some comment", "The some comment")]
        [TestCase("Shall indicate some comment", "The some comment")]
        [TestCase("Shall indicate UPPER CASE comment", "The UPPER CASE comment")]
        [TestCase("Specifies a specific value. The range is a value between 1 and 254", "The specific value. The range is a value between 1 and 254")]
        [TestCase("Specifies the specific value. A valid value is between 1 and 254", "The specific value. A valid value is between 1 and 254")]
        [TestCase("Gets or sets the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets or sets a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets or sets an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets or Sets the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets or Sets a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets or Sets an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get or set the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get or set a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get or set an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Gets an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Get an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Sets the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Sets a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Sets an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Set the remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Set a remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Set an remaining days in evaluation mode", "The remaining days in evaluation mode")]
        [TestCase("Returns a value for something", "The value for something")]
        [TestCase("Returns an value for something", "The value for something")]
        [TestCase("Returns the value for something", "The value for something")]
        [TestCase("Returns value for something", "The value for something")]
        [TestCase("Return a value for something", "The value for something")]
        [TestCase("Return an value for something", "The value for something")]
        [TestCase("Return the value for something", "The value for something")]
        [TestCase("Return value for something", "The value for something")]
        [TestCase("/Returns a value for something", "The value for something")] // typo
        [TestCase("/Returns an value for something", "The value for something")] // typo
        [TestCase("/Returns the value for something", "The value for something")] // typo
        [TestCase("/Returns value for something", "The value for something")] // typo
        [TestCase("/Return a value for something", "The value for something")] // typo
        [TestCase("/Return an value for something", "The value for something")] // typo
        [TestCase("/Return the value for something", "The value for something")] // typo
        [TestCase("/Return value for something", "The value for something")] // typo
        [TestCase("This value for something", "The value for something")]
        [TestCase("Indicates that this something", "The something")]
        [TestCase("Indicates, that this something", "The something")]
        [TestCase("Holds the something", "The something")]
        [TestCase("Holds a something", "The something")]
        [TestCase("Holds an something", "The something")]
        [TestCase("Defines a something", "The something")]
        [TestCase("Use this something", "The something")]
        public void Code_gets_fixed_for_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private object m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("dictionary mapping a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping from a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping from", "Contains the mapping information from")]
        [TestCase("Dictionary mapping from", "Contains the mapping information from")]
        [TestCase("dictionary mapping info a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping info a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping info from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping info from a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping info from", "Contains the mapping information from")]
        [TestCase("Dictionary mapping info from", "Contains the mapping information from")]
        [TestCase("dictionary mapping info", "Contains the mapping information from")]
        [TestCase("Dictionary mapping info", "Contains the mapping information from")]
        [TestCase("dictionary mapping information a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping information a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping information from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping information from a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping information from", "Contains the mapping information from")]
        [TestCase("Dictionary mapping information from", "Contains the mapping information from")]
        [TestCase("dictionary mapping information", "Contains the mapping information from")]
        [TestCase("Dictionary mapping information", "Contains the mapping information from")]
        [TestCase("dictionary mapping infos a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping infos a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping infos from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mapping infos from a", "Contains the mapping information from a")]
        [TestCase("dictionary mapping infos from", "Contains the mapping information from")]
        [TestCase("Dictionary mapping infos from", "Contains the mapping information from")]
        [TestCase("dictionary mapping infos", "Contains the mapping information from")]
        [TestCase("Dictionary mapping infos", "Contains the mapping information from")]
        [TestCase("dictionary mapping", "Contains the mapping information from")]
        [TestCase("Dictionary mapping", "Contains the mapping information from")]
        [TestCase("dictionary mappings a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings from a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings from", "Contains the mapping information from")]
        [TestCase("Dictionary mappings from", "Contains the mapping information from")]
        [TestCase("dictionary mappings info a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings info a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings info from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings info from a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings info from", "Contains the mapping information from")]
        [TestCase("Dictionary mappings info from", "Contains the mapping information from")]
        [TestCase("dictionary mappings info", "Contains the mapping information from")]
        [TestCase("Dictionary mappings info", "Contains the mapping information from")]
        [TestCase("dictionary mappings information a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings information a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings information from a", "Contains the mapping information from a")]
        [TestCase("Dictionary mappings information from a", "Contains the mapping information from a")]
        [TestCase("dictionary mappings information from", "Contains the mapping information from")]
        [TestCase("Dictionary mappings information from", "Contains the mapping information from")]
        [TestCase("dictionary mappings information", "Contains the mapping information from")]
        [TestCase("Dictionary mappings information", "Contains the mapping information from")]
        [TestCase("dictionary mappings", "Contains the mapping information from")]
        [TestCase("Dictionary mappings", "Contains the mapping information from")]
        public void Code_gets_fixed_for_dictionary_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ### A to B.
    /// </summary>
    private Dictionary<string, int> m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        protected override string GetDiagnosticId() => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2080_FieldSummaryDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2080_CodeFixProvider();

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
        [ExcludeFromCodeCoverage]
        private static HashSet<string> CreateWrongBooleanPhrases()
        {
            string[] booleanStarts =
                                     [
                                         "Flag", "A flag", "The flag", "Value", "A value", "The value",
                                         "Boolean", "A Boolean", "A boolean", "The Boolean", "The boolean", "Boolean value", "A Boolean value", "A boolean value", "The Boolean value", "The boolean value",
                                         "Bool", "Bool value", "A bool", "A bool value", "The bool", "The bool value",
                                         "Contains a value", "Contains a flag", "Contains the value", "Contains the flag",
                                         "Contains a boolean", "Contains a Boolean", "Contains a boolean value", "Contains the boolean value",
                                         "Contains a bool", "Contains a bool value", "Contains the bool value",
                                     ];
            string[] middles = ["indicating", "that indicates", "to indicate", "which indicates", "controlling", "that controls", "to control", "which controls"];
            string[] ends = ["if", "that", "whether", "whether or not"];

            var results = new HashSet<string>();

            foreach (var start in booleanStarts)
            {
                foreach (var middle in middles)
                {
                    var begin = string.Concat(start, " ", middle, " ");

                    foreach (var end in ends)
                    {
                        var phrase = begin + end;

                        results.Add(phrase);
                        results.Add(phrase.ToLowerCaseAt(0));
                    }
                }
            }

            string[] nonBooleanStarts =
                                        [
                                            "Control",
                                            "Controling", // typo
                                            "Controlling",
                                            "Controls",
                                            "Indicate",
                                            "Indicates",
                                            "Indicating",
                                            "Shall control",
                                            "Shall indicate",
                                            "Should control",
                                            "Should indicate",
                                            "To control",
                                            "To indicate",
                                            "Will control",
                                            "Will indicate",
                                            "Would control",
                                            "Would indicate",
                                        ];

            string[] commas = [string.Empty, ","];

            foreach (var start in nonBooleanStarts)
            {
                foreach (var comma in commas)
                {
                    var begin = string.Concat(start, comma, " ");

                    foreach (var end in ends)
                    {
                        var phrase = begin + end;

                        results.Add(phrase);
                        results.Add(phrase.ToLowerCaseAt(0));
                    }
                }
            }

            foreach (var phrase in new[] { "If", "When", "Whether" })
            {
                results.Add(phrase);
                results.Add(phrase.ToLowerCaseAt(0));
            }

            // those are allowed boolean terms
            results.Remove("Indicates whether");
            results.Remove("Indicates whether or not");
            results.Remove("indicates whether");
            results.Remove("indicates whether or not");

            return results;
        }
    }
}