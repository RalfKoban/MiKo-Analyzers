using System;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3018_ObjectDisposedExceptionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ProblematicVisibilities =
                                                                   {
                                                                       "public",
                                                                       "internal",
                                                                       "protected",
                                                                       "protected internal",
                                                                   };

        private static readonly string[] Visibilities = ProblematicVisibilities.Concat(new[] { "private " }).ToArray();

        [Test]
        public void No_issue_is_reported_for_interface() => No_issue_is_reported_for(@"
using System;

public interface ITestMe : IDisposable
{
    void DoSomething();
}
");

        [Test]
        public void No_issue_is_reported_for_test_class() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

[TestFixture]
public class TestMeTests : IDisposable
{
    private bool _isDisposed;

    [Test]
    public void TestSomething()
    {
    }

    public void Dispose() => _isDisposed = true;
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_in_non_test_class() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

public class TestMeTests : IDisposable
{
    private bool _isDisposed;

    [Test]
    public void TestSomething()
    {
    }

    public void Dispose() => _isDisposed = true;
}
");

        [Test]
        public void No_issue_is_reported_for_non_disposable_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }

    public int SomeProperty { get; set; }

    public event MyEvent
    {
        add { }
        remove { }
    }
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public TestMe()
    {
    }

    public void Dispose() => _isDisposed = true;
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_method_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" void DoSomething()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_expression_body_private_property() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    private int SomeProperty => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_property_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }

            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_special_IsDisposed_property_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    " + visibility + @" bool IsDisposed { get; private set; }

    public void Dispose() => IsDisposed = true;
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_indexer_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }

            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_event_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }

        remove
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_private_helper_method_in_method_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" void DoSomething()
    {
        VerifyDisposed();
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_private_helper_method_in_property_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_private_helper_method_in_indexer_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_disposable_type_that_throws_ObjectDisposedException_in_private_helper_method_in_event_that_is_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            VerifyDisposed();
        }

        remove
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_inherited_disposable_type_with_protected_helper_method_which_is_called_in_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    protected void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}

public class TestMeEnhanced : TestMe
{
    " + visibility + @" void DoSomething()
    {
        VerifyDisposed();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_of_disposable_type_that_throws_a_([Values(nameof(NotImplementedException), nameof(NotSupportedException))] string exception) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    public void DoSomething()
    {
        throw new " + exception + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_expression_body_of_disposable_type_that_throws_a_([Values(nameof(NotImplementedException), nameof(NotSupportedException))] string exception) => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    public void DoSomething() => throw new " + exception + @";
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_expression_body_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int DoSomething() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_property_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_property_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }

            return 42;
        }

        set
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_expression_body_property_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_property_getter_expression_body_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get => 42;

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_property_setter_expression_body_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }

            return 42;
        }

        set => value = value;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_indexer_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_indexer_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }

            return 42;
        }

        set
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_event_adder_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
        }

        remove
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_that_does_not_throw_ObjectDisposedException_in_event_remover_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }

        remove
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" void DoSomething()
    {
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_property_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            return 42;
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_property_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_indexer_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            return 42;
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_indexer_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_event_adder_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
        }

        remove
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_which_is_not_called_in_event_remover_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            VerifyDisposed();
        }

        remove
        {
        }
    }

    private void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" void DoSomething()
    {
        VerifyDisposed();
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_property_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_property_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int SomeProperty
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_indexer_getter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            VerifyDisposed();

            return 42;
        }

        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_indexer_setter_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" int this[string key]
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }

        set
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_event_adder_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            VerifyDisposed();
        }

        remove
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_disposable_type_with_private_helper_method_that_does_not_throw_ObjectDisposedException_in_event_remover_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    " + visibility + @" event MyEvent
    {
        add
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException();
            }
        }

        remove
        {
            VerifyDisposed();
        }
    }

    private void VerifyDisposed()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_inherited_disposable_type_with_protected_helper_method_which_is_not_called_in_method_that_is_([ValueSource(nameof(ProblematicVisibilities))] string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    private bool _isDisposed;

    public void Dispose() => _isDisposed = true;

    protected void VerifyDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException();
        }
    }
}

public class TestMeEnhanced : TestMe
{
    " + visibility + @" void DoSomething()
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3018_ObjectDisposedExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3018_ObjectDisposedExceptionAnalyzer();
    }
}