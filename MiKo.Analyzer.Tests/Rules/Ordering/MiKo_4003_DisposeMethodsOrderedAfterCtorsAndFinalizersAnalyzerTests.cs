﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_without_Dispose_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_method_before_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void Dispose() { }

    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_before_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    void IDisposable.Dispose() { }

    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_before_ctor() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_before_ctor() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    void IDisposable.Dispose() { }

    public TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_in_between_ctor_and_finalizer() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public void Dispose() { }

    ~TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_in_between_ctor_and_finalizer() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    void IDisposable.Dispose() { }

    ~TestMe() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_method_after_other_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void DoSomething() { }

    public void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_after_other_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void DoSomething() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void Code_gets_fixed_for_interface_implementation_and_ctor_and_finalizer()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void DoSomething() { }

    void IDisposable.Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    void IDisposable.Dispose() { }

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ctor_and_finalizer()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void DoSomething() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void Dispose() { }

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ctor_only()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public void DoSomething() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public void Dispose() { }

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_finalizer_only()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    public void DoSomething() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    public void Dispose() { }

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_no_ctor_or_finalizer()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void DoSomething() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        //// TODO RKN: partial parts

        protected override string GetDiagnosticId() => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4003_CodeFixProvider();
    }
}