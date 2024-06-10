using System.Collections.Generic;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2218_DocumentationShouldNotContainUsedToAnalyzerTests : CodeFixVerifier
    {
        private static readonly Dictionary<string, string> Map = new()
                                                                     {
                                                                         { "can be used in order to", "allows to" },
                                                                         { "can be used to", "allows to" },
                                                                         { "could be used in order to", "allows to" },
                                                                         { "could be used to", "allows to" },
                                                                         { "are expected to be used to", "allow to" },
                                                                         { "are intended to be used to", "allow to" },
                                                                         { "are meant to be used to", "allow to" },
                                                                         { "are primarily expected to be used to", "allow to" },
                                                                         { "are primarily intended to be used to", "allow to" },
                                                                         { "are primarily meant to be used to", "allow to" },
                                                                         { "is expected to be used to", "allows to" },
                                                                         { "is intended to be used to", "allows to" },
                                                                         { "is meant to be used to", "allows to" },
                                                                         { "is primarily expected to be used to", "allows to" },
                                                                         { "is primarily intended to be used to", "allows to" },
                                                                         { "is primarily meant to be used to", "allows to" },
                                                                         { "may be used to", "allows to" },
                                                                         { "might be used to", "allows to" },

                                                                         // parts starting with 'that'
                                                                         { "that can be used in order to", "that allows to" },
                                                                         { "that can be used to", "that allows to" },
                                                                         { "that could be used in order to", "that allows to" },
                                                                         { "that could be used to", "that allows to" },
                                                                         { "that is used to", "to" },
                                                                         { "that is used in order to", "to" },
                                                                         { "that it is used to", "to" },
                                                                         { "that it is used in order to", "to" },
                                                                         { "that are used to", "to" },
                                                                         { "that are used in order to", "to" },
                                                                         { "that shall be used to", "to" },
                                                                         { "that shall be used in order to", "to" },
                                                                         { "that should be used to", "to" },
                                                                         { "that should be used in order to", "to" },
                                                                         { "that should currently be used to", "to" },
                                                                         { "that should currently be used in order to", "to" },
                                                                         { "that will be used to", "to" },
                                                                         { "that will be used in order to", "to" },
                                                                         { "that would be used to", "to" },
                                                                         { "that would be used in order to", "to" },

                                                                         // parts starting with 'which'
                                                                         { "which can be used in order to", "which allows to" },
                                                                         { "which can be used to", "which allows to" },
                                                                         { "which could be used in order to", "which allows to" },
                                                                         { "which could be used to", "which allows to" },
                                                                         { "which is used to", "to" },
                                                                         { "which is used in order to", "to" },
                                                                         { "which are used to", "to" },
                                                                         { "which are used in order to", "to" },
                                                                         { "which shall be used to", "to" },
                                                                         { "which shall be used in order to", "to" },
                                                                         { "which should be used to", "to" },
                                                                         { "which should be used in order to", "to" },
                                                                         { "which should currently be used to", "to" },
                                                                         { "which should currently be used in order to", "to" },
                                                                         { "which will be used to", "to" },
                                                                         { "which will be used in order to", "to" },
                                                                         { "which would be used to", "to" },
                                                                         { "which would be used in order to", "to" },

                                                                         // TODO RKN:

                                                                         // can be used in case of -> allow
                                                                         // can be used in case
                                                                         // to be used in case of -> allow
                                                                         // to be used in case

                                                                         // -> suitable for /  made to work with
                                                                         { "are expected to be used by", "are suitable for" },
                                                                         { "are expected to be used for", "are suitable for" },
                                                                         { "are expected to be used in combination with", "are made to work with" },
                                                                         { "are expected to be used in conjunction with", "are made to work with" },
                                                                         { "are expected to be used internally", "are suitable for internal use" },
                                                                         { "are expected to be used in", "are suitable for" },
                                                                         { "are intended to be used by", "are suitable for" },
                                                                         { "are intended to be used for", "are suitable for" },
                                                                         { "are intended to be used in combination with", "are made to work with" },
                                                                         { "are intended to be used in conjunction with", "are made to work with" },
                                                                         { "are intended to be used internally", "are suitable for internal use" },
                                                                         { "are intended to be used in", "are suitable for" },
                                                                         { "are meant to be used by", "are suitable for" },
                                                                         { "are meant to be used in combination with", "are made to work with" },
                                                                         { "are meant to be used in conjunction with", "are made to work with" },
                                                                         { "are meant to be used internally", "are suitable for internal use" },
                                                                         { "are meant to be used in", "are suitable for" },
                                                                         { "are primarily expected to be used by", "are suitable for" },
                                                                         { "are primarily expected to be used for", "are suitable for" },
                                                                         { "are primarily expected to be used in combination with", "are made to work with" },
                                                                         { "are primarily expected to be used in conjunction with", "are made to work with" },
                                                                         { "are primarily expected to be used internally", "are suitable for internal use" },
                                                                         { "are primarily expected to be used in", "are suitable for" },
                                                                         { "are primarily intended to be used by", "are suitable for" },
                                                                         { "are primarily intended to be used for", "are suitable for" },
                                                                         { "are primarily intended to be used in combination with", "are made to work with" },
                                                                         { "are primarily intended to be used in conjunction with", "are made to work with" },
                                                                         { "are primarily intended to be used internally", "are suitable for internal use" },
                                                                         { "are primarily intended to be used in", "are suitable for" },
                                                                         { "are primarily meant to be used by", "are suitable for" },
                                                                         { "are primarily meant to be used for", "are suitable for" },
                                                                         { "are primarily meant to be used in combination with", "are made to work with" },
                                                                         { "are primarily meant to be used in conjunction with", "are made to work with" },
                                                                         { "are primarily meant to be used internally", "are suitable for internal use" },
                                                                         { "are primarily meant to be used in", "are suitable for" },
                                                                         { "are to be used by", "are suitable for" },
                                                                         { "are to be used in combination with", "are made to work with" },
                                                                         { "are to be used in conjunction with", "are made to work with" },
                                                                         { "are to be used internally", "are suitable for internal use" },
                                                                         { "are to be used in", "are suitable for" },
                                                                         { "can be used in combination with", "is made to work with" },
                                                                         { "can be used in conjunction with", "is made to work with" },
                                                                         { "can be used in", "is suitable for" },
                                                                         { "could be used in combination with", "is made to work with" },
                                                                         { "could be used in conjunction with", "is made to work with" },
                                                                         { "could be used in", "is suitable for" },
                                                                         { "expected to be used by", "suitable for" },
                                                                         { "expected to be used for", "suitable for" },
                                                                         { "expected to be used in combination with", "made to work with" },
                                                                         { "expected to be used in conjunction with", "made to work with" },
                                                                         { "expected to be used internally", "suitable for internal use" },
                                                                         { "expected to be used in", "suitable for" },
                                                                         { "has to be used in combination with", "is made to work with" },
                                                                         { "has to be used in conjunction with", "is made to work with" },
                                                                         { "has to be used internally", "is suitable for internal use" },
                                                                         { "has to be used in", "is suitable for" },
                                                                         { "have to be used in combination with", "are made to work with" },
                                                                         { "have to be used in conjunction with", "are made to work with" },
                                                                         { "have to be used internally", "are suitable for internal use" },
                                                                         { "have to be used in", "are suitable for" },
                                                                         { "intended to be used by", "suitable for" },
                                                                         { "intended to be used for", "suitable for" },
                                                                         { "intended to be used in combination with", "made to work with" },
                                                                         { "intended to be used in conjunction with", "made to work with" },
                                                                         { "intended to be used internally", "suitable for internal use" },
                                                                         { "intended to be used in", "suitable for" },
                                                                         { "is expected to be used by", "is suitable for" },
                                                                         { "is expected to be used for", "is suitable for" },
                                                                         { "is expected to be used in combination with", "is made to work with" },
                                                                         { "is expected to be used in conjunction with", "is made to work with" },
                                                                         { "is expected to be used internally", "is suitable for internal use" },
                                                                         { "is expected to be used in", "is suitable for" },
                                                                         { "is intended to be used by", "is suitable for" },
                                                                         { "is intended to be used for", "is suitable for" },
                                                                         { "is intended to be used in combination with", "is made to work with" },
                                                                         { "is intended to be used in conjunction with", "is made to work with" },
                                                                         { "is intended to be used internally", "is suitable for internal use" },
                                                                         { "is intended to be used in", "is suitable for" },
                                                                         { "is meant to be used by", "is suitable for" },
                                                                         { "is meant to be used for", "is suitable for" },
                                                                         { "is meant to be used in combination with", "is made to work with" },
                                                                         { "is meant to be used in conjunction with", "is made to work with" },
                                                                         { "is meant to be used internally", "is suitable for internal use" },
                                                                         { "is meant to be used in", "is suitable for" },
                                                                         { "is primarily expected to be used by", "is suitable for" },
                                                                         { "is primarily expected to be used for", "is suitable for" },
                                                                         { "is primarily expected to be used in combination with", "is made to work with" },
                                                                         { "is primarily expected to be used in conjunction with", "is made to work with" },
                                                                         { "is primarily expected to be used internally", "is suitable for internal use" },
                                                                         { "is primarily expected to be used in", "is suitable for" },
                                                                         { "is primarily intended to be used by", "is suitable for" },
                                                                         { "is primarily intended to be used for", "is suitable for" },
                                                                         { "is primarily intended to be used in combination with", "is made to work with" },
                                                                         { "is primarily intended to be used in conjunction with", "is made to work with" },
                                                                         { "is primarily intended to be used internally", "is suitable for internal use" },
                                                                         { "is primarily intended to be used in", "is suitable for" },
                                                                         { "is primarily meant to be used by", "is suitable for" },
                                                                         { "is primarily meant to be used for", "is suitable for" },
                                                                         { "is primarily meant to be used in combination with", "is made to work with" },
                                                                         { "is primarily meant to be used in conjunction with", "is made to work with" },
                                                                         { "is primarily meant to be used internally", "is suitable for internal use" },
                                                                         { "is primarily meant to be used in", "is suitable for" },
                                                                         { "is to be used by", "is suitable for" },
                                                                         { "meant to be used by", "suitable for" },
                                                                         { "meant to be used for", "suitable for" },
                                                                         { "meant to be used in combination with", "made to work with" },
                                                                         { "meant to be used in conjunction with", "made to work with" },
                                                                         { "meant to be used internally", "suitable for internal use" },
                                                                         { "meant to be used in", "suitable for" },
                                                                         { "may be used by", "is suitable for" },
                                                                         { "may be used for", "is suitable for" },
                                                                         { "may be used in combination with", "is made to work with" },
                                                                         { "may be used in conjunction with", "is made to work with" },
                                                                         { "may be used in", "is suitable for" },
                                                                         { "might be used by", "is suitable for" },
                                                                         { "might be used for", "is suitable for" },
                                                                         { "might be used in combination with", "is made to work with" },
                                                                         { "might be used in conjunction with", "is made to work with" },
                                                                         { "might be used in", "is suitable for" },
                                                                         { "primarily expected to be used by", "suitable for" },
                                                                         { "primarily expected to be used for", "suitable for" },
                                                                         { "primarily expected to be used internally", "suitable for internal use" },
                                                                         { "primarily expected to be used in", "suitable for" },
                                                                         { "primarily intended to be used by", "suitable for" },
                                                                         { "primarily intended to be used for", "suitable for" },
                                                                         { "primarily intended to be used internally", "suitable for internal use" },
                                                                         { "primarily intended to be used in", "suitable for" },
                                                                         { "primarily meant to be used by", "suitable for" },
                                                                         { "primarily meant to be used for", "suitable for" },
                                                                         { "primarily meant to be used internally", "suitable for internal use" },
                                                                         { "primarily meant to be used in", "suitable for" },
                                                                         { "to be used by", "suitable for" },
                                                                         { "to be used during", "suitable for" },
                                                                         { "to be used for", "suitable for" },
                                                                         { "to be used to", "to" },
                                                                         { "to be used when", "suitable when" },
                                                                         { "to be used with", "suitable for" },

                                                                         // -> applicable to
                                                                         { "primarily expected to be used within", "applicable to" },
                                                                         { "expected to be used within", "applicable to" },
                                                                         { "to be used within", "applicable to" },

                                                                         // it shall be used in
                                                                         // it should be used in
                                                                         // shall be used in
                                                                         // should be used in
                                                                         // that can be used in
                                                                         // that will be used in
                                                                         // to be used in
                                                                         // which can be used in
                                                                         // which will be used in
                                                                         // will be used in

                                                                         // should not be used in future
                                                                         // can not be used in general
                                                                         // cannot be used in general

                                                                         // !!! ATTENTION:
                                                                         // Be aware of follow-up texts:
                                                                         // - in order to
                                                                         // - in combination with
                                                                         // - in conjunction with
                                                                         // - in future
                                                                         // - in general
                                                                         // - in case of
                                                                         // - instead
                                                                     };

        private static readonly string[] WrongPhrases = [.. Map.Keys];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_text_in_class_documentation_([ValueSource(nameof(WrongPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// It " + phrase + @" do something.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_text_in_record_documentation_([ValueSource(nameof(WrongPhrases))] string phrase) => An_issue_is_reported_for(@"
/// <summary>
/// It " + phrase + @" do something.
/// </summary>
public sealed record TestMe(int primary)
{
}");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(WrongPhrases))] string phrase)
        {
            const string Template = @"
/// <summary>
/// It ### do something.
/// </summary>
public class TestMe
{
}";

            VerifyCSharpFix(Template.Replace("###", phrase), Template.Replace("###", Map[phrase]));
        }

        [TestCase("Callback used to analyze stuff.", "Callback to analyze stuff.")]
        [TestCase("Can be used to analyze stuff.", "Allows to analyze stuff.")]
        [TestCase("Does something. It is used to analyze stuff. Performs something more.", "Does something. It analyzes stuff. Performs something more.")]
        [TestCase("Does something. Used to analyze stuff. Performs something more.", "Does something. Analyzes stuff. Performs something more.")]
        [TestCase("Does something. Used to analyze stuff.", "Does something. Analyzes stuff.")]
        [TestCase("Does something that can be used to analyze stuff.", "Does something that allows to analyze stuff.")]
        [TestCase("Does something that can be used in order to analyze stuff.", "Does something that allows to analyze stuff.")]
        [TestCase("Does something that could be used to analyze stuff.", "Does something that allows to analyze stuff.")]
        [TestCase("Does something that could be used in order to analyze stuff.", "Does something that allows to analyze stuff.")]
        [TestCase("Does something which can be used to analyze stuff.", "Does something which allows to analyze stuff.")]
        [TestCase("Does something which can be used in order to analyze stuff.", "Does something which allows to analyze stuff.")]
        [TestCase("Does something which could be used to analyze stuff.", "Does something which allows to analyze stuff.")]
        [TestCase("Does something which could be used in order to analyze stuff.", "Does something which allows to analyze stuff.")]
        [TestCase("It can be used to analyze stuff.", "It allows to analyze stuff.")]
        [TestCase("It can be used in order to analyze stuff.", "It allows to analyze stuff.")]
        [TestCase("Markers are used to analyze stuff.", "Markers analyze stuff.")]
        [TestCase("Used to analyze stuff.", "Analyzes stuff.")]
        [TestCase("To be used by stuff.", "Suitable for stuff.")]
        [TestCase("Something to be used by stuff.", "Something suitable for stuff.")]
        [TestCase("This object is used to determine whether something has to be done.", "This object defines whether something has to be done.")]
        [TestCase("These objects are used to determine whether something has to be done.", "These objects define whether something has to be done.")]
        [TestCase("This object is used to find out whether something has to be done.", "This object defines whether something has to be done.")]
        [TestCase("These objects are used to find out whether something has to be done.", "These objects define whether something has to be done.")]
        [TestCase("This object is used to check whether something has to be done.", "This object defines whether something has to be done.")]
        [TestCase("These objects are used to check whether something has to be done.", "These objects define whether something has to be done.")]
        [TestCase("This method is intended to be used when you want to do something.", "This method is suitable when you want to do something.")]
        [TestCase("Is used to do something.", "Does something.")]
        [TestCase("Are used to do something.", "Does something.")]
        public void Code_gets_fixed_for_special_case_text_(string originalCode, string fixedCode)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// ###
    /// </summary>
    public void DoSomething()
    {
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("The information can be used to do something.", "The information allows to do something.")]
        [TestCase("The information. It can be used to analyze stuff.", "The information. It allows to analyze stuff.")]
        [TestCase("The information. Can be used to analyze stuff.", "The information. Allows to analyze stuff.")]
        [TestCase("The information is used to connect.", "The information connects.")]
        [TestCase("The sentences are used to avoid confusion.", "The sentences avoid confusion.")]
        [TestCase(
             @"<see langword=""true""/> to indicate something; otherwise, <see langword=""false""/>. This information can be used to hide specific stuff. The default is <see langword=""false""/>.",
             @"<see langword=""true""/> to indicate something; otherwise, <see langword=""false""/>. This information allows to hide specific stuff. The default is <see langword=""false""/>.")]
        public void Code_gets_fixed_for_param_text_(string originalCode, string fixedCode)
        {
            const string Template = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""o"">###</param>
    public void DoSomething(object o)
    {
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_param_text_with_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""o"">
    /// <see langword=""true""/> to indicate something; otherwise, <see langword=""false""/>.
    /// This information can be used to hide specific stuff.
    /// The default is <see langword=""false""/>.
    /// </param>
    public void DoSomething(object o)
    {
    }
}";

            // issue is with 2 separate text tokens
            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""o"">
    /// <see langword=""true""/> to indicate something; otherwise, <see langword=""false""/>.
    /// This information allows to hide specific stuff.
    /// The default is <see langword=""false""/>.
    /// </param>
    public void DoSomething(object o)
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_param_text_with_on_record()
        {
            const string OriginalCode = @"
/// <summary>
/// Does something.
/// </summary>
/// <param name=""o"">
/// An object that is used to order this stuff.
/// </param>
public sealed record TestMe(object o)
{
}";

            const string FixedCode = @"
/// <summary>
/// Does something.
/// </summary>
/// <param name=""o"">
/// An object that orders this stuff.
/// </param>
public sealed record TestMe(object o)
{
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2218_DocumentationShouldNotContainUsedToAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2218_CodeFixProvider();
    }
}