using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1064_ParameterNameMeaningAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(TestMe culprit)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_overridden_method_with_parameter_named_after_type_and_some_uppercase_letters() => No_issue_is_reported_for(@"
public class TestMe
{
    public override void DoSomething(TestMe testMe)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_type_and_only_lowercase_letters() => No_issue_is_reported_for(@"
public class Side
{
    public void DoSomething(Side side)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_property_and_used_in_ctor() => No_issue_is_reported_for(@"
public enum Side
{
}

public class TestMe
{
    public TestMe(Side side) => Side = side;

    public Side Side { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_property_and_used_in_ctor_with_Compound_word() => No_issue_is_reported_for(@"
public enum TestMeSide
{
}

public class TestMe
{
    public TestMe(TestMeSide testMeSide) => TestMeSide = testMeSide;

    public TestMeSide TestMeSide { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_parameter_named_after_field_and_used_in_ctor_with_Compound_word() => No_issue_is_reported_for(@"
public enum TestMeSide
{
}

public class TestMe
{
    private readonly TestMeSide m_testMeSide;
    
    public TestMe(TestMeSide testMeSide) => m_testMeSide = testMeSide;
}
");

        [TestCase("CancellationToken cancellationToken")]
        [TestCase("IFormatProvider formatProvider")]
        [TestCase("SemanticModel semanticModel")]
        [TestCase("ICommand command")]
        [TestCase("IProject project")]
        [TestCase("IProgress progress")]
        [TestCase("ISymbol symbol")]
        public void No_issue_is_reported_for_known_name_(string parameter) => No_issue_is_reported_for(@"
using System;
using System.Threading;

using Microsoft.CodeAnalysis;

public class TestMe
{
    public void DoSomething(" + parameter + @")
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Manager() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public interface ISomethingManager
{
}

public class TestMe
{
    public void DoSomething(ISomethingManager somethingManager)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_named_after_type_and_some_uppercase_letters() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(TestMe testMe)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_named_after_interface_type() => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe
{
    public void DoSomething(ITestMe testMe)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_exactly_named_after_interface_type() => An_issue_is_reported_for(@"
public interface ITestMe
{
}

public class TestMe
{
    public void DoSomething(ITestMe iTestMe)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_used_in_ctor_with_Compound_word_and_no_matching_field() => An_issue_is_reported_for(@"
public enum TestMeSide
{
}

public class TestMe
{
    public TestMe(TestMeSide testMeSide)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1064_ParameterNameMeaningAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1064_ParameterNameMeaningAnalyzer();
    }
}