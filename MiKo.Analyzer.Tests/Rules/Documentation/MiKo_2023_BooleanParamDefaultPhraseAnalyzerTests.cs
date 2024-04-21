using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2023_BooleanParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
//// ncrunch: no coverage start

        private static readonly string[] IndicatePhrases = CreateIndicatePhrases().Take(TestLimit).Distinct().ToArray();
        private static readonly string[] OptionalPhrases = CreateOptionalPhrases().Take(TestLimit).Distinct().ToArray();
        private static readonly string[] ConditionalPhrases = CreateConditionalStartPhrases().Take(TestLimit).Distinct().ToArray();
        private static readonly string[] DefaultCases = CreateDefaultCases().Take(TestLimit).Distinct().ToArray();

        private static readonly string[] TruePhrases =
                                                       {
                                                           @"<see langword=""true""/> if some condition. Otherwise <see langword=""false""/>.",
                                                           @"<see langword=""true""/> if some condition.<see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if some condition. <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if some condition; otherwise <see langword=""false""/>.",
                                                           @"<see langword=""true""/> if some condition; <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if some condition or not; <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if some condition or not, <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if you want to some condition, <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/> if you want to some condition or not, <see langword=""false""/> otherwise.",
                                                           @"<see langword=""true""/>: if some condition.",
                                                           @"<see langword=""true""/>: if some condition or not.",
                                                           @"<see langref=""true""/> if some condition",
                                                           @"<see langref=""true""/>: if some condition",
                                                           @"<see langref=""true""/>: if some condition or not",
                                                           "<b>true</b> if some condition; <b>false</b> otherwise.",
                                                           "<b>true</b>: if some condition; <b>false</b> otherwise.",
                                                           "<c>true</c> if some condition; <c>false</c> otherwise.",
                                                           "<c>true</c>: if some condition; <c>false</c> otherwise.",
                                                           "<value>true</value> if some condition; <value>false</value> otherwise.",
                                                           "<value>true</value>: if some condition; <value>false</value> otherwise.",
                                                           "True if some condition. Otherwise false.",
                                                           "True, if some condition. Otherwise false.",
                                                           "True: if some condition. Otherwise False.",
                                                           "TRUE: if some condition. Otherwise FALSE.",
                                                           @"""true"": if some condition. Otherwise ""false"".",
                                                           @"""True"": if some condition. Otherwise ""False"".",
                                                           @"""TRUE"": if some condition. Otherwise ""FALSE"".",
                                                           "'true': if some condition. Otherwise 'false'.",
                                                           "'True': if some condition. Otherwise 'False'.",
                                                           "'TRUE': if some condition. Otherwise 'FALSE'.",
                                                       };

//// ncrunch: no coverage end

        [Test]
        public void No_issue_is_reported_for_undocumented_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool condition) { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_non_boolean_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Some condition</param>
    public void DoSomething(int condition) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_boolean_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to do something; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_boolean_parameter_with_additional_info() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to do something; otherwise, <see langword=""false""/>.
    /// In addition, some more information.
    /// </param>
    public void DoSomething(bool condition) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_boolean_parameter_of_Dispose_method() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name=""disposing"">
        /// Indicates whether unmanaged resources shall be freed.
        /// </param>
        protected virtual void Dispose(bool disposing) { }
    }
");

        [TestCase("Some condition")]
        [TestCase("<b>true</b> to some condition")]
        [TestCase("true to some condition, <b>false</b> otherwise.")]
        [TestCase("<c>true</c> to some condition")]
        [TestCase("true to some condition, <c>false</c> otherwise.")]
        [TestCase("<value>true</value> to some condition")]
        [TestCase("true to some condition, <value>false</value> otherwise.")]
        public void An_issue_is_reported_for_incorrectly_documented_boolean_parameter_(string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + comment + @"</param>
    public void DoSomething(bool condition) { }
}
");

        [TestCase("Some", "some")]
        [TestCase("Tests the", "test the")]
        public void Code_gets_fixed_on_same_line_(string textToFix, string fixedText)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">### condition</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to ### condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", textToFix), FixedCode.Replace("###", fixedText));
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_comment_with_ending_dot()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Some condition.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_different_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// Some condition
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_ending_seeCref()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// Some condition for <see cref=""TestMe""/>
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition for <see cref=""TestMe""/>; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_special_phrase_([ValueSource(nameof(IndicatePhrases))] string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phrase + @" some condition is met.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that some condition is met; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_Or_not_special_phrase()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">A flag that defines whether some condition is met or not.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that some condition is met; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_special_phrase_with_default_case_([ValueSource(nameof(DefaultCases))] string defaultCase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Indicated whether something. " + defaultCase + @".</param>
    public void DoSomething(bool condition) { }
}
";
            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that something; otherwise, <see langword=""false""/>.
    /// " + defaultCase + @".
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_phrase_with_default_case_([ValueSource(nameof(DefaultCases))] string defaultCase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition""><see langword=""true""/> if some condition. Otherwise <see langword=""false""/>. " + defaultCase + @".</param>
    public void DoSomething(bool condition) { }
}
";
            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// " + defaultCase + @".
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_phrase_with_separate_line_for_default_case_([ValueSource(nameof(DefaultCases))] string defaultCase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition""><see langword=""true""/> if some condition. Otherwise <see langword=""false""/>.
    /// " + defaultCase + @".</param>
    public void DoSomething(bool condition) { }
}
";
            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// " + defaultCase + @".
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_without_links_for_phrase_with_default_case_([ValueSource(nameof(DefaultCases))] string defaultCase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">true if some condition, false otherwise. " + defaultCase + @".</param>
    public void DoSomething(bool condition) { }
}
";
            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// " + defaultCase + @".
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_optional_parameter_phrase_([ValueSource(nameof(OptionalPhrases))] string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phrase + @" some condition is met.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that some condition is met; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_true_phrase_([ValueSource(nameof(TruePhrases))] string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phrase + @"</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_different_line_for_true_phrase_([ValueSource(nameof(TruePhrases))] string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// " + phrase + @"
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_If_phrase_([ValueSource(nameof(ConditionalPhrases))] string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phrase + @" some condition</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_If_Else_phrase_([ValueSource(nameof(ConditionalPhrases))] string phraseStart)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phraseStart + @" some condition, else any other condition.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_If_Otherwise_phrase_([ValueSource(nameof(ConditionalPhrases))] string phraseStart)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + phraseStart + @" some condition. Otherwise any other condition.</param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to some condition; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("A flag controlling whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag controlling whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag indicating whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag indicating whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag specifying whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag specifying whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag that controls whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag that controls whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag that indicates whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag that indicates whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which controls whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which controls whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which indicates whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which indicates whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which specifies whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A flag which specifies whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]

        [TestCase("A bool controlling whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool controlling whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool indicating whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool indicating whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool specifying whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool specifying whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool that controls whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool that controls whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool that indicates whether items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool that indicates whether or not items shall be updated.", @"<see langword=""true""/> to indicate that items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which controls whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which controls whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which indicates whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which indicates whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which specifies whether the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]
        [TestCase("A bool which specifies whether or not the items shall be updated.", @"<see langword=""true""/> to indicate that the items shall be updated; otherwise, <see langword=""false""/>.")]

        [TestCase("Whether to do something.", @"<see langword=""true""/> to do something; otherwise, <see langword=""false""/>.")]
        [TestCase("<value>true</value>: Activates some stuff.", @"<see langword=""true""/> to activate some stuff; otherwise, <see langword=""false""/>.")]
        [TestCase(@"Set to <see langword=""true""/> if you want to do something, <see langword=""false""/> otherwise.", @"<see langword=""true""/> to do something; otherwise, <see langword=""false""/>.")]

        [TestCase(@"use true if the the value is something, <see langword=""false""/> otherwise", @"<see langword=""true""/> to indicate that the value is something; otherwise, <see langword=""false""/>.")]
        [TestCase(@"use true when the the value is something, <see langword=""false""/> otherwise", @"<see langword=""true""/> to indicate that the value is something; otherwise, <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true"" /> if a a value is something, <see langword=""false""/> otherwise", @"<see langword=""true""/> to indicate that a value is something; otherwise, <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true"" /> if an an value is something, <see langword=""false""/> otherwise", @"<see langword=""true""/> to indicate that an value is something; otherwise, <see langword=""false""/>.")]

        [TestCase(@"some data if <see langword=""true""/>, some other data if <see langword=""false""/>. Default value is <see langword=""false""/>.", @"<see langword=""true""/> to some data; otherwise, <see langword=""false""/>. Default value is <see langword=""false""/>.", Ignore = "Just for now")]
        [TestCase(@"<see langword=""true""/> if the items shall be selected.<see langword=""false""/> otherwise.", @"<see langword=""true""/> to select the items; otherwise, <see langword=""false""/>.", Ignore = "Just for now")]
        public void Code_gets_fixed_on_same_line_for_phrase_(string originalPhrase, string fixedPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">" + originalPhrase + @"</param>
    public void DoSomething(bool condition) { }
}
";
            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// " + fixedPhrase + @"
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Ignore("Just for now")]
        [Test]
        public void Code_gets_fixed_on_multi_line_for_phrase()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// ""true"": does some stuff inside
    /// ""false"": only other stuff shall be done
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to do some stuff inside; otherwise, <see langword=""false""/>.
    /// In such case only other stuff shall be done.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_mixed_multi_line_for_phrase()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Determines whether something is done. 
    /// Something else is skipped.
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that something is done; otherwise, <see langword=""false""/>.
    /// Something else is skipped.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_same_line_for_empty_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary></summary>
    /// <param name=""condition""></param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary></summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that TODO; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_on_different_line_for_empty_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// </param>
    public void DoSomething(bool condition) { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">
    /// <see langword=""true""/> to indicate that TODO; otherwise, <see langword=""false""/>.
    /// </param>
    public void DoSomething(bool condition) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2023_BooleanParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2023_CodeFixProvider();

//// ncrunch: no coverage start

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateStartTerms()
        {
            var terms = new[] { "flag", "Flag", "value", "Value", "parameter", "Parameter" };
            var booleans = new[] { "bool", "boolean" };

            foreach (var term in terms)
            {
                yield return term;
                yield return "A " + term;
                yield return "The " + term;

                foreach (var boolean in booleans)
                {
                    var booleanTerm = boolean + " " + term;

                    yield return boolean;
                    yield return "A " + boolean;
                    yield return "A " + booleanTerm;
                    yield return "The " + boolean;
                    yield return "The " + booleanTerm;
                }
            }
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateIndicatePhrases()
        {
            var starts = CreateStartTerms();

            var conditions = new[] { "if", "whether", "whether or not", "if to", "whether to", "whether or not to" };

            var verbs = new[]
                            {
                                "controling", // be aware of typo
                                "controlling",
                                "defining",
                                "determining",
                                "determinating", // be aware of typo
                                "indicating",
                                "specifying",
                                "that controls",
                                "that defined", // be aware of typo
                                "that defines",
                                "that determined", // be aware of typo
                                "that determines",
                                "that indicated", // be aware of typo
                                "that indicates",
                                "that specifies",
                                "which controls",
                                "which defines",
                                "which determines",
                                "which indicates",
                                "which specified", // be aware of typo
                                "which specifies",
                            };

            foreach (var phrase in from verb in verbs
                                   select " " + verb + " " into middle // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from condition in conditions
                                   select middle + condition into end // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from start in starts
                                   select start + end)
            {
                yield return phrase.ToUpperCaseAt(0).Interned();
                yield return phrase.ToLowerCaseAt(0).Interned();
            }

            var startingVerbs = new[]
                                    {
                                        "Controls",
                                        "Controling", // be aware of typo
                                        "Controlling",
                                        "Defines",
                                        "Defined",
                                        "Defining",
                                        "Determines",
                                        "Determined",
                                        "Determining",
                                        "Determinating", // be aware of typo
                                        "Indicates",
                                        "Indicated",
                                        "Indicating",
                                        "Specifies",
                                        "Specified",
                                        "Specifying",
                                    };

            foreach (var phrase in from startingVerb in startingVerbs
                                   from condition in conditions
                                   select string.Concat(startingVerb, " ", condition))
            {
                yield return phrase.ToUpperCaseAt(0).Interned();
                yield return phrase.ToLowerCaseAt(0).Interned();
            }
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateOptionalPhrases()
        {
            var starts = new[] { "A optional", "An optional", "The optional", "An (optional)", "The (optional)", "Optional", "(Optional)" };
            var conditions = new[] { "if", "whether", "whether or not", "if to", "whether to", "whether or not to" };
            var booleans = new[] { "bool ", "Boolean ", string.Empty };
            var values = new[]
                             {
                                 "parameter",
                                 "flag",
                                 "value",
                             };

            var verbs = new[]
                            {
                                "controling", // be aware of typo
                                "controlling",
                                "defining",
                                "determining",
                                "indicating",
                                "specifying",
                                "that controls",
                                "that defined", // be aware of typo
                                "that defines",
                                "that determined",
                                "that determines",
                                "that indicated", // be aware of typo
                                "that indicates",
                                "that specifies",
                                "which controls",
                                "which defines",
                                "which determines",
                                "which indicates",
                                "which specified", // be aware of typo
                                "which specifies",
                            };

            foreach (var phrase in from verb in verbs
                                   select " " + verb + " " into v // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from condition in conditions
                                   select v + condition into c // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from value in values
                                   select value + c into vc // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from boolean in booleans
                                   select " " + boolean + vc into end // we have lots of loops, so cache data to avoid unnecessary calculations
                                   from start in starts
                                   select (start + end).Replace("   ", " ").Replace("  ", " ").Trim())
            {
                yield return phrase.ToUpperCaseAt(0).Interned();
                yield return phrase.ToLowerCaseAt(0).Interned();
            }
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateConditionalStartPhrases()
        {
            var starts = new[] { "If set to", "If given", "If", "When set to", "When given", "When", "In case set to", "In case" };
            var booleans = new[] { @"<see langword=""true""/>", @"<see langref=""true""/>", "true" };
            var separators = new[] { string.Empty, ":", ";", "," };

            foreach (var phrase in from separator in separators
                                   from boolean in booleans
                                   from start in starts
                                   select string.Concat(start, " ", boolean, separator))
            {
                yield return phrase.ToUpperCaseAt(0).Interned();
                yield return phrase.ToLowerCaseAt(0).Interned();
            }
        }

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> CreateDefaultCases()
        {
            var starts = new[] { "The default is", "Default is", "Defaults to" };
            var booleans = new[] { @"<see langword=""true""/>", @"<see langref=""true""/>", "true", @"<see langword=""false""/>", @"<see langref=""false""/>", "false" };

            foreach (var phrase in from start in starts
                                   from boolean in booleans
                                   select string.Concat(start, " ", boolean))
            {
                yield return phrase.Interned();
            }
        }

//// ncrunch: no coverage end
    }
}