using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3208_UsingDirectivePrecededByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void An_issue_is_reported_for_multiple_usings_that_have_different_identifiers_(string namespace1, string namespace2) => An_issue_is_reported_for(@"
using " + namespace1 + @";
using " + namespace2 + @";

public class TestMe
{
}
");

        [TestCase("System", "NUnit")]
        [TestCase("System.IO", "NUnit.Framework")]
        [TestCase("Bla.BlaBlubb", "Blablubb")]
        [TestCase("BlaBlubb.Bla", "Bla")]
        public void Code_gets_fixed_for_multiple_usings_that_have_different_identifiers_(string namespace1, string namespace2)
        {
            var originalCode = @"
using " + namespace1 + @";
using " + namespace2 + @";

public class TestMe
{
}
";

            var fixedCode = @"
using " + namespace1 + @";

using " + namespace2 + @";

public class TestMe
{
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3208_UsingDirectivePrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3208_UsingDirectivePrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3208_CodeFixProvider();
    }
}