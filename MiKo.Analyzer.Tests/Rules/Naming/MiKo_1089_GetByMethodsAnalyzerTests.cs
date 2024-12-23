﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1089_GetByMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Get_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_Get_only_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Get()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_non_repository_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void GetSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_extension_method() => No_issue_is_reported_for(@"
using System;

public static class TestMeExtensions
{
    public static void GetSomething(this object o)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_extension_method_in_extension_class() => No_issue_is_reported_for(@"
using System;

public static class TestMeExtensions
{
    public static void GetSomething(object o)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_method_test_method() => No_issue_is_reported_for(@"
using System;

using NUnit;

public class TestMeRepository
{
    [Test]
    public void GetSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method() => An_issue_is_reported_for(@"
using System;

public class TestMeRepository
{
    public void GetSomething()
    {
    }
}
");

        [TestCase("GetUserById", "UserWithId")]
        [TestCase("GetById", "WithId")]
        [TestCase("GetSomething", "Something")]
        public void Code_gets_fixed_for_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMeRepository
{
    public void ###()
    {
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1089_GetByMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1089_GetByMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1089_CodeFixProvider();
    }
}