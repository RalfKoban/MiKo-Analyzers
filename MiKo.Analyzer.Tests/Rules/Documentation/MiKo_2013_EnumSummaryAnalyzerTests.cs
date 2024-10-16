using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2013_EnumSummaryAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_enum_without_documentation() => No_issue_is_reported_for(@"
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_with_correct_phrase() => No_issue_is_reported_for(@"
/// <summary>
/// Defines values that specify something.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_with_correct_phrase_in_para_tag() => No_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Defines values that specify something.
/// </para>
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_empty_phrase() => An_issue_is_reported_for(@"
/// <summary>
///
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_wrong_phrase() => An_issue_is_reported_for(@"
/// <summary>
/// Some documentation.
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_wrong_phrase_in_para_tag() => An_issue_is_reported_for(@"
/// <summary>
/// <para>
/// Defines something.
/// </para>
/// </summary>
public enum TestMe
{
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
/// <summary>
/// Something.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_single_line()
        {
            const string OriginalCode = @"
/// <summary>Something to do.</summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_multiline()
        {
            const string OriginalCode = @"
/// <summary>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify Something
/// to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_seecref_multiline()
        {
            const string OriginalCode = @"
/// <summary>
/// <see cref=""TestMe""/>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify <see cref=""TestMe""/>
/// Something
/// to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_seecref_on_single_line()
        {
            const string OriginalCode = @"
/// <summary><see cref=""TestMe""/> Something to do.</summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify <see cref=""TestMe""/> Something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Contains")]
        [TestCase("Define")]
        [TestCase("Defined")]
        [TestCase("Defines")]
        [TestCase("Describe")]
        [TestCase("Described")]
        [TestCase("Describes")]
        [TestCase("Identify")]
        [TestCase("Identified")]
        [TestCase("Identifies")]
        [TestCase("Indicate")]
        [TestCase("Indicated")]
        [TestCase("Indicates")]
        [TestCase("Present")]
        [TestCase("Presents")]
        [TestCase("Provide")]
        [TestCase("Provides")]
        [TestCase("Represent")]
        [TestCase("Represents")]
        [TestCase("Specify")]
        [TestCase("Specified")]
        [TestCase("Specifies")]
        public void Code_gets_fixed_for_almost_correct_documentation_(string firstWord)
        {
            var originalCode = @"
/// <summary>
/// " + firstWord + @" something to do.
/// </summary>
public enum TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Defines values that specify something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("Declaration of", "")]
        [TestCase("Enum containing the", "the ")]
        [TestCase("Enum contains the", "the ")]
        [TestCase("Enum describes the", "the ")]
        [TestCase("Enum describing the", "the ")]
        [TestCase("Enum for", "")]
        [TestCase("Enum of", "")]
        [TestCase("Enum representing the", "the ")]
        [TestCase("Enum represents the", "the ")]
        [TestCase("Enum that contains", "")]
        [TestCase("Enum that describes", "")]
        [TestCase("Enum that represents", "")]
        [TestCase("Enum what", "what ")]
        [TestCase("Enum which contains", "")]
        [TestCase("Enum which describes", "")]
        [TestCase("Enum which represents", "")]
        [TestCase("Enumeration containing the", "the ")]
        [TestCase("Enumeration contains the", "the ")]
        [TestCase("Enumeration describes the", "the ")]
        [TestCase("Enumeration describing the", "the ")]
        [TestCase("Enumeration for", "")]
        [TestCase("Enumeration of", "")]
        [TestCase("Enumeration representing the", "the ")]
        [TestCase("Enumeration represents the", "the ")]
        [TestCase("Enumeration that contains", "")]
        [TestCase("Enumeration that describes", "")]
        [TestCase("Enumeration that represents", "")]
        [TestCase("Enumeration what", "what ")]
        [TestCase("Enumeration which contains", "")]
        [TestCase("Enumeration which describes", "")]
        [TestCase("Enumeration which represents", "")]
        [TestCase("Flagged enum for", "")]
        [TestCase("Flagged enumeration for", "")]
        [TestCase("Flags enum representing the", "the ")]
        [TestCase("Flags enum represents the", "the ")]
        [TestCase("Flags enumeration representing the", "the ")]
        [TestCase("Flags enumeration represents the", "the ")]
        [TestCase("State containing the", "the ")]
        [TestCase("State contains the", "the ")]
        [TestCase("State describes the", "the ")]
        [TestCase("State describing the", "the ")]
        [TestCase("State for", "")]
        [TestCase("State of", "")]
        [TestCase("State representing the", "the ")]
        [TestCase("State represents the", "the ")]
        [TestCase("State that contains", "")]
        [TestCase("State that describes", "")]
        [TestCase("State that represents", "")]
        [TestCase("State what", "what ")]
        [TestCase("State which contains", "")]
        [TestCase("State which describes", "")]
        [TestCase("State which represents", "")]
        [TestCase("Gets", "")]
        [TestCase("Sets", "")]
        [TestCase("Gets or sets", "")]
        [TestCase("Gets or Sets", "")]
        public void Code_gets_fixed_for_documentation_(string start, string fixedStart)
        {
            var originalCode = @"
/// <summary>
/// " + start + @" something to do.
/// </summary>
public enum TestMe
{
}
";

            var fixedCode = @"
/// <summary>
/// Defines values that specify " + fixedStart + @"something to do.
/// </summary>
public enum TestMe
{
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Message", "messages")]
        [TestCase("MessageEnum", "messages")]
        [TestCase("MessageKind", "messages")]
        [TestCase("MessageType", "messages")]
        [TestCase("MessageTypes", "messages")]
        [TestCase("MessageTypeKind", "messages")]
        [TestCase("MessageTypeKinds", "messages")]
        [TestCase("MessageTypeEnum", "messages")]
        [TestCase("MessageTypeEnums", "messages")]
        [TestCase("Direction", "directions")]
        [TestCase("DirectionKind", "directions")]
        [TestCase("ReferenceTypes", "references")]
        public void Code_gets_fixed_for_special_documentation_(string typeName, string fixedEnding)
        {
            var originalCode = @"
/// <summary>
/// Gets or Sets " + typeName + @"
/// </summary>
public enum " + typeName + @"
{
}
";

            var fixedCode = @"
/// <summary>
/// Defines values that specify the different kinds of " + fixedEnding + @"
/// </summary>
public enum " + typeName + @"
{
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Message", "messages")]
        [TestCase("MessageEnum", "messages")]
        [TestCase("MessageKind", "messages")]
        [TestCase("MessageType", "messages")]
        [TestCase("MessageTypes", "messages")]
        [TestCase("MessageTypeKind", "messages")]
        [TestCase("MessageTypeKinds", "messages")]
        [TestCase("MessageTypeEnum", "messages")]
        [TestCase("MessageTypeEnums", "messages")]
        [TestCase("Direction", "directions")]
        [TestCase("DirectionKind", "directions")]
        [TestCase("ReferenceTypes", "references")]
        public void Code_gets_fixed_for_sentenced_with_special_documentation_(string typeName, string fixedEnding)
        {
            var originalCode = @"
/// <summary>
/// Gets or Sets " + typeName + @".
/// </summary>
public enum " + typeName + @"
{
}
";

            var fixedCode = @"
/// <summary>
/// Defines values that specify the different kinds of " + fixedEnding + @".
/// </summary>
public enum " + typeName + @"
{
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Message", "messages")]
        [TestCase("MessageKind", "messages")]
        [TestCase("MessageType", "messages")]
        [TestCase("MessageDef", "message definitions")]
        public void Code_gets_fixed_for_sentenced_with_special_documentation_suffixed_with_Enum_(string typeName, string fixedEnding)
        {
            var originalCode = @"
/// <summary>
/// Gets or Sets " + typeName + @".
/// </summary>
public enum " + typeName + @"Enum
{
}
";

            var fixedCode = @"
/// <summary>
/// Defines values that specify the different kinds of " + fixedEnding + @".
/// </summary>
public enum " + typeName + @"Enum
{
}
";
            VerifyCSharpFix(originalCode, fixedCode);
        }

        // TODO RKN: Replace abbreviations such as in 'DataTypeDef'
        // TODO RKN: Replace preambles such as 'StateOf', 'TypeOf', 'KindOf', etc ('xyzOf'
        protected override string GetDiagnosticId() => MiKo_2013_EnumSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2013_EnumSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2013_CodeFixProvider();
    }
}