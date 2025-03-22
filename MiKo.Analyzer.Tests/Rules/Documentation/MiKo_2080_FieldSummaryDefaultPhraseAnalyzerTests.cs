using System;
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

#if NCRUNCH

        [OneTimeSetUp]
        public static void PrepareTestEnvironment() => MiKo_2080_CodeFixProvider.LoadData();

#else

        [OneTimeTearDown]
        public static void CleanupTestEnvironment() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

#endif

        [Test]
        public void No_issue_is_reported_for_uncommented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_enumerable_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_boolean_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Guid_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_const_field() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_commented_field_([Values("Bla bla.", "Contains something.", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_commented_enumerable_field_([Values("Bla bla", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_commented_boolean_field_([Values("Bla bla", "The field", "Contains something.")] string comment) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_DependencyProperty_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_RoutedEvent_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_constant_boolean_field() => No_issue_is_reported_for(@"
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

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_constant_boolean_field_(
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
        public void Code_gets_fixed_for_non_constant_boolean_field_([ValueSource(nameof(WrongBooleanPhrases))] string originalComment)
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
        public void Code_gets_fixed_for_nullable_non_constant_boolean_field_([ValueSource(nameof(WrongBooleanPhrases))] string originalComment)
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

        [TestCase("A cache for")]
        [TestCase("A Cache for")]
        [TestCase("A collection for")]
        [TestCase("A collection of")]
        [TestCase("A collection that is holding")]
        [TestCase("A collection that holds")]
        [TestCase("A collection which holds")]
        [TestCase("A collection which is holding")]
        [TestCase("A collection holding")]
        [TestCase("A collection that is containing")]
        [TestCase("A collection that contains")]
        [TestCase("A collection which contains")]
        [TestCase("A collection which is containing")]
        [TestCase("A collection containing")]
        [TestCase("A collection that is storing")]
        [TestCase("A collection that stores")]
        [TestCase("A collection which stores")]
        [TestCase("A collection which is storing")]
        [TestCase("A collection storing")]
        [TestCase("A dictionary for")]
        [TestCase("A dictionary of")]
        [TestCase("A list for")]
        [TestCase("A list of")]
        [TestCase("An array for")]
        [TestCase("An array of")]
        [TestCase("An array that holds")]
        [TestCase("An array which holds")]
        [TestCase("Array for")]
        [TestCase("Array holding")]
        [TestCase("Array of")]
        [TestCase("Array that holds")]
        [TestCase("Array which holds")]
        [TestCase("Cache for")]
        [TestCase("Containing")]
        [TestCase("Collection for")]
        [TestCase("Collection holding")]
        [TestCase("Collection of")]
        [TestCase("Collection that holds")]
        [TestCase("Collection that is holding")]
        [TestCase("Collection which holds")]
        [TestCase("Collection which is holding")]
        [TestCase("Dictionary for")]
        [TestCase("Dictionary of")]
        [TestCase("Holding")]
        [TestCase("Holds")]
        [TestCase("List for")]
        [TestCase("List of")]
        [TestCase("Stores")]
        [TestCase("Storing")]
        [TestCase("The collection that holds")]
        [TestCase("The collection which holds")]
        [TestCase("This cache holds")]
        [TestCase("This collection holds")]
        [TestCase("This dictionary holds")]
        [TestCase("This is a array for")]
        [TestCase("This is a array of")]
        [TestCase("This is a cache for")]
        [TestCase("This is a cache of")]
        [TestCase("This is a dictionary for")]
        [TestCase("This is a dictionary of")]
        [TestCase("This is a list for")]
        [TestCase("This is a list of")]
        [TestCase("This list holds")]
        public void Code_gets_fixed_for_collection_field_(string originalComment)
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

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", "Contains the"));
        }

        [TestCase("List for all subsets", "Contains the subsets")]
        [TestCase("List for all supersets", "Contains the supersets")]
        [TestCase("List for all sub-sets", "Contains the sub-sets")]
        [TestCase("List for all super-sets", "Contains the super-sets")]
        [TestCase("List of all subsets", "Contains the subsets")]
        [TestCase("List of all supersets", "Contains the supersets")]
        [TestCase("List of all sub-sets", "Contains the sub-sets")]
        [TestCase("List of all super-sets", "Contains the super-sets")]
        public void Code_gets_fixed_for_collection_field_(string originalComment, string fixedComment)
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
        public void Code_gets_fixed_for_normal_field_(string originalComment, string fixedComment)
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
                                            "Controls",
                                            "Indicates",
                                            "Controling", // typo
                                            "Controlling",
                                            "Indicating",
                                            "Indicates",
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
                    }
                }
            }

            // those are allowed boolean terms
            results.Remove("Indicates whether");
            results.Remove("Indicates whether or not");

            return results;
        }
    }
}