using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1543_FactoriesWithPropertiesOnlyAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CreationTypeNames = ["TestMeFactory", "TestMeBuilder"];

        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_ordinary_type_with_only_properties() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string Name { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_methods_([ValueSource(nameof(CreationTypeNames))] string name) => No_issue_is_reported_for(@"
using System;

public class " + name + @"
{
    public string CreateName() => string.Empty;
}
");

        [Test]
        public void No_issue_is_reported_for_interface_with_methods_([ValueSource(nameof(CreationTypeNames))] string name) => No_issue_is_reported_for(@"
using System;

public interface " + name + @"
{
    string CreateName();
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_only_properties_([ValueSource(nameof(CreationTypeNames))] string name) => An_issue_is_reported_for(@"
using System;

public class " + name + @"
{
    public string Name { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_interface_with_only_properties_([ValueSource(nameof(CreationTypeNames))] string name) => An_issue_is_reported_for(@"
using System;

public interface " + name + @"
{
    string Name { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1543_FactoriesWithPropertiesOnlyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1543_FactoriesWithPropertiesOnlyAnalyzer();
    }
}