using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NCrunch.Framework;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture, Isolated]
    public sealed partial class MiKo_1063_AbbreviationsInNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BadPrefixes =
            {
                "btn",
                "cb",
                "cmd",
                "ddl",
                "decl",
                "desc",
                "dict",
                "dir",
                "doc",
                "idx",
                "itf",
                "lbl",
                "max",
                "mgr",
                "min",
                "mngr",
                "msg",
                "num",
                "param",
                "params",
                "pos",
                "proc",
                "procs",
                "propName",
                "pt",
                "pts",
                "res",
                "std",
                "str",
                "tmp",
                "txt",
            };

        private static readonly string[] BadMidTerms =
            {
                "Btn",
                "Cb",
                "Cmd",
                "Ddl",
                "Decl",
                "Desc",
                "Dict",
                "Dir",
                "Doc",
                "Idx",
                "Itf",
                "Lbl",
                "Max",
                "Mgr",
                "Mngr",
                "Min",
                "Msg",
                "Num",
                "Pos",
                "Params",
                "Proc",
                "Procs",
                "PropName",
                "Pt",
                "Pts",
                "Res",
                "Std",
                "Tmp",
                "Txt",
            };

        private static readonly string[] BadPostfixes = BadMidTerms
                                                        .Concat(new[]
                                                                    {
                                                                        "BL",
                                                                        "Bl",
                                                                        "Err",
                                                                        "Param",
                                                                        "Params",
                                                                        "Proc",
                                                                        "Prop",
                                                                        "Props",
                                                                        "PropName",
                                                                        "PropNames",
                                                                        "Pos",
                                                                        "VM",
                                                                        "Vm",
                                                                    })
                                                        .Distinct()
                                                        .ToArray();

        private static readonly string[] AllowedTerms =
            {
                "accept",
                "acceptName",
                "adopt",
                "adoptWhatever",
                "allowedFeatures",
                "corrupt",
                "corruptNumber",
                "declared",
                "document",
                "doctor",
                "except",
                "firmwares",
                "firstNumber",
                "fixtures",
                "measures",
                "mixtures",
                "number",
                "tires",
                "Enum",
                "Enumeration",
            };

        [Test]
        public void No_issue_is_reported_for_properly_named_code() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        public TestMe(int i)
        {
            m_i = i;
        }

        public event EventHandler Raised;

        public string Name { get; set; }

        public int DoSomething()
        {
            var x = 42;
            return x;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_properly_named_variable_([ValueSource(nameof(AllowedTerms))] string variableName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var " + variableName + @" = 42;
            return " + variableName + @";
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_variable_([ValueSource(nameof(BadPrefixes))] string variableName) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var @" + variableName + @" = 42;
            return @" + variableName + @";
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_foreach_variable_([ValueSource(nameof(BadPrefixes))] string variableName) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int[] variables)
        {
            foreach (var @" + variableName + @" in variables)
            {
                return @" + variableName + @";
            }
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_extern_method_([ValueSource(nameof(BadPrefixes))] string part) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static extern int " + part + @"DoSomething();
    }
}");

        protected override string GetDiagnosticId() => MiKo_1063_AbbreviationsInNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1063_AbbreviationsInNameAnalyzer();
    }
}