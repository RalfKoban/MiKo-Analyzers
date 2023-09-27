using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
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

        [TestCase("A flag indicating if some comment", "Indicates whether some comment")]
        [TestCase("A flag indicating that some comment", "Indicates whether some comment")]
        [TestCase("A flag indicating whether some comment", "Indicates whether some comment")]
        [TestCase("A flag that indicates if some comment", "Indicates whether some comment")]
        [TestCase("A flag that indicates that some comment", "Indicates whether some comment")]
        [TestCase("A flag that indicates whether some comment", "Indicates whether some comment")]
        [TestCase("A flag to indicate if some comment", "Indicates whether some comment")]
        [TestCase("A flag to indicate that some comment", "Indicates whether some comment")]
        [TestCase("A flag to indicate whether some comment", "Indicates whether some comment")]
        [TestCase("A flag which indicates if some comment", "Indicates whether some comment")]
        [TestCase("A flag which indicates that some comment", "Indicates whether some comment")]
        [TestCase("A flag which indicates whether some comment", "Indicates whether some comment")]
        [TestCase("Flag indicating if some comment", "Indicates whether some comment")]
        [TestCase("Flag indicating that some comment", "Indicates whether some comment")]
        [TestCase("Flag indicating whether some comment", "Indicates whether some comment")]
        [TestCase("Flag that indicates if some comment", "Indicates whether some comment")]
        [TestCase("Flag that indicates that some comment", "Indicates whether some comment")]
        [TestCase("Flag that indicates whether some comment", "Indicates whether some comment")]
        [TestCase("Flag to indicate if some comment", "Indicates whether some comment")]
        [TestCase("Flag to indicate that some comment", "Indicates whether some comment")]
        [TestCase("Flag to indicate whether some comment", "Indicates whether some comment")]
        [TestCase("Flag which indicates if some comment", "Indicates whether some comment")]
        [TestCase("Flag which indicates that some comment", "Indicates whether some comment")]
        [TestCase("Flag which indicates whether some comment", "Indicates whether some comment")]
        [TestCase("Indicates if some comment", "Indicates whether some comment")]
        [TestCase("Indicates that some comment", "Indicates whether some comment")]
        [TestCase("Indicating if some comment", "Indicates whether some comment")]
        [TestCase("Indicating that some comment", "Indicates whether some comment")]
        [TestCase("Indicating whether some comment", "Indicates whether some comment")]
        [TestCase("Shall indicate if some comment", "Indicates whether some comment")]
        [TestCase("Shall indicate that some comment", "Indicates whether some comment")]
        [TestCase("Shall indicate whether some comment", "Indicates whether some comment")]
        [TestCase("Should indicate if some comment", "Indicates whether some comment")]
        [TestCase("Should indicate that some comment", "Indicates whether some comment")]
        [TestCase("Should indicate whether some comment", "Indicates whether some comment")]
        [TestCase("Some comment", "Indicates whether some comment")]
        [TestCase("The flag that indicates if some comment", "Indicates whether some comment")]
        [TestCase("The flag that indicates that some comment", "Indicates whether some comment")]
        [TestCase("The flag that indicates whether some comment", "Indicates whether some comment")]
        [TestCase("The flag to indicate if some comment", "Indicates whether some comment")]
        [TestCase("The flag to indicate that some comment", "Indicates whether some comment")]
        [TestCase("The flag to indicate whether some comment", "Indicates whether some comment")]
        [TestCase("The flag which indicates if some comment", "Indicates whether some comment")]
        [TestCase("The flag which indicates that some comment", "Indicates whether some comment")]
        [TestCase("The flag which indicates whether some comment", "Indicates whether some comment")]
        [TestCase("To indicate if some comment", "Indicates whether some comment")]
        [TestCase("To indicate that some comment", "Indicates whether some comment")]
        [TestCase("To indicate whether some comment", "Indicates whether some comment")]
        [TestCase("Will indicate if some comment", "Indicates whether some comment")]
        [TestCase("Will indicate that some comment", "Indicates whether some comment")]
        [TestCase("Will indicate whether some comment", "Indicates whether some comment")]
        [TestCase("Would indicate if some comment", "Indicates whether some comment")]
        [TestCase("Would indicate that some comment", "Indicates whether some comment")]
        [TestCase("Would indicate whether some comment", "Indicates whether some comment")]
        public void Code_gets_fixed_for_non_constant_boolean_field_(string originalComment, string fixedComment)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// ###.
    /// </summary>
    private bool m_field;
}
";

            VerifyCSharpFix(Template.Replace("###", originalComment), Template.Replace("###", fixedComment));
        }

        [TestCase("A Guid of some comment", "The unique identifier for some comment")]
        [TestCase("A GUID of some comment", "The unique identifier for some comment")]
        [TestCase("A unique identifier for some comment", "The unique identifier for some comment")]
        [TestCase("An unique identifier for some comment", "The unique identifier for some comment")]
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
    }
}