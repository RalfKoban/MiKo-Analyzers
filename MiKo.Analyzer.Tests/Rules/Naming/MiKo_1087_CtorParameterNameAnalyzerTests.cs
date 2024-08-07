﻿using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1087_CtorParameterNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_primary_constructor_on_record() => No_issue_is_reported_for(@"
public sealed record TestMe(int X, int Y, double Distance);
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_ctor() => No_issue_is_reported_for(@"
using System;

public class BaseClass
{
    public BaseClass()
    {
    }

    public BaseClass(int a, int b)
    {
    }

    public BaseClass(int a, int b, int c)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int a, int b, int c) : base(a, b, c)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_ctor_when_multiple_ctors_are_available() => No_issue_is_reported_for(@"
using System;

public class BaseClass
{
    public BaseClass(string x, string y, string z)
    {
    }

    public BaseClass(int a, int b, int c)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int a, int b, int c) : base(a, b, c)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_if_ctor_parameter_does_not_match_parent_ctor_parameter() => No_issue_is_reported_for(@"
using System;

public class BaseClass
{
    public BaseClass()
    {
    }

    public BaseClass(string s)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int a, int b) : base(a.ToString())
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_if_the_base_constructor_has_more_parameters_than_the_inherited_one_but_the_code_cannot_yet_compile() => No_issue_is_reported_for(@"
using System;

public class BaseClass
{
    public BaseClass(int a, int b, int c)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int a, int b) : base(a, b)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_if_the_base_constructor_has_less_parameters_than_the_inherited_one_but_the_code_cannot_yet_compile() => No_issue_is_reported_for(@"
using System;

public class BaseClass
{
    public BaseClass(int a, int b)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int x, int y, int a, int b) : base(a, b, x)
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_ctor_if_the_names_to_not_match() => An_issue_is_reported_for(3, @"
using System;

public class BaseClass
{
    public BaseClass()
    {
    }

    public BaseClass(int a, int b)
    {
    }

    public BaseClass(int c, int d, int e)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int x, int y, int z) : base(x, y, z)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;

public class BaseClass
{
    public BaseClass(int a, int b, int c)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int x, int y, int z) : base(x, y, z)
    {
    }
}
";

            const string FixedCode = @"
using System;

public class BaseClass
{
    public BaseClass(int a, int b, int c)
    {
    }
}

public class ChildClass : BaseClass
{
    public ChildClass(int a, int b, int c) : base(a, b, c)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1087_CtorParameterNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1087_CtorParameterNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1087_CodeFixProvider();
    }
}