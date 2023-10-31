using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6034_DotsAreOnSameLineAsInvokedMemberAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_if_complete_call_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.Collect();
    }
}
");

        [Test]
        public void No_issue_is_reported_if_dot_and_member_are_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC
          .Collect();
    }
}
");

        [Test]
        public void An_issue_is_reported_if_dot_and_member_are_on_different_lines() => An_issue_is_reported_for(@"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.
           Collect();
    }
}
");

        [Test]
        public void Code_gets_fixed_if_dot_and_member_are_on_different_lines()
        {
            const string OriginalCode = @"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC.
           Collect();
    }
}
";

            const string FixedCode = @"
using System;

public class TestME
{
    public void DoSomething()
    {
        GC
           .Collect();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_for_multi_line_invocations_if_dots_and_members_are_on_different_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestME
{
    public IEnumerable<int> DoSomething()
    {
        return Enumerable.
                    Empty<int>().
                    OrderBy(_ => _).
                    ThenBy(_ => _).
                    ToList();
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class TestME
{
    public IEnumerable<int> DoSomething()
    {
        return Enumerable
                    .Empty<int>()
                    .OrderBy(_ => _)
                    .ThenBy(_ => _)
                    .ToList();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6034_DotsAreOnSameLineAsInvokedMemberAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6034_DotsAreOnSameLineAsInvokedMemberAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6034_CodeFixProvider();
    }
}