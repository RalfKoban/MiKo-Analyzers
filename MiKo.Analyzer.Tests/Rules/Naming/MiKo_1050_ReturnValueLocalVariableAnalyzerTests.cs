using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1050_ReturnValueLocalVariableAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Fitting =
                                                   [
                                                       "myVariable",
                                                       "result",
                                                       "results",
                                                   ];

        private static readonly string[] NonFitting =
                                                      [
                                                          "res1",
                                                          "res2",
                                                          "resultingList",
                                                          "resultList",
                                                          "ret",
                                                          "ret1",
                                                          "ret2",
                                                          "retGuid",
                                                          "retList",
                                                          "retMock",
                                                          "retMockVm",
                                                          "returnCommunicationChannel",
                                                          "returned",
                                                          "returningList",
                                                          "returnVal",
                                                          "returnVals",
                                                          "returnValue",
                                                          "returnValue4",
                                                          "returnValues",
                                                          "retval",
                                                          "retVal",
                                                          "retVal_5",
                                                          "retVal3",
                                                          "retValid",
                                                          "retVals",
                                                      ];

        [Test]
        public void No_issue_is_reported_for_variable_with_fitting_name_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_non_fitting_name_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_with_fitting_name_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_with_non_fitting_name_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(NonFitting))] string name)
        {
            const string Template = @"
public class TestMe
{
    public int DoSomething()
    {
        var ### = 42;
        return ###; 
    }
}
";

            VerifyCSharpFix(Template.Replace("###", name), Template.Replace("###", "result"));
        }

        protected override string GetDiagnosticId() => MiKo_1050_ReturnValueLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1050_ReturnValueLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1050_CodeFixProvider();
    }
}