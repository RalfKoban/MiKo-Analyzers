using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_event_handling_method_with_custom_event_args() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class MyEventArgs : EventArgs
    {
    }

    public class TestMe
    {
        public event EventHandler<MyEventArgs> MyEvent;

        public void Initialize()
        {
            MyEvent += OnMyEvent;
        }

        private void OnMyEvent(object sender, MyEventArgs e)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_event_handling_method_with_fake_event_args() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class FakeEventArgs
    {
    }

    public class TestMe
    {
        public event EventHandler<FakeEventArgs> FakeEvent;

        public void Initialize()
        {
            FakeEvent += OnFakeEvent;
        }

        private void OnFakeEvent(object sender, FakeEventArgs e)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_dependency_property_changed_event_handling_method() => No_issue_is_reported_for(@"
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
        public void DoSomething(object sender, DependencyPropertyChangedEventArgs e)
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
        public void No_issue_is_reported_for_partial_method_body() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        partial void DoSomething(int i)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_partial_method_definition() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        partial void DoSomething(int i);
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

        [Test]
        public void No_issue_is_reported_if_method_is_passed_in_as_argument() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() => DoSomethingCore(MyCallback);

        private void DoSomethingCore(Action<object> callback) => callback(new object());

        private void MyCallback(object obj)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_if_method_is_passed_in_as_coalescing_argument() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() => DoSomethingCore(MyCallback1 ?? MyCallback2);

        private void DoSomethingCore(Action<object> callback) => callback(new object());

        private void MyCallback1(object obj)
        {
        }

        private void MyCallback2(object obj)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_AspNetCore_Startup_class() => No_issue_is_reported_for(@"
using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bla
{
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_AspNetCore_ControllerBase_class() => No_issue_is_reported_for(@"
using System;

using Microsoft.AspNetCore.Mvc;

namespace Bla
{
    public class MyController : ControllerBase
    { 
        public IActionResult DoSomething(byte[] data)
        {
            return null;
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

        protected override string GetDiagnosticId() => MiKo_3026_UnusedParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3026_UnusedParameterAnalyzer();
    }
}