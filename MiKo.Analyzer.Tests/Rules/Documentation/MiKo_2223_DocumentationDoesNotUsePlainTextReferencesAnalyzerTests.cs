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
                                                              "IntelliSense",
                                                              "FxCop",
                                                              "StyleCop",
                                                              "SonarCube",
                                                              "SonarQube",
                                                              "CSharp",
                                                              "VisualBasic",
                                                              "NCrunch",
                                                              "NCrunch's",
                                                              "NCover",
                                                              "PostSharp",
                                                              "SonarLint",
                                                              "ReSharper",
                                                              "ASP.NET",
                                                              "Microsoft",
                                                              "Outlook",
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
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_para() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_see_cref() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_upper_case_only_abbreviation() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_upper_case_only_abbreviation_in_plural_form() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_upper_case_only_abbreviation_in_genitive_case() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_upper_case_only_abbreviation_in_past_tense_case() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_upper_case_only_abbreviation_and_additional_information() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_Guid_(string value) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_hyperlink_([Values("http", "https", "ftp", "ftps")] string preamble) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_hyperlink_anchor_and_descriptive_text_([Values(@"<a href=""http://www.nunit.org/"">NUnit</a>")] string text) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_fully_qualified_path() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_keyboard_shortcut() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_known_text_([ValueSource(nameof(WellknownWords))] string text) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_combined_text_([Values("Undo/Redo", "XYZ1234:Method", "PublicKeyToken=1234")] string text) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_compiler_warning_([Values("CS0012", "CS0067")] string warning) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_file_extension_in_quotes() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_file_extension_not_in_quotes() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_hash() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_starting_number() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_starting_number_as_pseudo_namespace_or_type_name() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_(string example) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_exclamation_mark() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_comment_with_e_g() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something, e.g. that is very important, on stuff.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_para() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_at_end_of_sentence() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_embraced_method_name() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_single_quoted_method_name() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_quoted_method_name() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_embraced_and_quoted_method_name() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_reference_to_default_typeparam() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_starting_underscore_number() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_braces() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_on_interface() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_on_interface_with_array_return_type() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_on_interface_with_generic_return_value_that_contains_an_array() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_on_interface_with_generic_array_return_type() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_parameter() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_([ValueSource(nameof(NonCompoundWords))] string type) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_(
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
        public void Code_gets_fixed_for_incorrectly_documented_method_with_([ValueSource(nameof(NonCompoundWords))] string type)
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
        public void Code_gets_fixed_for_incorrectly_documented_method_with_(
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
        public void Code_gets_fixed_for_incorrectly_documented_method_ending_with_(
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
        public void Code_gets_fixed_for_incorrectly_documented_method_on_single_line_with_(
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
        public void Code_gets_fixed_for_incorrectly_documented_method_on_single_line_ending_with_(
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
        public void Code_gets_fixed_for_incorrectly_documented_method_with_type_(string originalName, string fixedName)
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

        protected override string GetDiagnosticId() => MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2223_CodeFixProvider();
    }
}