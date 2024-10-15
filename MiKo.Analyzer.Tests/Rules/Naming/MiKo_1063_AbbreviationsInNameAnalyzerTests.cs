using System;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed partial class MiKo_1063_AbbreviationsInNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BadPrefixes =
                                                       [
                                                           "app",
                                                           "apps",
                                                           "assoc",
                                                           "auth",
                                                           "btn",
                                                           "cb",
                                                           "cert",
                                                           "chk",
                                                           "cls",
                                                           "cmb",
                                                           "cmd",
                                                           "conf",
                                                           "config",
                                                           "configs",
                                                           "conn",
                                                           "ctg",
                                                           "ctl",
                                                           "ctlg",
                                                           "ctrl",
                                                           "ctx",
                                                           "db",
                                                           "ddl",
                                                           "decl",
                                                           "decr",
                                                           "def",
                                                           "desc",
                                                           "dest",
                                                           "diag",
                                                           "diags",
                                                           "dict",
                                                           "diff",
                                                           "diffs",
                                                           "dir",
                                                           "dlg",
                                                           "doc",
                                                           "dst",
                                                           "dto",
                                                           "encr",
                                                           "env",
                                                           "environ",
                                                           "err",
                                                           "ext",
                                                           "frm",
                                                           "hdls",
                                                           "ident",
                                                           "idx",
                                                           "init",
                                                           "itf",
                                                           "lang",
                                                           "lbl",
                                                           "lib",
                                                           "libs",
                                                           "lv",
                                                           "max",
                                                           "methName",
                                                           "mgmt",
                                                           "mgr",
                                                           "min",
                                                           "mngr",
                                                           "mnu",
                                                           "msg",
                                                           "num",
                                                           "obj",
                                                           "param",
                                                           "params",
                                                           "perc",
                                                           "perf",
                                                           "phys",
                                                           "pos",
                                                           "pow",
                                                           "proc",
                                                           "procs",
                                                           "propName",
                                                           "pt",
                                                           "pts",
                                                           "qty",
                                                           "ref",
                                                           "repo",
                                                           "req",
                                                           "res",
                                                           "resp",
                                                           "sem",
                                                           "spec",
                                                           "src",
                                                           "std",
                                                           "str",
                                                           "sync",
                                                           "svc",
                                                           "tm",
                                                           "tmp",
                                                           "txt",
                                                           "var",
                                                           "ver",
                                                           "vol",
                                                       ];

        private static readonly string[] BadMidTerms =
                                                       [
                                                           "App",
                                                           "Apps",
                                                           "Assoc",
                                                           "Auth",
                                                           "Btn",
                                                           "Cb",
                                                           "Cert",
                                                           "Chk",
                                                           "Cli",
                                                           "Cls",
                                                           "Cmb",
                                                           "Cmd",
                                                           "Conf",
                                                           "Config",
                                                           "Configs",
                                                           "Conn",
                                                           "Ctg",
                                                           "Ctl",
                                                           "Ctlg",
                                                           "Ctrl",
                                                           "Ctx",
                                                           "Db",
                                                           "Ddl",
                                                           "Def",
                                                           "Decl",
                                                           "Decr",
                                                           "Desc",
                                                           "Dest",
                                                           "Diag",
                                                           "Diags",
                                                           "Dict",
                                                           "Diff",
                                                           "Diffs",
                                                           "Dir",
                                                           "Dlg",
                                                           "Doc",
                                                           "Dst",
                                                           "Ef",
                                                           "Encr",
                                                           "Env",
                                                           "Environ",
                                                           "Err",
                                                           "Ext",
                                                           "Frm",
                                                           "Hdls",
                                                           "Ident",
                                                           "Idx",
                                                           "Init",
                                                           "Itf",
                                                           "Lang",
                                                           "Lbl",
                                                           "Lib",
                                                           "Libs",
                                                           "Lv",
                                                           "Max",
                                                           "MethName",
                                                           "Mgmt",
                                                           "Mgr",
                                                           "Min",
                                                           "Mngr",
                                                           "Mnu",
                                                           "Msg",
                                                           "Num",
                                                           "Obj",
                                                           "Op",
                                                           "Params",
                                                           "Perc",
                                                           "Perf",
                                                           "Phys",
                                                           "Pos",
                                                           "Pow",
                                                           "Proc",
                                                           "Procs",
                                                           "PropName",
                                                           "Pt",
                                                           "Pts",
                                                           "Qty",
                                                           "Repo",
                                                           "Ref",
                                                           "Req",
                                                           "Res",
                                                           "Resp",
                                                           "Sem",
                                                           "Spec",
                                                           "Src",
                                                           "Std",
                                                           "Sync",
                                                           "Svc",
                                                           "Tm",
                                                           "Tmp",
                                                           "Txt",
                                                           "Var",
                                                           "Ver",
                                                           "Vol",
                                                       ];

        private static readonly string[] BadPostfixes = BadMidTerms
                                                        .Union([
                                                                   "Bl",
                                                                   "BL",
                                                                   "CLI",
                                                                   "Dto",
                                                                   "DTO",
                                                                   "Itf",
                                                                   "Meth",
                                                                   "Param",
                                                                   "Params",
                                                                   "Pos",
                                                                   "Proc",
                                                                   "Prop",
                                                                   "PropName",
                                                                   "PropNames",
                                                                   "Props",
                                                                   "Vm",
                                                                   "VM",
                                                               ])
                                                        .ToArray();

        private static readonly string[] AllowedTerms =
                                                        [
                                                            "accept",
                                                            "acceptName",
                                                            "accepts",
                                                            "acceptsName",
                                                            "adopt",
                                                            "adopts",
                                                            "adoptsWhatever",
                                                            "adoptWhatever",
                                                            "allowedFeatures",
                                                            "asyncGoOnline",
                                                            "attempt",
                                                            "attempts",
                                                            "authenticate",
                                                            "authenticates",
                                                            "authorize",
                                                            "authorizes",
                                                            "corrupt",
                                                            "corruptNumber",
                                                            "corrupts",
                                                            "corruptsNumber",
                                                            "declared",
                                                            "decrypt",
                                                            "doctor",
                                                            "document",
                                                            "effort",
                                                            "encrypt",
                                                            "enum",
                                                            "enumeration",
                                                            "environment",
                                                            "except",
                                                            "firmwares",
                                                            "firstNumber",
                                                            "fixtures",
                                                            "httpRequest",
                                                            "httpResponse",
                                                            "identifiable",
                                                            "identification",
                                                            "identifier",
                                                            "identities",
                                                            "identity",
                                                            "isKept",
                                                            "kept",
                                                            "measures",
                                                            "inTheMidstOfTheNight",
                                                            "mixtures",
                                                            "next",
                                                            "number",
                                                            "onClick",
                                                            "OAuth",
                                                            "OAuth1",
                                                            "OAuth2",
                                                            "prompt",
                                                            "requestTime",
                                                            "responseTime",
                                                            "script",
                                                            "scripts",
                                                            "signCertificate",
                                                            "text",
                                                            "tires",
                                                        ];

        private static readonly string[] AllowedWords = [.. AllowedTerms, "obj", "href", "cref"];

        private static readonly string[] WrongWords = BadPrefixes.Except(AllowedWords).ToArray();

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
        public void No_issue_is_reported_for_well_known_abbreviation_([Values("MEF")] string abbreviation) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int " + abbreviation + @"DoSomething();
    }
}");

        [Test] // verifies that 'wParam' and 'lParam' which are used by Windows C++ API are not reported as abbreviations even though they actually are
        public void No_issue_is_reported_for_special_parameters_wParam_and_lParam() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IntPtr wParam, IntPtr lParam) { }
    }
}");

        [Test]
        public void No_issue_is_reported_for_properly_named_method_([ValueSource(nameof(AllowedWords))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int " + methodName.ToUpperCaseAt(0) + @"()
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_properly_named_method_with_upper_case_suffix_([ValueSource(nameof(AllowedTerms))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething" + methodName.ToUpperCaseAt(0) + @"()
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_properly_named_method_with_lower_case_suffix_([ValueSource(nameof(AllowedTerms))] string methodName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething_" + methodName.ToLowerCaseAt(0) + @"()
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_properly_named_property_([ValueSource(nameof(AllowedWords))] string propertyName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int " + propertyName.ToUpperCaseAt(0) + @" { get; set; }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_properly_named_variable_([ValueSource(nameof(AllowedWords))] string variableName) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_named_variable_([ValueSource(nameof(WrongWords))] string variableName) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_named_foreach_variable_([ValueSource(nameof(WrongWords))] string variableName) => An_issue_is_reported_for(@"
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

        [TestCase("Min", "Minimum")]
        [TestCase("MinLength", "MinimumLength")]
        [TestCase("MaxVer", "MaximumVersion")]
        public void Code_gets_fixed_for_incorrectly_named_property_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int ### { get; set; }
    }
}
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("app", "application")]
        [TestCase("appVariable", "applicationVariable")]
        [TestCase("appVar", "applicationVariable")]
        public void Code_gets_fixed_for_incorrectly_named_foreach_variable_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int[] variables)
        {
            foreach (var ### in variables)
            {
                return ###;
            }
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1063_AbbreviationsInNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1063_AbbreviationsInNameAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1063_CodeFixProvider();
    }
}