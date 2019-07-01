using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3026_UnusedParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_no_unused_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int i)
        {
            var j = 42 + i;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_unused_variable_but_no_unused_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int i)
        {
            var j = 42 + i;
            return i;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_out_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(out int i)
        {
            i = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_handling_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object sender, EventArgs e)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_dependency_object_event_handling_method() => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_dependency_object_CoerceValueCallback_method() => No_issue_is_reported_for(@"
namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public object DoSomething(DependencyObject d, object value)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_dependency_object_ValidateValueCallback_method() => No_issue_is_reported_for(@"
namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public bool DoSomething(object value)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_overridden_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public abstract class BaseClass
    {
        public abstract void DoSomething(int i);
    }

    public class TestMe : BaseClass
    {
        public override void DoSomething(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_virtual_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public virtual void DoSomething(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_interface_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public interface IInterface
    {
        void DoSomething(int i);

        void DoSomething2(int j);
    }

    public class TestMe : IInterface
    {
        public void DoSomething(int i)
        {
        }

        void IInterface.DoSomething2(int j)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_bodied_method_with_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i) => DoSomething2(i);

        private int DoSomething2(int j) => j;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_with_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(int field)
        {
            m_field = field;
        }

        private int m_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_bodied_ctor_with_used_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(int field) => m_field = field;

        private int m_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unfinished_ctor() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(int field)
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_has_unused_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int i)
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_expression_bodied_method_that_has_unused_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int i) => 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_expression_bodied_method_that_has_1_used_and_1_unused_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int i, int j) => DoSomething2(i);

        private int DoSomething2(int k) => k;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_with_unused_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(int field)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_expression_bodied_ctor_with_unused_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe(int field) => m_field = 42;

        private int m_field;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_aspect_enhanced_method_with_unused_parameter() => No_issue_is_reported_for(@"
using System;

namespace PostSharp.Aspects.Advices
{
    public class Advice : Attribute
    {
    }

    public class OnMethodInvokeAdviceAttribute : Advice
    {
    }
}

namespace Bla
{
    using PostSharp.Aspects.Advices;

    public class TestMe
    {
        [OnMethodInvokeAdvice]
        public void DoSomething(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_serialization_ctor_with_unused_parameter() => No_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

namespace Bla
{
    public class TestMe
    {
        private TestMe(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3026_UnusedParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3026_UnusedParameterAnalyzer();
    }
}