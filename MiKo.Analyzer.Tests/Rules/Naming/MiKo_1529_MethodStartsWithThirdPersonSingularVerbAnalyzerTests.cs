using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_starting_with_infinite_verb() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [TestCase("Ends")]
        [TestCase("Starts")]
        [TestCase("Contains")]
        [TestCase("Extends")]
        [TestCase("Implements")]
        [TestCase("Is")]
        [TestCase("Matches")]
        [TestCase("Throws")]
        public void No_issue_is_reported_for_boolean_method_starting_with_(string verb) => No_issue_is_reported_for(@"
public class TestMe
{
    public bool " + verb + @"Something() { }
}
");

        [TestCase("AsSomething")]
        [TestCase("BytesCount")]
        [TestCase("ColumnsCount")]
        [TestCase("DevicesCount")]
        [TestCase("Do_stuff_with_resources")]
        [TestCase("MessagesCount")]
        [TestCase("ModulesCount")]
        [TestCase("NumbersCount")]
        [TestCase("PackagesCount")]
        [TestCase("ParametersCount")]
        [TestCase("ProjectsCount")]
        [TestCase("PropertiesCount")]
        [TestCase("WindowsCount")]
        public void No_issue_is_reported_for_method_(string verb) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + verb + @"() { }
}
");

        [TestCase("Contains")]
        [TestCase("Creates")]
        [TestCase("Ends")]
        [TestCase("Extends")]
        [TestCase("Implements")]
        [TestCase("Is")]
        [TestCase("Matches")]
        [TestCase("Starts")]
        [TestCase("Throws")]
        public void No_issue_is_reported_for_test_type_method_starting_with_3rd_person_singular_verb_(string verb) => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    public void " + verb + @"Something() { }
}
");

        [TestCase("Creates")]
        [TestCase("Contains")]
        [TestCase("Matches")]
        [TestCase("Throws")]
        public void No_issue_is_reported_for_test_method_starting_with_3rd_person_singular_verb_(string verb) => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void " + verb + @"Something() { }
}
");

        [Test]
        public void No_issue_is_reported_for_DependencyProperty_changed_callback() => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void IsWhateverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [TestCase("Contains")]
        [TestCase("Creates")]
        [TestCase("Ends")]
        [TestCase("Extends")]
        [TestCase("Implements")]
        [TestCase("Is")]
        [TestCase("Matches")]
        [TestCase("Starts")]
        [TestCase("Throws")]
        public void An_issue_is_reported_for_method_starting_with_3rd_person_singular_verb_(string verb) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + verb + @"Something() { }
}
");

        [TestCase("Creates", "Create")]
        public void Code_gets_fixed_for_method_with_3rd_person_singular_verb_(string originalName, string fixedName)
        {
            const string Template = """

                                    public class TestMe
                                    {
                                        public void ###Something() { }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1529_MethodStartsWithThirdPersonSingularVerbAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1529_CodeFixProvider();
    }
}