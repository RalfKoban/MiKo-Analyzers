using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_DependencyPropertyKey_field() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_public_static_readonly_DependencyPropertyKey_field_([Values("protected", "internal", "protected internal", "private", "")] string visibility)
            => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        " + visibility + @" static readonly DependencyPropertyKey m_field;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_public_static_readonly_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        public static readonly DependencyPropertyKey m_field;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_only_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal static DependencyPropertyKey m_field;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_readonly_only_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal readonly DependencyPropertyKey m_field;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_static_non_readonly_DependencyPropertyKey_field() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        internal DependencyPropertyKey m_field;
    }
}
");

        [TestCase("public")]
        [TestCase("static readonly public")]
        [TestCase("static public")]
        public void Code_gets_fixed_for_field_with_modifier_(string modifier)
        {
            var originalCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        " + modifier + @" DependencyPropertyKey m_field;
    }
}
";

            const string FixedCode = @"
using System.Windows;

namespace Bla
{
    public class TestMe
    {
        private static readonly DependencyPropertyKey m_field;
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
        public static readonly DependencyPropertyKey m_field;
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
        private static readonly DependencyPropertyKey m_field;
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
        static readonly public DependencyPropertyKey m_field;
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
        private static readonly DependencyPropertyKey m_field;
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
        DependencyPropertyKey m_field;
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
        private static readonly DependencyPropertyKey m_field;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3052_CodeFixProvider();
    }
}