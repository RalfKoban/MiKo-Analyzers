using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags =
            {
                "example",
                "exception",
                "note",
                "overloads",
                "para",
                "param",
                "permission",
                "remarks",
                "returns",
                "summary",
                "typeparam",
                "value",
            };

        private static readonly string[] WellknownFileExtensions =
            {
                ".bmp",
                ".gif",
                ".png",
                ".jpg",
                ".jpeg",
                ".htm",
                ".html",
                ".xaml",
                ".xml",
                ".cs",
            };

        private static readonly char[] LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        private static readonly char[] UpperCaseLetters = "abcdefghijklmnopqrstuvwxyz".ToUpperInvariant().ToCharArray();

        [Test, Combinatorial]
        public void No_issue_is_reported_for_documentation_starting_with_upper_case_after_dot_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(UpperCaseLetters))] char startingChar) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation. " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_documentation_starting_with_upper_case_after_dot_in_para_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(UpperCaseLetters))] char startingChar) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation.
/// <para>" + startingChar + @" something.</para>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_documentation_starting_with_upper_case_after_dot_in_para_with_line_break_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(UpperCaseLetters))] char startingChar) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation.
/// <para>
/// " + startingChar + @" something.
/// </para>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_documentation_starting_with_lower_case_after_dot_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(LowerCaseLetters))] char startingChar) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation. " + startingChar + @" something.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_documentation_starting_with_lower_case_after_dot_in_para_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(LowerCaseLetters))] char startingChar) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation.
/// <para>" + startingChar + @" something.</para>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_documentation_starting_with_lower_case_after_dot_in_para_with_line_break_in([ValueSource(nameof(XmlTags))] string xmlTag, [ValueSource(nameof(LowerCaseLetters))] char startingChar) => An_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Documentation.
/// <para>
/// " + startingChar + @" something.
/// </para>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_empty_documentation([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @" />
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_nested_XML_documentation([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <some />
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_nested_XML_documentation_with_hyperlink([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <a href=""https://docs.microsoft.com/en-us/dotnet/framework/mef/index"">Managed Extensibility Framework (MEF)</a>.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_nested_XML_documentation_with_code_source_to_file([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// The following example demonstrates its usage.
/// <code source=""..\MiKo Aspects Samples\MiscSamples.cs"" region=""Required API usage"" lang=""C#"" />
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_well_known_abbreviation_in_XML_documentation([ValueSource(nameof(XmlTags))] string xmlTag, [Values("i.e.", "e.g.")] string abbreviation) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Something " + abbreviation + @" whatever.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_well_known_file_extension_in_code_block_in_XML_documentation(
                                                                                                    [ValueSource(nameof(XmlTags))] string xmlTag,
                                                                                                    [ValueSource(nameof(WellknownFileExtensions))] string fileExtension,
                                                                                                    [Values("c", "code")] string codeBlock)
            => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Some <" + codeBlock + ">file" + fileExtension + "</" + codeBlock + @"> extension.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_well_known_file_extension_not_within_code_block_in_XML_documentation(
                                                                                                            [ValueSource(nameof(XmlTags))] string xmlTag,
                                                                                                            [ValueSource(nameof(WellknownFileExtensions))] string fileExtension)
            => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Some file" + fileExtension + @" extension.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_code_in_code_block_para_tag_in_XML_documentation([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// <para>
/// Some example:
/// <code>
/// if (x)
///   return y; // that is e.g. a .png file
/// </code>
/// </para>
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_code_in_code_block_in_list_description_in_XML_documentation() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the data value is a specific file extension.
/// This class cannot be inherited.
/// </summary>
/// <remarks>
/// The default extensions - that is when no other extensions have been specified - are:
/// <list type=""bullet"">
/// <item><description><c>.gif</c></description></item>
/// <item><description><c>.jpg</c></description></item>
/// <item><description><c>.jpeg</c></description></item>
/// <item><description><c>.png</c></description></item>
/// </list>
/// <para />
/// <note type=""important"">
/// You need to apply the <see cref=""ValidateArgumentsAttribute""/> aspect to the owning class, as otherwise this marker will not do anything.
/// </note>
/// </remarks>
public sealed class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_nested_HTML_tag_in_XML_documentation([ValueSource(nameof(XmlTags))] string xmlTag) => No_issue_is_reported_for(@"
/// <" + xmlTag + @">
/// Some <b>not</b> so important text.
/// </" + xmlTag + @">
public sealed class TestMe { }
");

        protected override string GetDiagnosticId() => MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer();
    }
}