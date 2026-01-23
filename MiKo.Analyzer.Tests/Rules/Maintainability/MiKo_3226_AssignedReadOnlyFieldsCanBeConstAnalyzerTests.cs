using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_unassigned_static_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private static int m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_unassigned_static_readonly_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private static readonly int m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_assigned_non_static_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private int m_field = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_const_field() => No_issue_is_reported_for(@"
public class TestMe
{
    private const int FIELD = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_assigned_static_readonly_field_of_reference_type() => No_issue_is_reported_for(@"
public class TestMe
{
    private static readonly object m_field = new object();
}
");

        [Test]
        public void No_issue_is_reported_for_assigned_non_static_readonly_field_of_reference_type() => No_issue_is_reported_for(@"
public class TestMe
{
    private readonly object m_field = new object();
}
");

        [Test]
        public void No_issue_is_reported_for_static_readonly_field_of_string_type_when_assigned_to_different_type() => No_issue_is_reported_for(@"
using System;
using System.Xml.Linq;

public class TestMe
{
    private static readonly XNamespace Value = ""test me"";
}
");

        [Test]
        public void An_issue_is_reported_for_assigned_static_readonly_field_with_number_literal() => An_issue_is_reported_for(@"
public class TestMe
{
    private static readonly int m_field = 42;
}
");

        [Test]
        public void An_issue_is_reported_for_assigned_non_static_readonly_field_with_number_literal() => An_issue_is_reported_for(@"
public class TestMe
{
    private readonly int m_field = 42;
}
");

        [Test]
        public void An_issue_is_reported_for_assigned_static_readonly_field_with_string_literal() => An_issue_is_reported_for(@"
public class TestMe
{
    private static readonly string m_field = ""42"";
}
");

        [Test]
        public void An_issue_is_reported_for_assigned_non_static_readonly_field_with_string_literal() => An_issue_is_reported_for(@"
public class TestMe
{
    private readonly string m_field = ""42"";
}
");

        [Test]
        public void Code_gets_fixed_for_assigned_static_readonly_field_with_number_literal()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static readonly int m_field = 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    private const int m_field = 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assigned_non_static_readonly_field_with_number_literal()
        {
            const string OriginalCode = @"
public class TestMe
{
    private readonly int m_field = 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    private const int m_field = 42;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assigned_static_readonly_field_with_string_literal()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static readonly string m_field = ""42"";
}
";

            const string FixedCode = @"
public class TestMe
{
    private const string m_field = ""42"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assigned_non_static_readonly_field_with_string_literal()
        {
            const string OriginalCode = @"
public class TestMe
{
    private readonly string m_field = ""42"";
}
";

            const string FixedCode = @"
public class TestMe
{
    private const string m_field = ""42"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3226_CodeFixProvider();
    }
}