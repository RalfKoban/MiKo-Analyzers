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
    public sealed class MiKo_1510_FieldsWithStructuralDesignPatternSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] FieldPrefixes = Constants.Markers.FieldPrefixes;

        private static readonly string[] PatternNames = ["Adapter", "Wrapper", "Decorator"];

        private static readonly TestCaseData[] CodeFixData = [.. CreateCodeFixData()];

        [Test]
        public void No_issue_is_reported_for_field_without_pattern_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int something;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_suffix_([ValueSource(nameof(FieldPrefixes))] string prefix, [ValueSource(nameof(PatternNames))] string name) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        private int " + prefix + "something" + name + @";
    }
}
");

        [Test]
        public void Code_gets_fixed_for_field_with_([ValueSource(nameof(CodeFixData))] TestCaseData data)
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

        protected override string GetDiagnosticId() => MiKo_1510_FieldsWithStructuralDesignPatternSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1510_FieldsWithStructuralDesignPatternSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1510_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<TestCaseData> CreateCodeFixData()
        {
            Pair[] pairs =
                           [
                               new("adapter", "adapted"),
                               new("wrapper", "wrapped"),
                               new("decorator", "decorated"),
                               new("dataAdapter", "adaptedData"),
                               new("dataWrapper", "wrappedData"),
                               new("dataDecorator", "decoratedData")
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