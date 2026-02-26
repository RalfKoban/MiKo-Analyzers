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
                                                           "alt",
                                                           "app",
                                                           "apps",
                                                           "assoc",
                                                           "auth",
                                                           "attr",
                                                           "btn",
                                                           "cb",
                                                           "cert",
                                                           "cfg",
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
                                                           "opts",
                                                           "param",
                                                           "params",
                                                           "perc",
                                                           "perf",
                                                           "phys",
                                                           "plausi",
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
                                                           "seq",
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
                                                           "Alt",
                                                           "App",
                                                           "Apps",
                                                           "Assoc",
                                                           "Attr",
                                                           "Auth",
                                                           "Btn",
                                                           "Cb",
                                                           "Cert",
                                                           "Cfg",
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
                                                           "El",
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
                                                           "Opt",
                                                           "Opts",
                                                           "Params",
                                                           "Perc",
                                                           "Perf",
                                                           "Phys",
                                                           "Plausi",
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
                                                           "Seq",
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

        private static readonly string[] BadPostfixes = [.. BadMidTerms.Union([
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
                                                                              ])];

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
                                                            "saltIsUsed",
                                                            "SomeSaltIsUsed",
                                                            "script",
                                                            "scripts",
                                                            "signCertificate",
                                                            "text",
                                                            "tires",

                                                            // languages
                                                            "ptPT", // Portugal
                                                            "ptBR", // Brazil
                                                            "lvLV", // Latvia
                                                        ];

        private static readonly string[] AllowedWords = [.. AllowedTerms, "obj", "href", "cref", "topLevelItem", "toplevelItem", "topMost", "topmost", "atTop"];

        private static readonly string[] WrongWords = [.. BadPrefixes.Except(AllowedWords)];

        [Test]
        public void No_issue_is_reported_for_code_without_abbreviations() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_well_known_abbreviation_as_prefix_([Values("MEF", "ALT")] string abbreviation) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int " + abbreviation + @"DoSomething();
    }
}");

        [Test]
        public void No_issue_is_reported_for_well_known_abbreviation_as_suffix_([Values("MEF", "ALT")] string abbreviation) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething" + abbreviation + @"();
    }
}");

        [Test]
        public void No_issue_is_reported_for_well_known_abbreviation_as_prefix_with_underscore_separator_([Values("MEF", "ALT")] string abbreviation) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int " + abbreviation + @"_DoSomething();
    }
}");

        [Test]
        public void No_issue_is_reported_for_well_known_abbreviation_as_suffix_with_underscore_separator_([Values("MEF", "ALT")] string abbreviation) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static int DoSomething_" + abbreviation + @"();
    }
}");

        [Test] // verifies that 'wParam' and 'lParam' which are used by Windows C++ API are not reported as abbreviations even though they actually are
        public void No_issue_is_reported_for_Windows_API_parameters_wParam_and_lParam() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IntPtr wParam, IntPtr lParam) { }
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_([ValueSource(nameof(AllowedWords))] string methodName) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_upper_case_suffix_([ValueSource(nameof(AllowedTerms))] string methodName) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_lower_case_suffix_([ValueSource(nameof(AllowedTerms))] string methodName) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_property_([ValueSource(nameof(AllowedWords))] string propertyName) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_([ValueSource(nameof(AllowedWords))] string variableName) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_variable_([ValueSource(nameof(WrongWords))] string variableName) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_foreach_variable_([ValueSource(nameof(WrongWords))] string variableName) => An_issue_is_reported_for(@"
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

        [TestCase("alt", "alternative", "alternative")]
        [TestCase("alt", "alternative", "alternatives")]
        [TestCase("app", "application", "application")]
        [TestCase("app", "application", "applications")]
        [TestCase("arg", "argument", "argument")]
        [TestCase("arg", "argument", "arguments")]
        [TestCase("arr", "array", "array")]
        [TestCase("assoc", "association", "association")]
        [TestCase("asynchron", "asynchronous", "asynchronous")]
        [TestCase("attr", "attribute", "attribute")]
        [TestCase("attr", "attribute", "attributes")]
        [TestCase("auth", "authorization", "authenticate")]
        [TestCase("auth", "authorization", "authenticated")]
        [TestCase("auth", "authorization", "authenticates")]
        [TestCase("auth", "authorization", "authenticating")]
        [TestCase("auth", "authorization", "authentication")]
        [TestCase("auth", "authorization", "authentications")]
        [TestCase("auth", "authorization", "authorization")]
        [TestCase("auth", "authorization", "authorizations")]
        [TestCase("auth", "authorization", "authorize")]
        [TestCase("auth", "authorization", "authorized")]
        [TestCase("auth", "authorization", "authorizes")]
        [TestCase("auth", "authorization", "authorizing")]
        [TestCase("calc", "calculate", "calculate")]
        [TestCase("calc", "calculate", "calculation")]
        [TestCase("cert", "certificate", "certificate")]
        [TestCase("cert", "certificate", "certificates")]
        [TestCase("comp", "compile", "compatible")]
        [TestCase("comp", "compile", "compile")]
        [TestCase("comp", "compile", "compiles")]
        [TestCase("compat", "compatible", "compatibilities")]
        [TestCase("compat", "compatible", "compatibility")]
        [TestCase("compat", "compatible", "compatible")]
        [TestCase("conf", "configuration", "configuration")]
        [TestCase("conf", "configuration", "configure")]
        [TestCase("config", "configuration", "configuration")]
        [TestCase("config", "configuration", "configure")]
        [TestCase("conn", "connection", "connect")]
        [TestCase("conn", "connection", "connected")]
        [TestCase("conn", "connection", "connecting")]
        [TestCase("conn", "connection", "connection")]
        [TestCase("cur", "current", "concurrency")]
        [TestCase("cur", "current", "concurrent")]
        [TestCase("cur", "current", "currency")]
        [TestCase("cur", "current", "current")]
        [TestCase("db", "database", "database")]
        [TestCase("decl", "declaration", "declaration")]
        [TestCase("decl", "declaration", "declarations")]
        [TestCase("decl", "declaration", "declare")]
        [TestCase("decl", "declaration", "declaring")]
        [TestCase("decr", "decrypt", "decrement")]
        [TestCase("decr", "decrypt", "decrementing")]
        [TestCase("decr", "decrypt", "decrements")]
        [TestCase("decr", "decrypt", "decrypt")]
        [TestCase("def", "definition", "define")]
        [TestCase("def", "definition", "defining")]
        [TestCase("def", "definition", "definition")]
        [TestCase("dep", "dependent", "depend")]
        [TestCase("dep", "dependent", "dependencies")]
        [TestCase("dep", "dependent", "dependency")]
        [TestCase("dep", "dependent", "dependent")]
        [TestCase("dep", "dependent", "depends")]
        [TestCase("desc", "description", "description")]
        [TestCase("desc", "description", "descriptions")]
        [TestCase("dest", "destination", "destination")]
        [TestCase("dest", "destination", "destinations")]
        [TestCase("diag", "diagnostic", "diagnosis")]
        [TestCase("diag", "diagnostic", "diagnostic")]
        [TestCase("diag", "diagnostic", "diagram")]
        [TestCase("dic", "dictionary", "dictionary")]
        [TestCase("dict", "dictionary", "dictionary")]
        [TestCase("diff", "difference", "differ")]
        [TestCase("diff", "difference", "difference")]
        [TestCase("diff", "difference", "differences")]
        [TestCase("diff", "difference", "differencing")]
        [TestCase("diff", "difference", "diffing")]
        [TestCase("dir", "directory", "directories")]
        [TestCase("dir", "directory", "directory")]
        [TestCase("div", "division", "divide")]
        [TestCase("div", "division", "dividing")]
        [TestCase("div", "division", "division")]
        [TestCase("doc", "document", "document")]
        [TestCase("doc", "document", "documentation")]
        [TestCase("docu", "documentation", "document")]
        [TestCase("docu", "documentation", "documentation")]
        [TestCase("ed", "edit", "edit")]
        [TestCase("ed", "edit", "edited")]
        [TestCase("ed", "edit", "editing")]
        [TestCase("el", "element", "element")]
        [TestCase("el", "element", "select")]
        [TestCase("ele", "element", "element")]
        [TestCase("ele", "element", "select")]
        [TestCase("elem", "element", "element")]
        [TestCase("encr", "encrypt", "encrypt")]
        [TestCase("env", "environment", "environment")]
        [TestCase("env", "environment", "environments")]
        [TestCase("environ", "environment", "environment")]
        [TestCase("environ", "environment", "environments")]
        [TestCase("eq", "equal", "equal")]
        [TestCase("eq", "equal", "require")]
        [TestCase("eq", "equal", "sequence")]
        [TestCase("err", "error", "error")]
        [TestCase("exec", "execute", "executable")]
        [TestCase("exec", "execute", "execute")]
        [TestCase("exec", "execute", "executing")]
        [TestCase("exec", "execute", "execution")]
        [TestCase("ext", "extension", "extend")]
        [TestCase("ext", "extension", "extends")]
        [TestCase("ext", "extension", "extension")]
        [TestCase("ext", "extension", "exterior")]
        [TestCase("ext", "extension", "extern")]
        [TestCase("horz", "horizontal", "horizontal")]
        [TestCase("ident", "identification", "identification")]
        [TestCase("ident", "identification", "identifier")]
        [TestCase("ident", "identification", "identify")]
        [TestCase("ident", "identification", "identity")]
        [TestCase("imp", "implementation", "implement")]
        [TestCase("imp", "implementation", "implementation")]
        [TestCase("imp", "implementation", "implementations")]
        [TestCase("imp", "implementation", "implementing")]
        [TestCase("imp", "implementation", "implements")]
        [TestCase("imp", "implementation", "impress")]
        [TestCase("impl", "implementation", "implement")]
        [TestCase("impl", "implementation", "implementation")]
        [TestCase("impl", "implementation", "implementations")]
        [TestCase("impl", "implementation", "implementing")]
        [TestCase("impl", "implementation", "implements")]
        [TestCase("init", "initialize", "initialize")]
        [TestCase("init", "initialize", "initializes")]
        [TestCase("lang", "language", "language")]
        [TestCase("len", "length", "length")]
        [TestCase("lib", "library", "libraries")]
        [TestCase("lib", "library", "library")]
        [TestCase("libs", "libraries", "libraries")]
        [TestCase("man", "manager", "manage")]
        [TestCase("man", "manager", "manager")]
        [TestCase("man", "manager", "managing")]
        [TestCase("max", "maximum", "maxi")]
        [TestCase("max", "maximum", "maximum")]
        [TestCase("meth", "method", "method")]
        [TestCase("min", "minimum", "mini")]
        [TestCase("min", "minimum", "minimum")]
        [TestCase("nav", "navigation", "navigation")]
        [TestCase("num", "number", "number")]
        [TestCase("num", "number", "numbers")]
        [TestCase("obj", "object", "object")]
        [TestCase("op", "operation", "operation")]
        [TestCase("opt", "option", "adopt", Ignore = "Currently unsure how to fix that")]
        [TestCase("opts", "options", "adopts", Ignore = "Currently unsure how to fix that")]
        [TestCase("para", "parameter", "parameter")]
        [TestCase("param", "parameter", "parameter")]
        [TestCase("perc", "percentage", "percent")]
        [TestCase("perc", "percentage", "percentage")]
        [TestCase("perf", "performance", "perform")]
        [TestCase("perf", "performance", "performance")]
        [TestCase("phys", "physical", "physical")]
        [TestCase("plausi", "plausibility", "plausibilities")]
        [TestCase("pos", "position", "position")]
        [TestCase("pow", "power", "power", Ignore = "Currently unsure how to fix that")]
        [TestCase("prev", "previous", "previous")]
        [TestCase("proc", "process", "procedure")]
        [TestCase("proc", "process", "procedures")]
        [TestCase("proc", "process", "process")]
        [TestCase("proc", "process", "processes")]
        [TestCase("prop", "property", "properties")]
        [TestCase("prop", "property", "property")]
        [TestCase("pt", "point", "adopt")]
        [TestCase("pts", "points", "adopts")]
        [TestCase("rec", "record", "direct")]
        [TestCase("rec", "record", "directory")]
        [TestCase("rec", "record", "record")]
        [TestCase("rec", "record", "rectangle")]
        [TestCase("rect", "rectangle", "direct")]
        [TestCase("rect", "rectangle", "directory")]
        [TestCase("rect", "rectangle", "rectangle")]
        [TestCase("ref", "reference", "refactor")]
        [TestCase("ref", "reference", "refactoring")]
        [TestCase("ref", "reference", "refactors")]
        [TestCase("ref", "reference", "reference")]
        [TestCase("ref", "reference", "references")]
        [TestCase("ref", "reference", "refers")]
        [TestCase("ref", "reference", "refresh")]
        [TestCase("rel", "relative", "relate")]
        [TestCase("rel", "relative", "related")]
        [TestCase("rel", "relative", "relative")]
        [TestCase("reloc", "relocation", "relocate")]
        [TestCase("reloc", "relocation", "relocating")]
        [TestCase("reloc", "relocation", "relocation")]
        [TestCase("repo", "repository", "repositories")]
        [TestCase("repo", "repository", "repository")]
        [TestCase("repos", "repositories", "repositories")]
        [TestCase("repos", "repositories", "repository")]
        [TestCase("req", "request", "request")]
        [TestCase("res", "result", "corresponding")]
        [TestCase("res", "result", "fires")]
        [TestCase("res", "result", "hires")]
        [TestCase("res", "result", "responding")]
        [TestCase("res", "result", "response")]
        [TestCase("res", "result", "responsible")]
        [TestCase("res", "result", "resting", Ignore = "Currently unsure how to fix that")]
        [TestCase("res", "result", "restoration")]
        [TestCase("res", "result", "restore")]
        [TestCase("res", "result", "restoring")]
        [TestCase("res", "result", "result")]
        [TestCase("resp", "response", "corresponding")]
        [TestCase("resp", "response", "responding")]
        [TestCase("resp", "response", "response")]
        [TestCase("resp", "response", "responsible")]
        [TestCase("rest", "restore", "resting", Ignore = "Currently unsure how to fix that")]
        [TestCase("rest", "restore", "restoration")]
        [TestCase("rest", "restore", "restore")]
        [TestCase("rest", "restore", "restoring")]
        [TestCase("sem", "semantic", "assemble")]
        [TestCase("sem", "semantic", "assembler")]
        [TestCase("sem", "semantic", "assembling")]
        [TestCase("sem", "semantic", "assembly")]
        [TestCase("sem", "semantic", "semantic")]
        [TestCase("seq", "sequential", "sequence")] // TODO RKN: What about upper case endings 'Sequence' ?
        [TestCase("seq", "sequential", "sequences")] // TODO RKN: What about upper case endings 'Sequence' ?
        [TestCase("seq", "sequential", "sequencing")] // TODO RKN: What about upper case endings 'Sequence' ?
        [TestCase("seq", "sequential", "sequential")] // TODO RKN: What about upper case endings 'Sequence' ?
        [TestCase("sess", "session", "session")]
        [TestCase("spec", "specification", "aspect")]
        [TestCase("spec", "specification", "specific")]
        [TestCase("spec", "specification", "specification")]
        [TestCase("spec", "specification", "specify")]
        [TestCase("syn", "syntax", "async")]
        [TestCase("syn", "syntax", "asynchronous")]
        [TestCase("syn", "syntax", "synchronization")]
        [TestCase("syn", "syntax", "synchronize")]
        [TestCase("syn", "syntax", "synchronizing")]
        [TestCase("syn", "syntax", "synchronous")]
        [TestCase("syn", "syntax", "syntax")]
        [TestCase("sync", "synchronization", "async")]
        [TestCase("sync", "synchronization", "asynchronous")]
        [TestCase("sync", "synchronization", "synchronization")]
        [TestCase("sync", "synchronization", "synchronize")]
        [TestCase("sync", "synchronization", "synchronizing")]
        [TestCase("sync", "synchronization", "synchronous")]
        [TestCase("synchron", "synchronous", "synchronous")]
        [TestCase("sys", "system", "system")]
        [TestCase("util", "utility", "utilities")]
        [TestCase("util", "utility", "utility")]
        [TestCase("utils", "utilities", "utilities")]
        [TestCase("val", "value", "value")]
        [TestCase("var", "variable", "variable")]
        [TestCase("var", "variable", "variables")]
        [TestCase("ver", "version", "hover")]
        [TestCase("ver", "version", "hovering")]
        [TestCase("ver", "version", "over")]
        [TestCase("ver", "version", "Over")]
        [TestCase("ver", "version", "version")]
        [TestCase("ver", "version", "versions")]
        [TestCase("vert", "vertical", "vertical")]
        [TestCase("vol", "volume", "volume")]
        public void Code_gets_fixed_for_method_by_expanding_abbreviation_(string originalName, string fixedName1, string fixedName2)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Some_#1#_word_in_#2#_word() => 42;
    }
}
}";

            VerifyCSharpFix(Template.Replace("#1#", originalName).Replace("#2#", fixedName2), Template.Replace("#1#", fixedName1).Replace("#2#", fixedName2));
        }

        [TestCase("Min", "Minimum")]
        [TestCase("MinLength", "MinimumLength")]
        [TestCase("MaxVer", "MaximumVersion")]
        [TestCase("Cur", "Current")]
        [TestCase("Prev", "Previous")]
        public void Code_gets_fixed_for_property_by_expanding_abbreviation_(string originalName, string fixedName)
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
        [TestCase("cur", "current")]
        [TestCase("prev", "previous")]
        public void Code_gets_fixed_for_foreach_variable_by_expanding_abbreviation_(string originalName, string fixedName)
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

        [TestCase("config", "configuration")]
        [TestCase("cur", "current")]
        [TestCase("prev", "previous")]
        public void Code_gets_fixed_for_field_by_expanding_abbreviation_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private int _###;
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [Ignore("Currently, renaming tuples does not work. See https://github.com/dotnet/roslyn/issues/14115")]
        [TestCase("lang", "language")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        public void Code_gets_fixed_for_tuple_element_by_expanding_abbreviation_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            static (string? ###) Extract(object o) => null;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("lang", "language")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        public void Code_gets_fixed_for_deconstructed_simplified_variable_by_expanding_abbreviation_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var (###) = Extract(null);

            static (string? result) Extract(object o) => null;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("lang", "language")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        public void Code_gets_fixed_for_deconstructed_variable_by_expanding_abbreviation_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            (var ###, var result) = Extract(null);

            static (string? a, string? b) Extract(object o)
            {
                return (o?.ToString(), o?.ToString());
            }
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("cert", "certificate")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        [TestCase("lang", "language")]
        public void Code_gets_fixed_for_variable_in_is_declaration_pattern_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private object Data;

        public void DoSomething()
        {
            if (Data is { } ###)
            {
            }
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("cert", "certificate")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        [TestCase("lang", "language")]
        public void Code_gets_fixed_for_variable_in_list_pattern_(string originalName, string fixedName)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        private List<int> Data;

        public void DoSomething()
        {
            if (Data is not [ _, var ###, _ ])
            {
            }
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("cert", "certificate")]
        [TestCase("decl", "declaration")]
        [TestCase("impl", "implementation")]
        [TestCase("lang", "language")]
        public void Code_gets_fixed_for_variable_in_property_pattern_with_variable_declaration_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private TestMe Data;

        public int State { get; set; }

        public void DoSomething()
        {
            if (Data is { State: var ### } state)
            {
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