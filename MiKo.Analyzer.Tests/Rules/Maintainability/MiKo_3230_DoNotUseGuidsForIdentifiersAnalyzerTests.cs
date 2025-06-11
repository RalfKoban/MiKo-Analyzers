using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3230_DoNotUseGuidsForIdentifiersAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongPropertyNames = ["SomeId", "SomeID", "ID", "Id", "SomeIdentifier", "Identifier", "SomeIdentifer", "Identifer"]; // contains typos to test for as well
        private static readonly string[] WrongParameterNames = ["someId", "someID", "id", "someIdentifier", "identifier", "someIdentifer", "identifer"]; // contains typos to test for as well

        [Test]
        public void No_issue_is_reported_for_type_named_([ValueSource(nameof(WrongPropertyNames))] string name) => No_issue_is_reported_for(@"
using System;

public class " + name + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_field_named_([ValueSource(nameof(WrongParameterNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private Guid _" + name + @"
}
");

        [Test]
        public void No_issue_is_reported_for_Non_Guid_property_named_([ValueSource(nameof(WrongPropertyNames))] string name) => No_issue_is_reported_for(@"
using System;

public record Identifier;

public class TestMe
{
    public Identifier " + name + @" { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_Non_Guid_parameter_named_([ValueSource(nameof(WrongParameterNames))] string name) => No_issue_is_reported_for(@"
using System;

public record Identifier;

public class TestMe
{
    public void DoSomething(Identifier " + name + @")
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Nullable_Non_Guid_property_named_([ValueSource(nameof(WrongPropertyNames))] string name) => No_issue_is_reported_for(@"
using System;

public record Identifier;

public class TestMe
{
    public Identifier? " + name + @" { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_Nullable_Non_Guid_parameter_named_([ValueSource(nameof(WrongParameterNames))] string name) => No_issue_is_reported_for(@"
using System;

public record Identifier;

public class TestMe
{
    public void DoSomething(Identifier? " + name + @")
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Guid_property_named_([ValueSource(nameof(WrongPropertyNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public Guid " + name + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_Guid_parameter_named_([ValueSource(nameof(WrongParameterNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Guid " + name + @")
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Nullable_Guid_property_named_([ValueSource(nameof(WrongPropertyNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public Guid? " + name + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_Nullable_Guid_parameter_named_([ValueSource(nameof(WrongParameterNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Guid? " + name + @")
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3230_DoNotUseGuidsForIdentifiersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3230_DoNotUseGuidsForIdentifiersAnalyzer();
    }
}