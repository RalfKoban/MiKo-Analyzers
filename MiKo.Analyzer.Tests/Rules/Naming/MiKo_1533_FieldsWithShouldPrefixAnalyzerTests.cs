using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1533_FieldsWithShouldPrefixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = Constants.Markers.FieldPrefixes;

        private static readonly string[] Prefixes = ["shall", "should", "will", "would", "could"];

        private static readonly TestCaseData[] CodeFixData = [.. CreateCodeFixData()];

        [Test]
        public void No_issue_is_reported_for_field_without_should_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int something;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_prefix_([ValueSource(nameof(FieldPrefixes))] string prefix, [ValueSource(nameof(Prefixes))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private int " + prefix + name + @"Something;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_field_([ValueSource(nameof(CodeFixData))] TestCaseData data)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private int ###;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", data.Wrong), Template.Replace("###", data.Fixed));
        }

        protected override string GetDiagnosticId() => MiKo_1533_FieldsWithShouldPrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1533_FieldsWithShouldPrefixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1533_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<TestCaseData> CreateCodeFixData()
        {
            Pair[] pairs =
                           [
                               new("couldBeNotOnline", "isNotOnline"),
                               new("couldBeOnline", "isOnline"),
                               new("couldConnect", "connect"),
                               new("couldHaveState", "hasState"),
                               new("couldNotBeOnline", "isNotOnline"),
                               new("shallBeOnline", "isOnline"),
                               new("shallConnect", "connect"),
                               new("shallHaveState", "hasState"),
                               new("shallNotConnect", "notConnect"),
                               new("shouldBeNotOnline", "isNotOnline"),
                               new("shouldBeOnline", "isOnline"),
                               new("shouldConnect", "connect"),
                               new("shouldHaveState", "hasState"),
                               new("shouldNotBeOnline", "isNotOnline"),
                               new("willBeOnline", "isOnline"),
                               new("willConnect", "connect"),
                               new("willHaveState", "hasState"),
                               new("willNotConnect", "notConnect"),
                               new("wouldBeNotOnline", "isNotOnline"),
                               new("wouldBeOnline", "isOnline"),
                               new("wouldConnect", "connect"),
                               new("wouldHaveState", "hasState"),
                               new("wouldNotBeOnline", "isNotOnline"),
                               new("wouldNotHaveState", "hasNotState"),
                           ];

            foreach (var prefix in FieldPrefixes)
            {
                foreach (var pair in pairs)
                {
                    yield return new TestCaseData
                                     {
                                         Wrong = prefix + pair.Key,
                                         Fixed = prefix + pair.Value,
                                     };
                }
            }
        }
    }
}