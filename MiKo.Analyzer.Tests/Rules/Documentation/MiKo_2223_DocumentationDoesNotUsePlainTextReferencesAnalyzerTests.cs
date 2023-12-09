using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_correctly_documented_method_with_hyperlink_anchor_and_descriptive_text([Values(@"<a href=""http://www.nunit.org/"">NUnit</a>")] string text) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_known_text_([Values("IntelliSense", "FxCop", "StyleCop", "SonarCube", "SonarQube", "CSharp", "VisualBasic", "NCrunch", "NCrunch's", "NCover", "PostSharp", "SonarLint", "ReSharper", "ASP.NET")] string text)
            => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_documented_method_with_file_extension() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_method_with_starting_number() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something regarding 1A.SomeNameSpace.SomeClass that is very important.
    /// </summary>
    public void DoSomething()
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

        protected override string GetDiagnosticId() => MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer();
    }
}