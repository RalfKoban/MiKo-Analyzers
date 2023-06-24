using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3079_DoNotUseIntegerForHResultAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_([Values(-2147, 2147, int.MinValue, int.MaxValue)] int number) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething()
    {
        return " + number.ToString("D") + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_([Values(0x80070000, 0x80070001, 0x80070002, 0x80070003, 0x80070004, 0x80070005)] uint number) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething()
    {
        return " + unchecked((int)number).ToString("D") + @";
    }
}
");

        [Test]
        public void Code_gets_fixed_for_HResult_([Values(0x80070000, 0x80070001, 0x80070002, 0x80070003, 0x80070004, 0x80070005)] uint number)
        {
            var originalCode = @"
using System;

public class TestMe
{
    public int DoSomething()
    {
        return " + unchecked((int)number).ToString("D") + @";
    }
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    public int DoSomething()
    {
        return unchecked((int)0x" + number.ToString("X") + @");
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3079_DoNotUseIntegerForHResultAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3079_DoNotUseIntegerForHResultAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3079_CodeFixProvider();
    }
}