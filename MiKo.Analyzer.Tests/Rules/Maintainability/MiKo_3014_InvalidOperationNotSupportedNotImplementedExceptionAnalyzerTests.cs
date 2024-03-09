using System;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MatchingExceptions =
                                                              {
                                                                  nameof(InvalidOperationException),
                                                                  nameof(NotSupportedException),
                                                                  nameof(NotImplementedException),
                                                                  typeof(InvalidOperationException).FullName,
                                                                  typeof(NotSupportedException).FullName,
                                                                  typeof(NotImplementedException).FullName,
                                                              };

        private static readonly string[] NonMatchingExceptions =
                                                                 {
                                                                     nameof(Exception),
                                                                     nameof(ArgumentException),
                                                                     nameof(ArgumentNullException),
                                                                     typeof(Exception).FullName,
                                                                     typeof(ArgumentException).FullName,
                                                                     typeof(ArgumentNullException).FullName,
                                                                 };

        private static readonly string[] Exceptions = MatchingExceptions.Concat(NonMatchingExceptions).Distinct().ToArray();

        [Test]
        public void No_issue_is_reported_for_thrown_([ValueSource(nameof(Exceptions))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new " + exceptionName + @"(""some reason"");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_matching_thrown_([ValueSource(nameof(NonMatchingExceptions))] string exceptionName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_reasonless_thrown_([ValueSource(nameof(MatchingExceptions))] string exceptionName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new " + exceptionName + @"();
    }
}
");

        [Test]
        public void Code_gets_fixed_for_type_inside_of_test_class_for_([ValueSource(nameof(MatchingExceptions))] string exceptionName)
        {
            const string Template = @"
using System;

using NUnit.Framework;

[TestFixture]
public class Tests
{
    private class TestMe
    {
        public void DoSomething(int x)
        {
            throw new ###;
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", exceptionName + "()"), Template.Replace("###", exceptionName + @"(""It's a mock."")"));
        }

        protected override string GetDiagnosticId() => MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3014_CodeFixProvider();
    }
}