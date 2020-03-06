using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3085_ConditionalExpressionTooLongAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_short_condition_and_paths() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => o != null ? true : false;
}");

        [Test]
        public void No_issue_is_reported_for_conditional_expression_with_simple_member_access() => No_issue_is_reported_for(@"
using System;

public static class Some
{
    public static class Constants
    {
        public static class That
        {
            public static class CannotBe
            {
                public const bool ShortenedAnymore = true;
            }
        }
    }
}

public class TestMe
{
    public bool DoSomething(object o) => o != null
                                         ? Some.Constants.That.CannotBe.ShortenedAnymore
                                         : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_long_condition_and_short_paths() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object o) => (o != null && (o.GetHashCode() == 42 || o.GetHashCode() == 0815)) ? true : false;
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_true_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items != null
                ? items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any()
                : false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_conditional_expression_with_short_condition_and_long_false_path() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public bool DoSomething(List<object> items)
    {
        return items == null
                ? false
                : items.Select(item => item.GetHashCode()).Where(hashCode  => hashCode >= 42).Any();
    }
}");

        [Test]
        public void No_issue_is_reported_for_object_creation_only() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ObjectDisposedException ObjectDisposed(string objectName, string message) => string.IsNullOrWhiteSpace(message)
                                                                                                 ? new ObjectDisposedException(objectName)
                                                                                                 : new ObjectDisposedException(objectName, message);
}");

        [Test]
        public void No_issue_is_reported_for_object_creation_with_literal() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string message, object value) => string.IsNullOrWhiteSpace(message)
                                                                                                                      ? new ArgumentOutOfRangeException(paramName, 0815, string.Empty)
                                                                                                                      : new ArgumentOutOfRangeException(paramName, 0815, message);
}");

        [Test]
        public void An_issue_is_reported_for_object_creation_with_Linq() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, string[] messages, object value) => messages.Any()
                                                                                                                          ? new ArgumentOutOfRangeException(paramName, 0815, messages.Where(_ => _.Length > 1).FirstOrDefault())
                                                                                                                          : new ArgumentOutOfRangeException(paramName, 0815, string.Empty);
}");

        [Test]
        public void No_issue_is_reported_for_object_initializer() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }

    public Guid ID { get; set; }

    pubic static TestMe Create(bool flag) => flag
                                            ? new TestMe { Name = ""my very long name to see whether it is too long"", ID = Guid.Empty }
                                            : new TestMe { Name = ""my very long name to see whether it is too long"", ID = new Guid(""45A3C8BA-2BC9-41F4-BA0D-997D38C8E6BA"") };
}");

        protected override string GetDiagnosticId() => MiKo_3085_ConditionalExpressionTooLongAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3085_ConditionalExpressionTooLongAnalyzer();
    }
}