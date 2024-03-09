using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private int m_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_public_static_readonly_DependencyProperty_field() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyProperty m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_public_static_readonly_DependencyProperty_field_([Values("protected", "internal", "protected internal", "private", "")] string visibility)
            => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        " + visibility + @" static readonly DependencyProperty m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_only_DependencyProperty_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static DependencyProperty m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_readonly_only_DependencyProperty_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public readonly DependencyProperty m_fieldProperty;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_static_non_readonly_DependencyProperty_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public DependencyProperty m_fieldProperty;
    }
}
");

        [TestCase("private")]
        [TestCase("protected readonly static")]
        [TestCase("static internal readonly")]
        public void Code_gets_fixed_for_field_with_modifier_(string modifier)
        {
            var originalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        " + modifier + @" DependencyProperty m_fieldProperty;
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyProperty m_fieldProperty;
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_inline_comment()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        // some comment
        static internal readonly DependencyProperty m_fieldProperty;
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        // some comment
        public static readonly DependencyProperty m_fieldProperty;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_XML_comment()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Some comment.</summary>
        protected readonly static DependencyProperty m_fieldProperty;
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Some comment.</summary>
        public static readonly DependencyProperty m_fieldProperty;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_without_modifiers_with_XML_comment()
        {
            const string OriginalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Some comment.</summary>
        DependencyProperty m_fieldProperty;
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Some comment.</summary>
        public static readonly DependencyProperty m_fieldProperty;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3050_CodeFixProvider();
    }
}