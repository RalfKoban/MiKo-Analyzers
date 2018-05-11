using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzerTests : CodeFixVerifier
    {
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

        protected override string GetDiagnosticId() => MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> Exceptions() => MatchingExceptions().Concat(NonMatchingExceptions());

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> MatchingExceptions() => new[]
                                                                       {
                                                                           nameof(InvalidOperationException),
                                                                           nameof(NotSupportedException),
                                                                           nameof(NotImplementedException),
                                                                           typeof(InvalidOperationException).FullName,
                                                                           typeof(NotSupportedException).FullName,
                                                                           typeof(NotImplementedException).FullName,
                                                                       };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> NonMatchingExceptions() => new[]
                                                                          {
                                                                              nameof(Exception),
                                                                              nameof(ArgumentException),
                                                                              nameof(ArgumentNullException),
                                                                              typeof(Exception).FullName,
                                                                              typeof(ArgumentException).FullName,
                                                                              typeof(ArgumentNullException).FullName,
                                                                          };
    }
}