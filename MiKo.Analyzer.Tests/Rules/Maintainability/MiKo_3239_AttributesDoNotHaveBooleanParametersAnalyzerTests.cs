using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3239_AttributesDoNotHaveBooleanParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_constructor_with_boolean_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(bool flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_class_primary_constructor_with_boolean_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe(bool flag)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_attribute_class_constructor_with_no_boolean_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute : Attribute
    {
        public TestMeAttribute(string message)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_attribute_class_primary_constructor_with_no_boolean_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute(string message) : Attribute
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_constructor_with_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute : Attribute
    {
        public TestMeAttribute(bool flag)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_primary_constructor_with_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute(bool flag) : Attribute
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_constructor_with_additional_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute : Attribute
    {
        public TestMeAttribute(string message, bool flag)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_primary_constructor_with_additional_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute(string message, bool flag) : Attribute
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_constructor_with_additional_optional_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute : Attribute
    {
        public TestMeAttribute(string message, bool flag = false)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_primary_constructor_with_additional_optional_boolean_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeAttribute(string message, bool flag = false) : Attribute
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3239_AttributesDoNotHaveBooleanParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3239_AttributesDoNotHaveBooleanParametersAnalyzer();
    }
}