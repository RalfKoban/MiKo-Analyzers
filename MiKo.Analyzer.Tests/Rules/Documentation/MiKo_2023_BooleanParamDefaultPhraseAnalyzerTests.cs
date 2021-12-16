using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2023_BooleanParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
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

        // [TestCase(@"If set to <see langword=""true""/> some condition.")]
        // [TestCase(@"If set to <see langref=""true""/> some condition.")]
        [TestCase(@"<see langword=""true""/> if some condition. Otherwise <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/> if some condition. <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> if some condition; otherwise <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/> if some condition; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langref=""true""/> if some condition")]
        [TestCase(@"<b>true</b> if some condition; <b>false</b> otherwise.")]
        [TestCase(@"<c>true</c> if some condition; <c>false</c> otherwise.")]
        [TestCase(@"<value>true</value> if some condition; <value>false</value> otherwise.")]
        [TestCase(@"Determines whether some condition.")]
        [TestCase(@"Indicates whether some condition.")]
        [TestCase(@"If set to true if some condition.")]
        [TestCase(@"True if some condition. Otherwise false.")]
        public void Code_gets_fixed_on_same_line_For_special_phrase_(string phrase)
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

        protected override string GetDiagnosticId() => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2023_BooleanParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2023_CodeFixProvider();
    }
}