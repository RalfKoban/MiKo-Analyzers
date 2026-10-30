using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3246_DoNotReturnDelegatesByMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Delegates = ["Action", "Action<int>", "Func<bool>", "Func<int, bool>", "System.Delegate"];

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => { }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_parameter_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + delegateName + @" callback) => { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_returning_no_delegate() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething() => 42
}
");

        [Test]
        public void An_issue_is_reported_for_interface_method_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    " + delegateName + @" DoSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_method_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + delegateName + @" DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_interface_implementation_of_method_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

#pragma warning disable MiKo_3246

public interface IFoo
{
    " + delegateName + @" DoSomething();
}

#pragma warning restore MiKo_3246

public class TestMe : IFoo
{
    public " + delegateName + @" DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_explicit_interface_implementation_of_method_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

#pragma warning disable MiKo_3246

public interface IFoo
{
    " + delegateName + @" DoSomething();
}

#pragma warning restore MiKo_3246

public class TestMe : IFoo
{
    " + delegateName + @" IFoo.DoSomething() => null;
}
");

        // 'static' interface members now allowed in C# 8, starting with C# 11 they can even be 'static abstract' or 'static virtual'
        // (https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interface-implementation/static-virtual-interface-members)
        [Test]
        public void No_issue_is_reported_for_implicit_static_interface_implementation_of_method_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

#pragma warning disable MiKo_3246

public interface IFoo
{
    static abstract " + delegateName + @" DoSomething();
}

#pragma warning restore MiKo_3246

public class TestMe : IFoo
{
    public static " + delegateName + @" DoSomething() => null;
}
");

        protected override string GetDiagnosticId() => MiKo_3246_DoNotReturnDelegatesByMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3246_DoNotReturnDelegatesByMethodsAnalyzer();
    }
}