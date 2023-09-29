using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongBooleanPhrases = CreateWrongBooleanPhrases().Distinct().ToArray();

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
        public void No_issue_is_reported_for_correctly_commented_enumerable_field_([Values("Contains the", "The")] string comment) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @" data for the field.
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

        [TestCase("Some comment", "Contains some comment")]
        [TestCase("List of some comment", "Contains some comment")]
        [TestCase("A list of some comment", "Contains some comment")]
        [TestCase("Cache for some comment", "Contains some comment")]
        [TestCase("A Cache for some comment", "Contains some comment")]
        [TestCase("A cache for some comment", "Contains some comment")]
        [TestCase("Stores some comment", "Contains some comment")]
        public void Code_gets_fixed_for_collection_field_(string originalComment, string fixedComment)
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

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("Some comment", "The some comment")]
        [TestCase("Shall indicate some comment", "The some comment")]
        [TestCase("Specifies a specific value. The range is a value between 1 and 254", "The specific value. The range is a value between 1 and 254")]
        [TestCase("Specifies the specific value. A valid value is between 1 and 254", "The specific value. A valid value is between 1 and 254")]
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

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateWrongBooleanPhrases()
        {
            var starts = new[]
                         {
                             "Flag", "A flag", "The flag", "Value", "A value", "The value",
                             "Boolean", "A Boolean", "A boolean", "The Boolean", "The boolean", "Boolean value", "A Boolean value", "A boolean value", "The Boolean value", "The boolean value",
                             "Bool", "Bool value", "A bool", "A bool value", "The bool", "The bool value",
                             "Contains a value", "Contains a flag", "Contains the value", "Contains the flag",
                             "Contains a boolean", "Contains a Boolean", "Contains a boolean value", "Contains the boolean value",
                             "Contains a bool", "Contains a bool value", "Contains the bool value",
                         };
            var middles = new[] { "indicating", "that indicates", "to indicate", "which indicates" };
            var ends = new[] { "if", "that", "whether" };

            foreach (var start in starts)
            {
                foreach (var middle in middles)
                {
                    foreach (var end in ends)
                    {
                        var phrase = $"{start} {middle} {end}";
                        yield return phrase;
                        yield return phrase.ToLowerCaseAt(0);
                    }
                }
            }

            yield return "Indicates if";
            yield return "Indicates that";
            yield return "Indicating if";
            yield return "Indicating that";
            yield return "Indicating whether";
            yield return "Shall indicate if";
            yield return "Shall indicate that";
            yield return "Shall indicate whether";
            yield return "Should indicate if";
            yield return "Should indicate that";
            yield return "Should indicate whether";
            yield return "To indicate if";
            yield return "To indicate that";
            yield return "To indicate whether";
            yield return "Will indicate if";
            yield return "Will indicate that";
            yield return "Will indicate whether";
            yield return "Would indicate if";
            yield return "Would indicate that";
            yield return "Would indicate whether";
        }
    }
}