﻿using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ExceptionNames =
            {
                nameof(ArgumentException),
                nameof(ArgumentNullException),
                nameof(ArgumentOutOfRangeException),
                "InvalidEnumArgumentException", // don't use nameof as the unit test framework will not find it (.NET CORE does not support System.ComponentModel)
                typeof(ArgumentException).FullName,
                typeof(ArgumentNullException).FullName,
                typeof(ArgumentOutOfRangeException).FullName,
                "System.ComponentModel.InvalidEnumArgumentException", // don't use typeof as the unit test framework will not find it (.NET CORE does not support System.ComponentModel)
            };

        [Test]
        public void No_issue_is_reported_for_correctly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_thrown_([ValueSource(nameof(ExceptionNames))] string exceptionName) => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething()
    {
        throw new " + exceptionName + @"();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3015_CodeFixProvider();
    }
}