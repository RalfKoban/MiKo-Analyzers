using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1539_NoVowelInNameAnalyzerTests : CodeFixVerifier
    {
        private const string LowerCaseVowels = "aeiou";
        private const string LowerCaseConstants = "bcdfghjklmnpqrstvwxyz";

        private const string UpperCaseVowels = "AEIOU";
        private const string UpperCaseConstants = "BCDFGHJKLMNPQRSTVWXYZ";

        [TestCaseSource(nameof(LowerCaseVowels))]
        [TestCaseSource(nameof(LowerCaseConstants))]
        [TestCaseSource(nameof(UpperCaseVowels))]
        [TestCaseSource(nameof(UpperCaseConstants))]
        [TestCase(Constants.Underscore)]
        public void No_issue_is_reported_for_local_variable_named_(char c) => No_issue_is_reported_for(@"
public class TestMe
    {
        public void DoSomething()
        {
            var " + c + @" = 1;
        }
    }
");

        [TestCase(Constants.LambdaIdentifiers.Default)]
        [TestCase(Constants.LambdaIdentifiers.FallbackUnderscores2)]
        [TestCase(Constants.LambdaIdentifiers.FallbackUnderscores3)]
        [TestCase(Constants.LambdaIdentifiers.FallbackUnderscores4)]
        public void No_issue_is_reported_for_local_variable_named_(string s) => No_issue_is_reported_for(@"
public class TestMe
    {
        public void DoSomething()
        {
            var " + s + @" = 1;
        }
    }
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_local_variable_named_(
                                                               [ValueSource(nameof(LowerCaseVowels))] char c1,
                                                               [ValueSource(nameof(LowerCaseConstants))] char c2)
            => No_issue_is_reported_for(@"
public class TestMe
    {
        public void DoSomething()
        {
            var " + c1 + c2 + @" = 1;
        }
    }
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_local_variable_named_(
                                                               [ValueSource(nameof(LowerCaseConstants))] char c1,
                                                               [ValueSource(nameof(LowerCaseConstants))] char c2)
            => An_issue_is_reported_for(@"
public class TestMe
    {
        public void DoSomething()
        {
            var " + c1 + c2 + @" = 1;
        }
    }
");

        protected override string GetDiagnosticId() => MiKo_1539_NoVowelInNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1539_NoVowelInNameAnalyzer();
    }
}