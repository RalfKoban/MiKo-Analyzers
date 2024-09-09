using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3221_GetHashCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_GetHashCode_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_method_using_HashCode_Add_with_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(m_field1);
        hash.Add(m_field2);
        hash.Add(m_field3);
        hash.Add(m_field4);
        hash.Add(m_field5);
        hash.Add(m_field6);
        hash.Add(m_field7);
        hash.Add(m_field8);
        hash.Add(m_field9);
        return hash.ToHashCode();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_method_using_HashCode_with_default_and_var() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        var hash = default(HashCode);
        hash.Add(m_field1);
        hash.Add(m_field2);
        hash.Add(m_field3);
        hash.Add(m_field4);
        hash.Add(m_field5);
        hash.Add(m_field6);
        hash.Add(m_field7);
        hash.Add(m_field8);
        hash.Add(m_field9);
        return hash.ToHashCode();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_method_using_HashCode_with_default_and_explicit_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        HashCode hash = default();
        hash.Add(m_field1);
        hash.Add(m_field2);
        hash.Add(m_field3);
        hash.Add(m_field4);
        hash.Add(m_field5);
        hash.Add(m_field6);
        hash.Add(m_field7);
        hash.Add(m_field8);
        hash.Add(m_field9);
        return hash.ToHashCode();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_body_method_using_HashCode_Combine() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public override int GetHashCode()
    {
        return HashCode.Combine(08, 15);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_expression_body_method_using_HashCode_Combine() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public override int GetHashCode() => HashCode.Combine(08, 15);
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_body_method_not_using_only_a_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public override int GetHashCode()
    {
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_expression_body_method_using_only_a_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public override int GetHashCode() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_GetHashCode_body_method_not_using_HashCode_Combine() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode()
    {
        return 42 ^ m_field.GetHashCode();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_GetHashCode_expression_body_method_not_using_HashCode_Combine() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode() => 42 ^ m_field.GetHashCode();
}
");

        [Test]
        public void Code_gets_fixed_for_GetHashCode_expression_body_method_with_1_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode() => m_field.GetHashCode();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode() => HashCode.Combine(m_field);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_expression_body_method_with_2_arguments()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2;

    public override int GetHashCode() => 42 * m_field1.GetHashCode() ^ m_field2.GetHashCode();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2;

    public override int GetHashCode() => HashCode.Combine(m_field1, m_field2);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_expression_body_method_with_8_arguments()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8;

    public override int GetHashCode() => 42 * m_field1.GetHashCode() ^ m_field2.GetHashCode() ^ m_field3.GetHashCode() ^ m_field4.GetHashCode() ^ m_field5.GetHashCode() ^ m_field6.GetHashCode() ^ m_field7.GetHashCode() ^ m_field8.GetHashCode();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8;

    public override int GetHashCode() => HashCode.Combine(m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_expression_body_method_with_9_arguments()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode() => 42 * m_field1.GetHashCode() ^ m_field2.GetHashCode() ^ m_field3.GetHashCode() ^ m_field4.GetHashCode() ^ m_field5.GetHashCode() ^ m_field6.GetHashCode() ^ m_field7.GetHashCode() ^ m_field8.GetHashCode() ^ m_field9.GetHashCode();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        var hash = default(HashCode);
        hash.Add(m_field1);
        hash.Add(m_field2);
        hash.Add(m_field3);
        hash.Add(m_field4);
        hash.Add(m_field5);
        hash.Add(m_field6);
        hash.Add(m_field7);
        hash.Add(m_field8);
        hash.Add(m_field9);
        return hash.ToHashCode();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_body_method_with_1_argument()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode()
    {
        return m_field.GetHashCode();
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field;

    public override int GetHashCode() => HashCode.Combine(m_field);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_body_method_with_9_arguments()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        var result = 42 * m_field1.GetHashCode()
                        ^ m_field2.GetHashCode()
                        ^ m_field3.GetHashCode()
                        ^ m_field4.GetHashCode()
                        ^ m_field5.GetHashCode()
                        ^ m_field6.GetHashCode()
                        ^ m_field7.GetHashCode()
                        ^ m_field8.GetHashCode()
                        ^ m_field9.GetHashCode();

        return result;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private object m_field1, m_field2, m_field3, m_field4, m_field5, m_field6, m_field7, m_field8, m_field9;

    public override int GetHashCode()
    {
        var hash = default(HashCode);
        hash.Add(m_field1);
        hash.Add(m_field2);
        hash.Add(m_field3);
        hash.Add(m_field4);
        hash.Add(m_field5);
        hash.Add(m_field6);
        hash.Add(m_field7);
        hash.Add(m_field8);
        hash.Add(m_field9);
        return hash.ToHashCode();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_body_method_with_base_GetHashCode_argument()
        {
            const string OriginalCode = @"
using System;

public abstract class BaseClass
{
    public override int GetHashCode() => 42;
}

public class TestMe : BaseClass
{
    private readonly object m_field = new object();

    public override int GetHashCode()
    {
        var hashCode = 339610899;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + m_field.GetHashCode();
        return hashCode;
    }
}
";

            const string FixedCode = @"
using System;

public abstract class BaseClass
{
    public override int GetHashCode() => 42;
}

public class TestMe : BaseClass
{
    private readonly object m_field = new object();

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), m_field);
}
";

            VerifyCSharpFix(OriginalCode, FixedCode, allowNewCompilerDiagnostics: true); // needed as Compiler reports CS0103
        }

        protected override string GetDiagnosticId() => MiKo_3221_GetHashCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3221_GetHashCodeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3221_CodeFixProvider();
    }
}