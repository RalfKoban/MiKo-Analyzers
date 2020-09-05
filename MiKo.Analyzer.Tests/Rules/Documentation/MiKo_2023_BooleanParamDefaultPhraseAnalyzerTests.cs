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

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_boolean_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Some condition</param>
    public void DoSomething(bool condition) { }
}
");

        [Test]
        public void Code_gets_fixed_on_same_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// </summary>
    /// <param name=""condition"">Some condition</param>
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

        protected override string GetDiagnosticId() => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2023_BooleanParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2023_CodeFixProvider();
    }
}