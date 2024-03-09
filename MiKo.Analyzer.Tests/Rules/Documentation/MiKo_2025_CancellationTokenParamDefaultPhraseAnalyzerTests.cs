using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [TestCase(nameof(Object))]
        public void No_issue_is_reported_for_method_with_(string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(" + type + @" o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_out_parameter() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(out CancellationToken o) { }
}
");

        [TestCase("The token to monitor for cancellation requests.")]
        public void No_issue_is_reported_for_method_with_correct_comment_(string comment) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(CancellationToken o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(CancellationToken o) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation_(string xmlElement) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(CancellationToken o) { }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='token'>Whatever.</param>
    public void DoSomething(CancellationToken token) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='token'>
    /// The token to monitor for cancellation requests.
    /// </param>
    public void DoSomething(CancellationToken token) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_on_different_lines()
        {
            const string OriginalCode = @"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='token'>
    /// Whatever.
    /// </param>
    public void DoSomething(CancellationToken token) { }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

public class TestMe
{
    /// <summary />
    /// <param name='token'>
    /// The token to monitor for cancellation requests.
    /// </param>
    public void DoSomething(CancellationToken token) { }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2025_CodeFixProvider();
    }
}