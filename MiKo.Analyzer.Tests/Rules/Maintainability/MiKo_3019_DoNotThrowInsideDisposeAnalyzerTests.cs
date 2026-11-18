using System;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3019_DoNotThrowInsideDisposeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_ordinary_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Dispose_method_explicit_interface_implementation() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Dispose_method() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DisposeCore_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private void DisposeCore()
    {
        throw new NotImplementedException();
    }
}");

        [Test]
        public void An_issue_is_reported_for_DisposeAsync_method() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DisposeAsync_method_explicit_interface_implementation() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe : IAsyncDisposable
{
    ValueTask IAsyncDisposable.DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DisposeCoreAsync_method() => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    private ValueTask DisposeCoreAsync()
    {
        throw new NotImplementedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_expression_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed => throw new NotImplementedException();
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_setter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed
    {
        set
        {
            throw new NotImplementedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_required() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed
    {
        required
        {
            throw new NotImplementedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_init() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed
    {
        init
        {
            throw new NotImplementedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_getter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_IsDisposed_expression_getter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool IsDisposed
    {
        get => throw new NotImplementedException();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3019_DoNotThrowInsideDisposeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3019_DoNotThrowInsideDisposeAnalyzer();
    }
}