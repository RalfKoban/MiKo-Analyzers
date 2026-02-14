using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WellknownWords =
                                                          [
                                                              "ASP.NET",
                                                              "CSharp",
                                                              "FxCop",
                                                              "IntelliSense",
                                                              "Microsoft",
                                                              "MSTest",
                                                              "NCover",
                                                              "NCrunch",
                                                              "NCrunch's",
                                                              "NUnit",
                                                              "Outlook",
                                                              "PostSharp",
                                                              "ReSharper",
                                                              "SonarCube",
                                                              "SonarLint",
                                                              "SonarQube",
                                                              "StyleCop",
                                                              "VisualBasic",
                                                              "xUnit",
                                                          ];

        private static readonly string[] NonCompoundWords =
                                                            [
                                                                 "bool",
                                                                 "byte",
                                                                 "char",
                                                                 "float",
                                                                 "int",
                                                                 "string",
                                                                 "uint",
                                                                 "ushort",
                                                                 "ulong",
                                                                 nameof(String),
                                                                 nameof(Int16),
                                                                 nameof(Int32),
                                                                 nameof(Int64),
                                                                 nameof(UInt16),
                                                                 nameof(UInt32),
                                                                 nameof(UInt64),
                                                                 nameof(Single),
                                                                 nameof(Double),
                                                                 nameof(Boolean),
                                                                 nameof(Byte),
                                                                 nameof(Char),
                                                                 nameof(Type),
                                                            ];

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_without_plain_text_references() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_without_plain_text_references_inside_para_tag() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_see_cref_tag() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside <see cref=""DoSomething""/> that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_uppercase_abbreviation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside UML that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_uppercase_abbreviation_in_plural_form() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside UIs that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_uppercase_abbreviation_in_genitive_case() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside UI's parts that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_uppercase_abbreviation_in_past_tense() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside MEFed parts that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_uppercase_abbreviation_and_hyphenated_suffix() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside UML-whatever that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [TestCase("{B19F1C23-57F6-4a4E-aa69-5EE303F5184B}")]
        [TestCase("B19F1C23-57F6-4a4E-aa69-5EE303F5184B")]
        [TestCase("B19F1C2357F64a4Eaa695EE303F5184B")]
        public void No_issue_is_reported_for_method_with_Guid_(string value) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside " + value + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_URL_([Values("http", "https", "ftp", "ftps")] string preamble) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + preamble + @":\\www.SomeWebSite.com that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_HTML_anchor_tag_([Values(@"<a href=""http://www.nunit.org/"">NUnit</a>")] string text) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + text + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_file_path() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding C:\SomeDirectory\SomeFile.txt that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_keyboard_shortcut() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding CTRL+X that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_wellknown_product_name_([ValueSource(nameof(WellknownWords))] string text) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + text + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_special_combined_syntax_([Values("Undo/Redo", "XYZ1234:Method", "PublicKeyToken=1234")] string text) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + text + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_compiler_diagnostic_code_([Values("CS0012", "CS0067")] string warning) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + warning + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_wildcard_file_pattern_in_quotes() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding '*.ZipFile' that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_wildcard_file_pattern() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding *.txt that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_hash_delimited_text() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding '#SomeText#' that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_identifier_starting_with_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding '123_SomeText' that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_qualified_name_starting_with_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding 1A.SomeNameSpace.SomeClass and 2B.SomeInterface that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [TestCase("e.g")]
        [TestCase("E.g")]
        [TestCase("E.G")]
        [TestCase("e.g.")]
        [TestCase("E.g.")]
        [TestCase("E.G.")]
        [TestCase("i.e")]
        [TestCase("I.e")]
        [TestCase("I.E")]
        [TestCase("i.e.")]
        [TestCase("I.e.")]
        [TestCase("I.E.")]
        [TestCase("etc.")]
        [TestCase("( such as nothing etc.)")]
        [TestCase("(such as anything, nothing, etc.)")]
        [TestCase("(such as anything, nothing, etc.). So")]
        public void No_issue_is_reported_for_method_with_abbreviation_(string example) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding " + example + @" that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_exclamation_mark_in_text() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding Some!Text that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_para_phrase() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    ///   This option affects the way the generated SFX runs. By default it is
    ///   false.  When you set it to true,...
    /// </para>
    /// </summary>
    void DoSomething();
}
");

        [Test]
        public void No_issue_is_reported_for_file_with_extension_([Values("zip", "exe", "pdf", "docx")] string extension) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Can be SomeFile." + extension + @"
    /// </summary>
    void DoSomething();
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_([Values("e.g.", "e.g", "i.e.", "i.e", "p.ex.", "p.ex")] string example) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something, " + example + @" that is very important, on stuff.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_empty_string() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <returns>
    /// A <see cref=""string""/> that contains something, or the empty string if nothing is found.
    /// </returns>
    public string DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_empty_string_replacement() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <returns>
    /// A <see cref=""string""/> that contains something, or the <see cref=""string.Empty""/> string ("""") if nothing is found.
    /// </returns>
    public string DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_method_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something inside SomeMethodName that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_method_name_inside_para_tag() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside SomeMethodName that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_method_name_at_end_of_sentence() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside SomeMethodName.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parenthesized_method_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside (SomeMethodName) that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_single_quoted_method_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside 'SomeMethodName' that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_double_quoted_method_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside ""SomeMethodName"" that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_quoted_and_parenthesized_method_name() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something inside ""(SomeMethodName)"" that is very important.
    /// </para>
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_default_expression() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <para>
    /// Does something with 'default(TItem)' that is very important.
    /// </para>
    /// </summary>
    public void DoSomething<TItem>()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_identifier_starting_with_underscore() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding _123_SomeText that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_method_invocation() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding object.ToString() that is very important.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_interface_method_with_plain_text_method_invocation() => An_issue_is_reported_for(@"
using System;

public interface TestMe
{
    /// <summary>
    /// Does something regarding object.ToString() that is very important.
    /// </summary>
    void DoSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_interface_method_with_array_return_type_and_plain_text_reference() => An_issue_is_reported_for(@"
using System;

public interface TestMe
{
    /// <summary>
    /// Does something regarding object.ToString() that is very important.
    /// </summary>
    byte[] DoSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_interface_method_with_generic_array_return_type_and_plain_text_reference() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public interface TestMe
{
    /// <summary>
    /// Does something regarding object.ToString() that is very important.
    /// </summary>
    IEnumerable<byte[]> DoSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_interface_method_with_array_of_generic_return_type_and_plain_text_reference() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public interface TestMe
{
    /// <summary>
    /// Does something regarding object.ToString() that is very important.
    /// </summary>
    IEnumerable<int>[] DoSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_parameter_member_access() => An_issue_is_reported_for(@"
using System;
using System.IO;

public class TestMe
{
    /// <summary>
    /// You may want to call stream.Seek() before.
    /// </summary>
    public void DoSomething(Stream stream)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_type_name_([ValueSource(nameof(NonCompoundWords))] string type) => An_issue_is_reported_for(@"
using System;
using System.IO;

public class TestMe
{
    /// <summary>
    /// Does something with " + type + @" to see.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_plain_text_member_name_(
                                                                             [Values("string", "String", "Sring", "sring", "Sting", "sting")] string type,
                                                                             [Values("Empty", "empty", "Empy", "empy", "Emtpy", "emtpy")] string property)
            => An_issue_is_reported_for(@"
using System;
using System.IO;

public class TestMe
{
    /// <summary>
    /// Does something with " + type + "." + property + @" to see.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_plain_text_type_name_([ValueSource(nameof(NonCompoundWords))] string type)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// Does something with ### to see.
                                        /// </summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type), Template.Replace("###", "<see cref=\"" + type + "\"/>"));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_member_name_(
                                                            [Values("string", "String", "Sring", "sring", "Sting", "sting")] string type,
                                                            [Values("Empty", "empty", "Empy", "empy", "Emtpy", "emtpy")] string property)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// Does something with ### to see.
                                        /// </summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type + "." + property), Template.Replace("###", """<see cref="String.Empty"/>"""));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_type_name_at_end_of_sentence_([ValueSource(nameof(NonCompoundWords))] string type)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// Does something with ###
                                        /// </summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type), Template.Replace("###", "<see cref=\"" + type + "\"/>"));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_member_at_end_of_sentence_(
                                                                          [Values("string", "String", "Sring", "sring", "Sting", "sting")] string type,
                                                                          [Values("Empty", "empty", "Empy", "empy", "Emtpy", "emtpy")] string property)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// Does something with ###
                                        /// </summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type + "." + property), Template.Replace("###", """<see cref="String.Empty"/>"""));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_type_name_before_period_([ValueSource(nameof(NonCompoundWords))] string type)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>
                                        /// Does something with ###.
                                        /// </summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type), Template.Replace("###", "<see cref=\"" + type + "\"/>"));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_member_on_single_line_comment_(
                                                                              [Values("string", "String", "Sring", "sring", "Sting", "sting")] string type,
                                                                              [Values("Empty", "empty", "Empy", "empy", "Emtpy", "emtpy")] string property)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>Does something with ### to see.</summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type + "." + property), Template.Replace("###", """<see cref="String.Empty"/>"""));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_member_at_end_of_single_line_comment_(
                                                                                     [Values("string", "String", "Sring", "sring", "Sting", "sting")] string type,
                                                                                     [Values("Empty", "empty", "Empy", "empy", "Emtpy", "emtpy")] string property)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>Does something with ###</summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", type + "." + property), Template.Replace("###", """<see cref="String.Empty"/>"""));
        }

        [TestCase("TestMe", "TestMe")]
        public void Code_gets_fixed_for_plain_text_class_name_(string originalName, string fixedName)
        {
            const string Template = """

                                    using System;
                                    using System.IO;

                                    public class TestMe
                                    {
                                        /// <summary>Does something with ###</summary>
                                        public void DoSomething()
                                        {
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", "<see cref=\"" + fixedName + "\"/>"));
        }

        [TestCase("Does something with charts but not with a char or any other character.", """Does something with charts but not with a <see cref="char"/> or any other character.""")]
        [TestCase("Does something with charts but not with a char", """Does something with charts but not with a <see cref="char"/>""")]
        [TestCase("Does something with hints but not with an int or anything else.", """Does something with hints but not with an <see cref="int"/> or anything else.""")]
        [TestCase("Does something with hints but not with an int", """Does something with hints but not with an <see cref="int"/>""")]
        [TestCase("Does something with substrings but not with a string or any other substring.", """Does something with substrings but not with a <see cref="string"/> or any other substring.""")]
        [TestCase("Does something with substrings but not with a string", """Does something with substrings but not with a <see cref="string"/>""")]
        public void Code_gets_fixed_but_does_not_adjust_parts_in_words_(string originalText, string fixedText)
        {
            const string Template = """

                                     using System;
                                     using System.IO;

                                     public class TestMe
                                     {
                                         /// <summary>###</summary>
                                         public void DoSomething()
                                         {
                                         }
                                     }

                                     """;

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [Test]
        public void Code_gets_fixed_for_plain_text_string_in_empty_string_comment()
        {
            const string OriginalCode = """
                                        using System;

                                        public class TestMe
                                        {
                                            /// <returns>
                                            /// A string that contains something, or the <see cref="string.Empty"/> string ("") if nothing is found.
                                            /// </returns>
                                            public string DoSomething()
                                            {
                                            }
                                        }
                                        """;

            const string FixedCode = """
                                     using System;

                                     public class TestMe
                                     {
                                         /// <returns>
                                         /// A <see cref="string"/> that contains something, or the <see cref="string.Empty"/> string ("") if nothing is found.
                                         /// </returns>
                                         public string DoSomething()
                                         {
                                         }
                                     }
                                     """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2223_CodeFixProvider();
    }
}