using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5015_StringLiteralGetsInternedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_interned_literal_as_constant_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
");

        [Test]
        public void No_issue_is_reported_for_non_interned_literal_as_readonly_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static readonly string Value = ""test me"";
}
");

        [Test]
        public void No_issue_is_reported_for_interned_string() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        string s = 1.ToString();
        var x = string.Intern(s);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_interned_literal() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static readonly string Value = string.Intern(""test me"");
}
");

        [Test]
        public void Code_gets_fixed_for_interned_literal_in_constant()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private static readonly string Value = string.Intern(""test me"");
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private static readonly string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interned_literal_in_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string DoSomething()
    {
        var x = string.Intern(""some text"");

        return x;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string DoSomething()
    {
        var x = ""some text"";

        return x;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5015_StringLiteralGetsInternedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5015_StringLiteralGetsInternedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5015_CodeFixProvider();
    }
}