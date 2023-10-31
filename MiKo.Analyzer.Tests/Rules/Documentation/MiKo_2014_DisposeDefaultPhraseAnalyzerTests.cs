using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2014_DisposeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_inherited_documentation() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <inheritdoc />
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_documentation() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_documentation_with_para_tags() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// <para>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </para>
    /// </summary>
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_documentation_with_parameter() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name='disposing'>Indicates whether unmanaged resources shall be freed.</param>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_documentation_with_parameter_and_para_tags() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// <para>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </para>
    /// </summary>
    /// <param name='disposing'>
    /// <para>
    /// Indicates whether unmanaged resources shall be freed.
    /// </para>
    /// </param>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_summary_documentation_without_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Some cleanup.
    /// </summary>
    public void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_summary_documentation_with_parameter() => An_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Some cleanup.
    /// </summary>
    /// <param name='disposing'>
    /// <para>
    /// Indicates whether unmanaged resources shall be freed.
    /// </para>
    /// </param>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_parameter_documentation() => An_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name='disposing'>Some flag.</param>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_parameter_documentation() => An_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name='disposing'></param>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_missing_parameter_documentation() => No_issue_is_reported_for(@"

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void Code_gets_fixed_for_method_without_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some text.
    /// </summary>
    private void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    private void Dispose() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parameter()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some text.
    /// </summary>
    /// <param name='disposing'>Some flag.</param>
    private void Dispose(bool disposing) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name=""disposing"">
    /// Indicates whether unmanaged resources shall be freed.
    /// </param>
    private void Dispose(bool disposing) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2014_DisposeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2014_DisposeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2014_CodeFixProvider();
    }
}