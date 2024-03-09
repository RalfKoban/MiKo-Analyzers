using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3047_ContentPropertyAttributeUsesNameofAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_ContentPropertyAttribute_with_nameof_applied() => No_issue_is_reported_for(@"
using System;

namespace System.Windows.Markup
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(string propertyName)
        {
        }
    }
}

namespace Bla
{
    using System.Windows.Markup;

    [ContentProperty(nameof(MyProperty))]
    public class TestMe
    {
        public string MyProperty { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_ContentPropertyAttribute_without_nameof_applied() => An_issue_is_reported_for(@"
using System;

namespace System.Windows.Markup
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(string propertyName)
        {
        }
    }
}

namespace Bla
{
    using System.Windows.Markup;

    [ContentProperty(""MyProperty"")]
    public class TestMe
    {
        public string MyProperty { get; set; }
    }
}");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;

namespace System.Windows.Markup
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(string propertyName)
        {
        }
    }
}

namespace Bla
{
    using System.Windows.Markup;

    [ContentProperty(""MyProperty"")]
    public class TestMe
    {
        public string MyProperty { get; set; }
    }
}";

            const string FixedCode = @"
using System;

namespace System.Windows.Markup
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        public ContentPropertyAttribute(string propertyName)
        {
        }
    }
}

namespace Bla
{
    using System.Windows.Markup;

    [ContentProperty(nameof(MyProperty))]
    public class TestMe
    {
        public string MyProperty { get; set; }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3047_ContentPropertyAttributeUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3047_ContentPropertyAttributeUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3047_CodeFixProvider();
    }
}