using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6008_UsingDirectivePrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_without_usings() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_single_using() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_usings_that_share_the_same_identifier() => No_issue_is_reported_for(@"
using System;
using System.IO;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_usings_with_name_aliases() => No_issue_is_reported_for(@"
using System;
using System.IO;

using File = System.IO.File;
using BlaBlubb = Bla.BlaBlubb;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_usings_that_share_the_same_identifier_and_are_part_of_AssemblyInfo_cs() => No_issue_is_reported_for(@"
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle(""Test title"")]
");

        [TestCase("System", "NUnit")]
        [TestCase("System.IO", "NUnit.Framework")]
        [TestCase("Bla.BlaBlubb", "Blablubb")]
        [TestCase("BlaBlubb.Bla", "Bla")]
        [TestCase("System", "File = System.IO.File")]
        [TestCase("File = System.IO.File", "System")]
        public void An_issue_is_reported_for_multiple_usings_that_have_different_identifiers_(string directive1, string directive2) => An_issue_is_reported_for(@"
using " + directive1 + @";
using " + directive2 + @";

public class TestMe
{
}
");

        [TestCase("System", "NUnit")]
        [TestCase("System.IO", "NUnit.Framework")]
        [TestCase("Bla.BlaBlubb", "Blablubb")]
        [TestCase("BlaBlubb.Bla", "Bla")]
        [TestCase("System", "File = System.IO.File")]
        [TestCase("File = System.IO.File", "System")]
        public void An_issue_is_reported_for_multiple_usings_that_have_different_identifiers_and_a_comment_in_between_(string directive1, string directive2) => An_issue_is_reported_for(@"
using " + directive1 + @";
// some comment
using " + directive2 + @";

public class TestMe
{
}
");

        [TestCase("System", "NUnit")]
        [TestCase("System.IO", "NUnit.Framework")]
        [TestCase("Bla.BlaBlubb", "Blablubb")]
        [TestCase("BlaBlubb.Bla", "Bla")]
        [TestCase("System", "File = System.IO.File")]
        [TestCase("File = System.IO.File", "System")]
        public void Code_gets_fixed_for_multiple_usings_that_have_different_identifiers_(string directive1, string directive2)
        {
            var originalCode = @"
using " + directive1 + @";
using " + directive2 + @";

public class TestMe
{
}
";

            var fixedCode = @"
using " + directive1 + @";

using " + directive2 + @";

public class TestMe
{
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("System", "NUnit")]
        [TestCase("System.IO", "NUnit.Framework")]
        [TestCase("Bla.BlaBlubb", "Blablubb")]
        [TestCase("BlaBlubb.Bla", "Bla")]
        [TestCase("System", "File = System.IO.File")]
        [TestCase("File = System.IO.File", "System")]
        public void Code_gets_fixed_for_multiple_usings_that_have_different_identifiers_and_a_comment_in_between_(string directive1, string directive2)
        {
            var originalCode = @"
using " + directive1 + @";
// some comment
using " + directive2 + @";

public class TestMe
{
}
";

            var fixedCode = @"
using " + directive1 + @";

// some comment
using " + directive2 + @";

public class TestMe
{
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6008_UsingDirectivePrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6008_UsingDirectivePrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6008_CodeFixProvider();
    }
}