using System.Linq;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class AbbreviationFinderTests
    {
        private static readonly Pair[] Prefixes =
                                                  [
                                                      new("alt", "alternative"),
                                                      new("app", "application"),
                                                      new("appl", "application"),
                                                      new("appls", "applications"),
                                                      new("apps", "applications"),
                                                      new("arg", "argument"),
                                                      new("args", "arguments"),
                                                      new("arr", "array"),
                                                      new("assoc", "association"),
                                                      new("assocs", "associations"),
                                                      new("asynchron", "asynchronous"),
                                                      new("attr", "attribute"),
                                                      new("auth", "authorization"),
                                                      new("bk", "back"),
                                                      new("bmp", "bitmap"),
                                                      new("btn", "button"),
                                                      new("calc", "calculate"),
                                                      new("calib", "calibration"),
                                                      new("cb", "checkBox"),
                                                      new("cert", "certificate"),
                                                      new("cfg", "configuration"),
                                                      new("chk", "checkBox"),
                                                      new("cls", "class"),
                                                      new("cm", "contextMenu"),
                                                      new("cmb", "comboBox"),
                                                      new("cmd", "command"),
                                                      new("cmp", "comparison"),
                                                      new("col", "column"),
                                                      new("coll", "collection"),
                                                      new("comm", "communication"),
                                                      new("comp", "compile"),
                                                      new("compat", "compatible"),
                                                      new("conf", "configuration"),
                                                      new("config", "configuration"),
                                                      new("configs", "configurations"),
                                                      new("conn", "connection"),
                                                      new("conns", "connections"),
                                                      new("conv", "conversion"),
                                                      new("ctg", "category"),
                                                      new("ctl", "control"),
                                                      new("ctlg", "catalog"),
                                                      new("ctrl", "control"),
                                                      new("ctx", "context"),
                                                      new("cur", "current"),
                                                      new("db", "database"),
                                                      new("ddl", "dropDownList"),
                                                      new("decl", "declaration"),
                                                      new("decomp", "decomposition"),
                                                      new("decr", "decrypt"),
                                                      new("def", "definition"),
                                                      new("defs", "definitions"),
                                                      new("dep", "dependent"),
                                                      new("deps", "dependencies"),
                                                      new("desc", "description"),
                                                      new("dest", "destination"),
                                                      new("dev", "device"),
                                                      new("diag", "diagnostic"),
                                                      new("diags", "diagnostics"),
                                                      new("dic", "dictionary"),
                                                      new("dics", "dictionaries"),
                                                      new("dict", "dictionary"),
                                                      new("dicts", "dictionaries"),
                                                      new("diff", "difference"),
                                                      new("diffs", "differences"),
                                                      new("dir", "directory"),
                                                      new("dirs", "directories"),
                                                      new("dist", "distance"),
                                                      new("div", "division"),
                                                      new("dlg", "dialog"),
                                                      new("dlgt", "delegate"),
                                                      new("dm", string.Empty),
                                                      new("doc", "document"),
                                                      new("docs", "documents"),
                                                      new("docu", "documentation"),
                                                      new("docus", "documentations"),
                                                      new("dst", "destination"),
                                                      new("dto", string.Empty),
                                                      new("dyn", "dynamic"),
                                                      new("ed", "edit"),
                                                      new("el", "element"),
                                                      new("ele", "element"),
                                                      new("elem", "element"),
                                                      new("encr", "encrypt"),
                                                      new("env", "environment"),
                                                      new("environ", "environment"),
                                                      new("eq", "equal"),
                                                      new("err", "error"),
                                                      new("eval", "evaluation"),
                                                      new("evnt", "event"),
                                                      new("evt", "event"),
                                                      new("exec", "execute"),
                                                      new("ext", "extension"),
                                                      new("fnc", "function"),
                                                      new("frm", "form"),
                                                      new("fwd", "forwarded"),
                                                      new("geo", "geometry"),
                                                      new("hdls", "headless"),
                                                      new("his", "history"),
                                                      new("hist", "history"),
                                                      new("hlp", "help"),
                                                      new("horiz", "horizontal"),
                                                      new("horz", "horizontal"),
                                                      new("ident", "identification"),
                                                      new("idents", "identifications"),
                                                      new("idx", "index"),
                                                      new("idxs", "indices"),
                                                      new("imp", "implementation"),
                                                      new("impl", "implementation"),
                                                      new("init", "initialize"),
                                                      new("interv", "interval"),
                                                      new("intf", "interface"),
                                                      new("intfc", "interface"),
                                                      new("intrfc", "interface"),
                                                      new("itf", "interface"),
                                                      new("kvp", "pair"),
                                                      new("lang", "language"),
                                                      new("lb", "label"),
                                                      new("lbl", "label"),
                                                      new("len", "length"),
                                                      new("lib", "library"),
                                                      new("libs", "libraries"),
                                                      new("loc", "local"),
                                                      new("lv", "listView"),
                                                      new("lvw", "listView"),
                                                      new("man", "manager"),
                                                      new("max", "maximum"),
                                                      new("meth", "method"),
                                                      new("mgmt", "management"),
                                                      new("mgr", "manager"),
                                                      new("mgrs", "managers"),
                                                      new("min", "minimum"),
                                                      new("mngr", "manager"),
                                                      new("mngrs", "managers"),
                                                      new("mnu", "menuItem"),
                                                      new("mod", "modified"),
                                                      new("msg", "message"),
                                                      new("msgs", "messages"),
                                                      new("nav", "navigation"),
                                                      new("navig", "navigation"),
                                                      new("neg", "negative"),
                                                      new("nfy", "notificationIcon"),
                                                      new("num", "number"),
                                                      new("nums", "numbers"),
                                                      new("obj", "object"),
                                                      new("op", "operation"),
                                                      new("ops", "operations"),
                                                      new("opt", "option"),
                                                      new("opts", "options"),
                                                      new("para", "parameter"),
                                                      new("param", "parameter"),
                                                      new("params", "parameters"),
                                                      new("passwd", "password"),
                                                      new("pct", "picture"),
                                                      new("perc", "percentage"),
                                                      new("perf", "performance"),
                                                      new("phys", "physical"),
                                                      new("plausi", "plausibility"),
                                                      new("pnl", "panel"),
                                                      new("pos", "position"),
                                                      new("pow", "power"),
                                                      new("prev", "previous"),
                                                      new("proc", "process"),
                                                      new("procs", "processes"),
                                                      new("prop", "property"),
                                                      new("props", "properties"),
                                                      new("prot", "protected"),
                                                      new("pswd", "password"),
                                                      new("pt", "point"),
                                                      new("pts", "points"),
                                                      new("pw", "password"),
                                                      new("pwd", "password"),
                                                      new("qty", "quantity"),
                                                      new("rec", "record"),
                                                      new("rect", "rectangle"),
                                                      new("ref", "reference"),
                                                      new("refs", "references"),
                                                      new("rel", "relative"),
                                                      new("reloc", "relocation"),
                                                      new("repo", "repository"),
                                                      new("repos", "repositories"),
                                                      new("req", "request"),
                                                      new("res", "result"),
                                                      new("resp", "response"),
                                                      new("rest", "restore"),
                                                      new("rgn", "region"),
                                                      new("sem", "semantic"),
                                                      new("sep", "separator"),
                                                      new("sepa", "separator"),
                                                      new("seq", "sequential"),
                                                      new("sess", "session"),
                                                      new("spec", "specification"),
                                                      new("specs", "specifications"),
                                                      new("src", "source"),
                                                      new("srcs", "sources"),
                                                      new("srv", "service"),
                                                      new("std", "standard"),
                                                      new("str", "string"),
                                                      new("sts", "status"),
                                                      new("svc", "service"),
                                                      new("svr", "server"),
                                                      new("syn", "syntax"),
                                                      new("sync", "synchronization"),
                                                      new("synchron", "synchronous"),
                                                      new("sys", "system"),
                                                      new("tb", "textBox"),
                                                      new("tgt", "target"),
                                                      new("tgts", "targets"),
                                                      new("tm", "time"),
                                                      new("tmp", "temp"),
                                                      new("tmr", "timer"),
                                                      new("tvw", "treeView"),
                                                      new("txt", "text"),
                                                      new("txts", "texts"),
                                                      new("util", "utility"),
                                                      new("utils", "utilities"),
                                                      new("val", "value"),
                                                      new("var", "variable"),
                                                      new("vars", "variables"),
                                                      new("ver", "version"),
                                                      new("vert", "vertical"),
                                                      new("vm", "viewModel"),
                                                      new("vms", "viewModels"),
                                                      new("vol", "volume"),
                                                  ];

        private static readonly Pair[] Postfixes =
                                                   [
                                                          new("Alt", "Alternative"),
                                                          new("App", "Application"),
                                                          new("Appl", "Application"),
                                                          new("Appls", "Applications"),
                                                          new("Apps", "Applications"),
                                                          new("Arg", "Argument"),
                                                          new("Args", "Arguments"),
                                                          new("Arr", "Array"),
                                                          new("Assoc", "Association"),
                                                          new("Assocs", "Associations"),
                                                          new("Asynchron", "Asynchronous"),
                                                          new("Attr", "Attribute"),
                                                          new("Auth", "Authorization"),
                                                          new("Bk", "Back"),
                                                          new("Bl", "BusinessLogic"),
                                                          new("BL", "BusinessLogic"),
                                                          new("Bmp", "Bitmap"),
                                                          new("Btn", "Button"),
                                                          new("Btns", "Buttons"),
                                                          new("Calc", "Calculation"),
                                                          new("Calib", "Calibration"),
                                                          new("Cb", "CheckBox"),
                                                          new("Cert", "Certificate"),
                                                          new("Certs", "Certificates"),
                                                          new("Cfg", "Configuration"),
                                                          new("Chk", "CheckBox"),
                                                          new("Cli", "CommandLineInterface"),
                                                          new("CLI", "CommandLineInterface"),
                                                          new("Cls", "Class"),
                                                          new("Cmb", "ComboBox"),
                                                          new("Cmd", "Command"),
                                                          new("Cmp", "Comparison"),
                                                          new("Col", "Column"),
                                                          new("Coll", "Collection"),
                                                          new("Comm", "Communication"),
                                                          new("Comp", "Compile"),
                                                          new("Compat", "Compatibility"),
                                                          new("Conf", "Configuration"),
                                                          new("Config", "Configuration"),
                                                          new("Configs", "Configurations"),
                                                          new("Conn", "Connection"),
                                                          new("Conns", "Connections"),
                                                          new("Conv", "Conversion"),
                                                          new("Ctg", "Category"),
                                                          new("Ctl", "Control"),
                                                          new("Ctlg", "Catalog"),
                                                          new("Ctrl", "Control"),
                                                          new("Ctx", "Context"),
                                                          new("Cur", "Current"),
                                                          new("Db", "Database"),
                                                          new("Ddl", "DropDownList"),
                                                          new("Decl", "Declaration"),
                                                          new("Decomp", "Decomposition"),
                                                          new("Decr", "Decrypt"),
                                                          new("Def", "Definition"),
                                                          new("Defs", "Definitions"),
                                                          new("Dep", "Dependency"),
                                                          new("Deps", "Dependencies"),
                                                          new("Desc", "Description"),
                                                          new("Dest", "Destination"),
                                                          new("Dev", "Device"),
                                                          new("Diag", "Diagnostic"),
                                                          new("Diags", "Diagnostics"),
                                                          new("Dic", "Dictionary"),
                                                          new("Dics", "Dictionaries"),
                                                          new("Dict", "Dictionary"),
                                                          new("Dicts", "Dictionaries"),
                                                          new("Diff", "Difference"),
                                                          new("Diffs", "Differences"),
                                                          new("Dir", "Directory"),
                                                          new("Dirs", "Directories"),
                                                          new("Dist", "Distance"),
                                                          new("Div", "Division"),
                                                          new("Dlg", "Dialog"),
                                                          new("Dlgt", "Delegate"),
                                                          new("Dm", string.Empty),
                                                          new("DM", string.Empty),
                                                          new("Doc", "Document"),
                                                          new("Docs", "Documents"),
                                                          new("Docu", "Documentation"),
                                                          new("Docus", "Documentations"),
                                                          new("Dst", "Destination"),
                                                          new("Dto", string.Empty),
                                                          new("DTO", string.Empty),
                                                          new("Dyn", "Dynamic"),
                                                          new("Ed", "Edit"),
                                                          new("Ef", "EntityFramework"),
                                                          new("EF", "EntityFramework"),
                                                          new("El", "Element"),
                                                          new("Ele", "Element"),
                                                          new("Elem", "Element"),
                                                          new("Encr", "Encrypt"),
                                                          new("Env", "Environment"),
                                                          new("Environ", "Environment"),
                                                          new("Eq", "Equal"),
                                                          new("Err", "Error"),
                                                          new("Eval", "Evaluation"),
                                                          new("Evnt", "Event"),
                                                          new("Evt", "Event"),
                                                          new("Exec", "Execute"),
                                                          new("Ext", "Extension"),
                                                          new("Fnc", "Function"),
                                                          new("Frm", "Form"),
                                                          new("Fwd", "Forwarded"),
                                                          new("Geo", "Geometry"),
                                                          new("Hdls", "Headless"),
                                                          new("His", "History"),
                                                          new("Hist", "History"),
                                                          new("Hlp", "Help"),
                                                          new("Horiz", "Horizontal"),
                                                          new("Horz", "Horizontal"),
                                                          new("Ident", "Identification"),
                                                          new("Idents", "Identifications"),
                                                          new("Idx", "Index"),
                                                          new("Idxs", "Indices"),
                                                          new("Imp", "Implementation"),
                                                          new("Impl", "Implementation"),
                                                          new("Init", "Initialize"),
                                                          new("Interv", "Interval"),
                                                          new("Intf", "Interface"),
                                                          new("Intfc", "Interface"),
                                                          new("Intrfc", "Interface"),
                                                          new("Itf", "Interface"),
                                                          new("Lang", "Language"),
                                                          new("Lb", "Label"),
                                                          new("Lbl", "Label"),
                                                          new("Len", "Length"),
                                                          new("Lib", "Library"),
                                                          new("Libs", "Libraries"),
                                                          new("Loc", "Local"),
                                                          new("Lv", "ListView"),
                                                          new("Lvw", "ListView"),
                                                          new("Man", "Manager"),
                                                          new("Max", "Maximum"),
                                                          new("Meth", "Method"),
                                                          new("Mgmt", "Management"),
                                                          new("Mgr", "Manager"),
                                                          new("Mgrs", "Managers"),
                                                          new("Min", "Minimum"),
                                                          new("Mngr", "Manager"),
                                                          new("Mngrs", "Managers"),
                                                          new("Mnu", "MenuItem"),
                                                          new("Mod", "Modification"),
                                                          new("Msg", "Message"),
                                                          new("Nav", "Navigation"),
                                                          new("Navig", "Navigation"),
                                                          new("Neg", "Negative"),
                                                          new("Nfy", "NotificationIcon"),
                                                          new("Ns", "Namespace"),
                                                          new("Num", "Number"),
                                                          new("Obj", "Object"),
                                                          new("Objs", "Objects"),
                                                          new("Op", "Operation"),
                                                          new("Ops", "Operations"),
                                                          new("Opt", "Option"),
                                                          new("Opts", "Options"),
                                                          new("Para", "Parameter"),
                                                          new("Param", "Parameter"),
                                                          new("Params", "Parameters"),
                                                          new("Passwd", "Password"),
                                                          new("Pct", "Picture"),
                                                          new("Perc", "Percentage"),
                                                          new("Perf", "Performance"),
                                                          new("Phys", "Physical"),
                                                          new("Plausi", "Plausibility"),
                                                          new("Pnl", "Panel"),
                                                          new("Pos", "Position"),
                                                          new("Pow", "Power"),
                                                          new("Prev", "Previous"),
                                                          new("Proc", "Process"),
                                                          new("Procs", "Processes"),
                                                          new("Prop", "Property"),
                                                          new("Props", "Properties"),
                                                          new("Prot", "Protection"),
                                                          new("Pswd", "Password"),
                                                          new("Pt", "Point"),
                                                          new("Pts", "Points"),
                                                          new("Pw", "Password"),
                                                          new("Pwd", "Password"),
                                                          new("Qty", "Quantity"),
                                                          new("Rec", "Record"),
                                                          new("Rect", "Rectangle"),
                                                          new("Ref", "Reference"),
                                                          new("Refs", "References"),
                                                          new("Rel", "Relative"),
                                                          new("Reloc", "Relocation"),
                                                          new("Repo", "Repository"),
                                                          new("Repos", "Repositories"),
                                                          new("Req", "Request"),
                                                          new("Res", "Result"),
                                                          new("Resp", "Response"),
                                                          new("Rest", "Restore"),
                                                          new("Rgn", "Region"),
                                                          new("Sel", "Selection"),
                                                          new("Sem", "Semantic"),
                                                          new("Sep", "Separator"),
                                                          new("Sepa", "Separator"),
                                                          new("Seq", "Sequence"),
                                                          new("Sess", "Session"),
                                                          new("Spec", "Specification"),
                                                          new("Src", "Source"),
                                                          new("Srcs", "Sources"),
                                                          new("Srv", "Service"),
                                                          new("Std", "Standard"),
                                                          new("Str", "String"),
                                                          new("Sts", "Status"),
                                                          new("Svc", "Service"),
                                                          new("Svr", "Server"),
                                                          new("Syn", "Syntax"),
                                                          new("Sync", "Synchronization"),
                                                          new("Synchron", "Synchronous"),
                                                          new("Sys", "System"),
                                                          new("Tb", "TextBox"),
                                                          new("Tgt", "Target"),
                                                          new("Tgts", "Targets"),
                                                          new("Tm", "Time"),
                                                          new("Tmp", "Temp"),
                                                          new("Tmr", "Timer"),
                                                          new("Tvw", "TreeView"),
                                                          new("Txt", "Text"),
                                                          new("Util", "Utility"),
                                                          new("Utils", "Utilities"),
                                                          new("Val", "Value"),
                                                          new("Var", "Variable"),
                                                          new("Vars", "Variables"),
                                                          new("Ver", "Version"),
                                                          new("Vert", "Vertical"),
                                                          new("Vm", "ViewModel"),
                                                          new("VM", "ViewModel"),
                                                          new("Vms", "ViewModels"),
                                                          new("VMs", "ViewModels"),
                                                          new("Vol", "Volume"),
                                                   ];

        private static readonly Pair[] MidTerms = [.. Postfixes.Where(_ => _.Key is not ("Mod" or "Prot" or "Seq"))
                                                               .ConcatenatedWith(new Pair("Mod", "Modified"), new Pair("Prot", "Protected"),  new Pair("Seq", "Sequential"))];

        private static readonly Pair[] StandalonePrefixes = [.. Prefixes.Where(_ => _.Key is not ("obj" or "args"))];

        private static readonly Pair[] StandalonePostfixes = [.. Postfixes.Where(_ => _.Key is not ("Obj" or "Args"))];

        [Test]
        public static void Finds_prefix_abbreviation_in_([ValueSource(nameof(Prefixes))] Pair prefix)
        {
            var findings = AbbreviationFinder.Find(prefix.Key + "SomeName");

            Assert.That(findings.Length, Is.EqualTo(1), "different findings");
            Assert.That(findings[0].Value, Is.EqualTo(prefix.Value), "wrong finding");
        }

        [Test]
        public static void Finds_postfix_abbreviation_in_([ValueSource(nameof(Postfixes))] Pair postfix)
        {
            var findings = AbbreviationFinder.Find("someName" + postfix.Key);

            Assert.That(findings.Length, Is.EqualTo(1), "Different findings");
            Assert.That(findings[0].Value, Is.EqualTo(postfix.Value), "wrong finding");
        }

        [Test]
        public static void Finds_prefix_abbreviations_and_fixes_them_in_([ValueSource(nameof(Prefixes))] Pair prefix)
        {
            var replacement = AbbreviationFinder.FindAndReplaceAllAbbreviations(prefix.Key + "SomeName");

            Assert.That(replacement, Is.EqualTo(prefix.Value + "SomeName"));
        }

        [TestCase("some_op_name", ExpectedResult = "some_operation_name")]
        [TestCase("some_ops_name", ExpectedResult = "some_operations_name")]
        [TestCase("some_opt_name", ExpectedResult = "some_option_name")]
        [TestCase("some_opts_name", ExpectedResult = "some_options_name")]
        public static string Finds_midterm_abbreviations_with_underlines_and_fixes_them_in_(string value) => AbbreviationFinder.FindAndReplaceAllAbbreviations(value);

        [Test]
        public static void Finds_midterm_abbreviations_and_fixes_them_in_([ValueSource(nameof(MidTerms))] Pair midterm)
        {
            var replacement = AbbreviationFinder.FindAndReplaceAllAbbreviations("some" + midterm.Key + "Name");

            Assert.That(replacement, Is.EqualTo("some" + midterm.Value + "Name"));
        }

        [Test]
        public static void Finds_postfix_abbreviations_and_fixes_them_in_([ValueSource(nameof(Postfixes))] Pair postfix)
        {
            var replacement = AbbreviationFinder.FindAndReplaceAllAbbreviations("someName" + postfix.Key);

            Assert.That(replacement, Is.EqualTo("someName" + postfix.Value));
        }

        [Test]
        public static void Finds_standalone_prefix_abbreviation_and_fixes_them_in_([ValueSource(nameof(StandalonePrefixes))] Pair prefix)
        {
            var replacement = AbbreviationFinder.FindAndReplaceAllAbbreviations(prefix.Key);

            Assert.That(replacement, Is.EqualTo(prefix.Value));
        }

        [Test]
        public static void Finds_standalone_postfix_abbreviation_and_fixes_them_in_([ValueSource(nameof(StandalonePostfixes))] Pair postfix)
        {
            var replacement = AbbreviationFinder.FindAndReplaceAllAbbreviations(postfix.Key);

            Assert.That(replacement, Is.EqualTo(postfix.Value));
        }

        [TestCase("sepaMySepSepaStuff", ExpectedResult = "separatorMySeparatorSeparatorStuff")]
        [TestCase("sepaMysepSepStuff",  ExpectedResult = "separatorMyseparatorSeparatorStuff", Ignore = "Currently not fixed but very unlikely")]
        [TestCase("sepMysepaSepStuff",  ExpectedResult = "separatorMyseparatorSeparatorStuff")]
        [TestCase("sepMySepSepaStuff", ExpectedResult = "separatorMySeparatorSeparatorStuff")]
        public static string Finds_strange_combinations_and_fixes_them_in_(string value) => AbbreviationFinder.FindAndReplaceAllAbbreviations(value);

        [TestCase("adoptSomething")]
        [TestCase("args")]
        [TestCase("Args")]
        [TestCase("BIBLE")]
        [TestCase("BLOCK")]
        [TestCase("BLUE_SKY")]
        [TestCase("CLICK")]
        [TestCase("COMPATIBLE")]
        [TestCase("CYCLIC")]
        [TestCase("DEFAULT")]
        [TestCase("depthSomething")]
        [TestCase("directView")]
        [TestCase("DOUBLE")]
        [TestCase("EventArgs")]
        [TestCase("HOVER")]
        [TestCase("hovering")]
        [TestCase("hoverSomeName")]
        [TestCase("indirectView")]
        [TestCase("LEFT")]
        [TestCase("MyEventArgs")]
        [TestCase("obj")]
        [TestCase("Obj")]
        [TestCase("OVERLAY")]
        [TestCase("overSomeName")]
        [TestCase("preview")]
        [TestCase("Preview")]
        [TestCase("previewSomething")]
        [TestCase("REFACTOR")]
        [TestCase("REFACTORING")]
        [TestCase("REFER")]
        [TestCase("REFERENCE")]
        [TestCase("REFRESH")]
        [TestCase("REFRIGERATOR")]
        [TestCase("some_adoption_name")]
        [TestCase("some_depth_name")]
        [TestCase("SOME_HOVERCRAFT")]
        [TestCase("some_option_name")]
        [TestCase("someBlueValue")]
        [TestCase("someNameHover")]
        [TestCase("someNameOver")]
        [TestCase("someOverSomeName")]
        [TestCase("somePreview")]
        [TestCase("somethingDepth")]
        [TestCase("somethingToAdopt")]
        [TestCase("TABLE")]
        [TestCase("USEFUL")]
        [TestCase("VARIABLE")]
        public static void Ignores_(string value) => Assert.That(AbbreviationFinder.FindAndReplaceAllAbbreviations(value), Is.EqualTo(value));
    }
}