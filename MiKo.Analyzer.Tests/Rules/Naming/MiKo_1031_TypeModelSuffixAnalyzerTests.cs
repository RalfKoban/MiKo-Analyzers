using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1031_TypeModelSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("class", "SemanticModel")]
        [TestCase("class", "Something")]
        [TestCase("class", "SomethingViewModel")]
        [TestCase("class", "SomethingViewModels")]
        [TestCase("interface", "ISomething")]
        [TestCase("interface", "ISomethingViewModel")]
        [TestCase("interface", "ISomethingViewModels")]
        public void No_issue_is_reported_for_(string type, string name) => No_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [TestCase("interface", "ISomethingModel")]
        [TestCase("interface", "ISomethingModels")]
        [TestCase("class", "SomethingModel")]
        [TestCase("class", "SomethingModels")]
        public void An_issue_is_reported_for_(string type, string name) => An_issue_is_reported_for(@"
public " + type + " " + name + @"
{
}
");

        [TestCase("abstract class", "Binder")]
        [TestCase("abstract class", "ModelBinder")]
        [TestCase("interface", "Binder")]
        [TestCase("interface", "ModelBinder")]
        public void No_issue_is_reported_for_special_type_(string type, string name) => No_issue_is_reported_for(@"
using Microsoft.AspNetCore.Mvc.ModelBinding;

public " + type + " " + name + @" : IModelBinder
{
}
");

        protected override string GetDiagnosticId() => MiKo_1031_TypeModelSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1031_TypeModelSuffixAnalyzer();
    }
}