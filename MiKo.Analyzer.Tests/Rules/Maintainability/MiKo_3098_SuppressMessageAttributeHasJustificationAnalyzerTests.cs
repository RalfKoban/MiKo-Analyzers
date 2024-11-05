using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3098_SuppressMessageAttributeHasJustificationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_without_attribute() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething()
        {
            return new object();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_some_attribute() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        [Pure]
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_SuppressMessage_attribute_without_justification() => No_issue_is_reported_for(@"
using System;
using System.Diagnostics.CodeAnalysis;

namespace Bla
{
    public class TestMe
    {
        [SuppressMessage(""category"", ""checkId"")]
        public void DoSomething()
        {
        }
    }
}
");

        [TestCase("We do not want to have that method.")]
        [TestCase("This is a template.")]
        public void No_issue_is_reported_for_SuppressMessage_attribute_with_proper_justification_(string justification) => No_issue_is_reported_for(@"
using System;
using System.Diagnostics.CodeAnalysis;

namespace Bla
{
    public class TestMe
    {
        [SuppressMessage(""category"", ""checkId"", Justification = """ + justification + @""")]
        public void DoSomething()
        {
        }
    }
}
");

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("-")]
        [TestCase("TODO")]
        [TestCase("To do")]
        [TestCase("FIXME")]
        [TestCase("Fix me")]
        [TestCase("Fix it later")]
        [TestCase("Fix it in future")]
        [TestCase("Pending")]
        [TestCase("<Pending>")]
        [TestCase("Reviewed")]
        [TestCase("Reviewed.")]
        [TestCase("Suppression is OK here")]
        [TestCase("Suppression is OK here.")]
        [TestCase("Reviewed. Suppression is OK here.")]
        [TestCase("Suppressed")]
        [TestCase("Suppressed.")]
        [TestCase("Temp. suppressed")]
        [TestCase("Temp. suppressed.")]
        [TestCase("Temporarily suppressed")]
        [TestCase("Temporarily suppressed.")]
        [TestCase("As design")]
        [TestCase("As designed")]
        [TestCase("As design.")]
        [TestCase("As designed.")]
        [TestCase("By design")]
        [TestCase("By design.")]
        [TestCase("This is by design")]
        [TestCase("This is by design.")]
        [TestCase("By intend")] // typo
        [TestCase("By intend.")] // typo
        [TestCase("By intent")]
        [TestCase("By intent.")]
        [TestCase("whatever")] // just some short text where an explanation cannot fit in
        [TestCase("...............")] // some placeholders to attempt to avoid match
        [TestCase("***************")] // some placeholders to attempt to avoid match
        [TestCase("<------------->")] // some placeholders to attempt to avoid match
        [TestCase("<- ->")] // some placeholders to attempt to avoid match
        [TestCase("<- whatever ->")] // no explanation that would explain
        [TestCase("for reason")] // no explanation that would explain
        [TestCase("just because")] // no explanation that would explain
        public void An_issue_is_reported_for_SuppressMessage_attribute_with_poor_justification_(string justification) => An_issue_is_reported_for(@"
using System;
using System.Diagnostics.CodeAnalysis;

namespace Bla
{
    public class TestMe
    {
        [SuppressMessage(""category"", ""checkId"", Justification = """ + justification + @""")]
        public void DoSomething()
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3098_SuppressMessageAttributeHasJustificationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3098_SuppressMessageAttributeHasJustificationAnalyzer();
    }
}