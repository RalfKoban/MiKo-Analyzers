using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_string_literal_as_constant_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_as_local_constant() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        const string Value = ""test me"";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_string_literal_as_argument() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        DoSomething(""test me"");
    }

    public void DoSomething(string s)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_static_readonly_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static readonly string Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_readonly_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private readonly string Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_static_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static string Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_field_without_accessibility() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    string Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_local_variable() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        string value = ""test me"";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_string_literal_as_local_variable_with_var() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var value = ""test me"";
    }
}
");

        [Test]
        public void Code_gets_fixed_for_string_literal_as_static_readonly_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private static readonly string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_readonly_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private readonly string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_static_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private static string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_field()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_field_with_inline_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    // some comment
    private string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    // some comment
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_field_with_XML_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Some comment.</summary>
    private string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Some comment.</summary>
    private const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_field_without_accessibility()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    string Value = ""test me"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    const string Value = ""test me"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_local_variable()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        string value = ""test me"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        const string value = ""test me"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_local_variable_with_var()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var value = ""test me"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        const string value = ""test me"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_string_literal_as_local_variable_with_var_and_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        // some comment
        var value = ""test me"";
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void DoSomething()
    {
        // some comment
        const string value = ""test me"";
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5017_CodeFixProvider();
    }
}