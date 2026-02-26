using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides functionality to find abbreviations in text and replace them with their full terms.
    /// </summary>
    internal static class AbbreviationFinder
    {
        private static readonly Pair[] Prefixes =
                                                  {
                                                      new Pair("alt", "alternative"),
                                                      new Pair("app", "application"),
                                                      new Pair("apps", "applications"),
                                                      new Pair("arg", "argument"),
                                                      new Pair("args", "arguments"),
                                                      new Pair("arr", "array"),
                                                      new Pair("assoc", "association"),
                                                      new Pair("assocs", "associations"),
                                                      new Pair("asynchron", "asynchronous"),
                                                      new Pair("attr", "attribute"),
                                                      new Pair("auth", "authorization"),
                                                      new Pair("btn", "button"),
                                                      new Pair("calc", "calculate"),
                                                      new Pair("cb", "checkBox"),
                                                      new Pair("cert", "certificate"),
                                                      new Pair("cfg", "configuration"),
                                                      new Pair("chk", "checkBox"),
                                                      new Pair("cls", "class"),
                                                      new Pair("cmb", "comboBox"),
                                                      new Pair("cmd", "command"),
                                                      new Pair("comp", "compile"),
                                                      new Pair("compat", "compatible"),
                                                      new Pair("conf", "configuration"),
                                                      new Pair("config", "configuration"),
                                                      new Pair("configs", "configurations"),
                                                      new Pair("conn", "connection"),
                                                      new Pair("ctg", "category"),
                                                      new Pair("ctl", "control"),
                                                      new Pair("ctlg", "catalog"),
                                                      new Pair("ctrl", "control"),
                                                      new Pair("ctx", "context"),
                                                      new Pair("cur", "current"),
                                                      new Pair("db", "database"),
                                                      new Pair("ddl", "dropDownList"),
                                                      new Pair("decl", "declaration"),
                                                      new Pair("decr", "decrypt"),
                                                      new Pair("def", "definition"),
                                                      new Pair("defs", "definitions"),
                                                      new Pair("dep", "dependent"),
                                                      new Pair("deps", "dependencies"),
                                                      new Pair("desc", "description"),
                                                      new Pair("dest", "destination"),
                                                      new Pair("diag", "diagnostic"),
                                                      new Pair("diags", "diagnostics"),
                                                      new Pair("dic", "dictionary"),
                                                      new Pair("dict", "dictionary"),
                                                      new Pair("diff", "difference"),
                                                      new Pair("diffs", "differences"),
                                                      new Pair("dir", "directory"),
                                                      new Pair("dirs", "directories"),
                                                      new Pair("div", "division"),
                                                      new Pair("dlg", "dialog"),
                                                      new Pair("dlgt", "delegate"),
                                                      new Pair("dm", string.Empty), // 'dm' means 'Domain Model'
                                                      new Pair("doc", "document"),
                                                      new Pair("docs", "documents"),
                                                      new Pair("docu", "documentation"),
                                                      new Pair("docus", "documentations"),
                                                      new Pair("dst", "destination"),
                                                      new Pair("dto", string.Empty),
                                                      new Pair("ed", "edit"),
                                                      new Pair("el", "element"),
                                                      new Pair("ele", "element"),
                                                      new Pair("elem", "element"),
                                                      new Pair("encr", "encrypt"),
                                                      new Pair("env", "environment"),
                                                      new Pair("environ", "environment"),
                                                      new Pair("eq", "equal"),
                                                      new Pair("err", "error"),
                                                      new Pair("exec", "execute"),
                                                      new Pair("ext", "extension"),
                                                      new Pair("fnc", "function"),
                                                      new Pair("frm", "form"),
                                                      new Pair("fwd", "forwarded"),
                                                      new Pair("hdls", "headless"),
                                                      new Pair("hlp", "help"),
                                                      new Pair("horz", "horizontal"),
                                                      new Pair("ident", "identification"),
                                                      new Pair("idents", "identifications"),
                                                      new Pair("idx", "index"),
                                                      new Pair("imp", "implementation"),
                                                      new Pair("impl", "implementation"),
                                                      new Pair("init", "initialize"),
                                                      new Pair("itf", "interface"),
                                                      new Pair("kvp", "pair"),
                                                      new Pair("lang", "language"),
                                                      new Pair("lbl", "label"),
                                                      new Pair("len", "length"),
                                                      new Pair("lib", "library"),
                                                      new Pair("libs", "libraries"),
                                                      new Pair("lv", "listView"),
                                                      new Pair("man", "manager"),
                                                      new Pair("max", "maximum"),
                                                      new Pair("meth", "method"),
                                                      new Pair("mgmt", "management"),
                                                      new Pair("mgr", "manager"),
                                                      new Pair("mgrs", "managers"),
                                                      new Pair("min", "minimum"),
                                                      new Pair("mngr", "manager"),
                                                      new Pair("mngrs", "managers"),
                                                      new Pair("mnu", "menuItem"),
                                                      new Pair("msg", "message"),
                                                      new Pair("msgs", "messages"),
                                                      new Pair("nav", "navigation"),
                                                      new Pair("num", "number"),
                                                      new Pair("nums", "numbers"),
                                                      new Pair("obj", "object"),
                                                      new Pair("op", "operation"),
                                                      new Pair("ops", "operations"),
                                                      new Pair("opt", "option"),
                                                      new Pair("opts", "options"),
                                                      new Pair("para", "parameter"),
                                                      new Pair("param", "parameter"),
                                                      new Pair("params", "parameters"),
                                                      new Pair("perc", "percentage"),
                                                      new Pair("perf", "performance"),
                                                      new Pair("phys", "physical"),
                                                      new Pair("plausi", "plausibility"),
                                                      new Pair("pos", "position"),
                                                      new Pair("pow", "power"),
                                                      new Pair("prev", "previous"),
                                                      new Pair("proc", "process"),
                                                      new Pair("procs", "processes"),
                                                      new Pair("prop", "property"),
                                                      new Pair("props", "properties"),
                                                      new Pair("pt", "point"),
                                                      new Pair("pts", "points"),
                                                      new Pair("pw", "password"),
                                                      new Pair("pwd", "password"),
                                                      new Pair("pswd", "password"),
                                                      new Pair("passwd", "password"),
                                                      new Pair("qty", "quantity"),
                                                      new Pair("rec", "record"),
                                                      new Pair("rect", "rectangle"),
                                                      new Pair("ref", "reference"),
                                                      new Pair("refs", "references"),
                                                      new Pair("rel", "relative"),
                                                      new Pair("reloc", "relocation"),
                                                      new Pair("repo", "repository"),
                                                      new Pair("repos", "repositories"),
                                                      new Pair("req", "request"),
                                                      new Pair("res", "result"),
                                                      new Pair("resp", "response"),
                                                      new Pair("rest", "restore"),
                                                      new Pair("rgn", "region"),
                                                      new Pair("sem", "semantic"),
                                                      new Pair("seq", "sequential"),
                                                      new Pair("sess", "session"),
                                                      new Pair("spec", "specification"),
                                                      new Pair("specs", "specifications"),
                                                      new Pair("src", "source"),
                                                      new Pair("srcs", "sources"),
                                                      new Pair("srv", "service"),
                                                      new Pair("std", "standard"),
                                                      new Pair("str", "string"),
                                                      new Pair("svc", "service"),
                                                      new Pair("svr", "server"),
                                                      new Pair("syn", "syntax"),
                                                      new Pair("sync", "synchronization"),
                                                      new Pair("synchron", "synchronous"),
                                                      new Pair("sys", "system"),
                                                      new Pair("tgt", "target"),
                                                      new Pair("tgts", "targets"),
                                                      new Pair("tm", "time"),
                                                      new Pair("tmp", "temp"),
                                                      new Pair("txt", "text"),
                                                      new Pair("txts", "texts"),
                                                      new Pair("util", "utility"),
                                                      new Pair("utils", "utilities"),
                                                      new Pair("val", "value"),
                                                      new Pair("var", "variable"),
                                                      new Pair("ver", "version"),
                                                      new Pair("vert", "vertical"),
                                                      new Pair("vol", "volume"),
                                                  };

        private static readonly Pair[] OnlyMidTerms =
                                                      {
                                                          new Pair("Alt", "Alternative"),
                                                          new Pair("App", "Application"),
                                                          new Pair("Apps", "Applications"),
                                                          new Pair("Arg", "Argument"),
                                                          new Pair("Args", "Arguments"),
                                                          new Pair("Arr", "Array"),
                                                          new Pair("Assoc", "Association"),
                                                          new Pair("Assocs", "Associations"),
                                                          new Pair("Asynchron", "Asynchronous"),
                                                          new Pair("Attr", "Attribute"),
                                                          new Pair("Auth", "Authorization"),
                                                          new Pair("Bl", "BusinessLogic"),
                                                          new Pair("BL", "BusinessLogic"),
                                                          new Pair("Btn", "Button"),
                                                          new Pair("Btns", "Buttons"),
                                                          new Pair("Calc", "Calculate"),
                                                          new Pair("Cb", "CheckBox"),
                                                          new Pair("Cert", "Certificate"),
                                                          new Pair("Certs", "Certificates"),
                                                          new Pair("Cfg", "Configuration"),
                                                          new Pair("Chk", "CheckBox"),
                                                          new Pair("Cli", "CommandLineInterface"),
                                                          new Pair("CLI", "CommandLineInterface"),
                                                          new Pair("Cls", "Class"),
                                                          new Pair("Cmb", "ComboBox"),
                                                          new Pair("Cmd", "Command"),
                                                          new Pair("Comp", "Compile"),
                                                          new Pair("Compat", "Compatibility"),
                                                          new Pair("Conf", "Configuration"),
                                                          new Pair("Config", "Configuration"),
                                                          new Pair("Configs", "Configurations"),
                                                          new Pair("Conn", "Connection"),
                                                          new Pair("Conns", "Connections"),
                                                          new Pair("Ctg", "Category"),
                                                          new Pair("Ctl", "Control"),
                                                          new Pair("Ctlg", "Catalog"),
                                                          new Pair("Ctrl", "Control"),
                                                          new Pair("Ctx", "Context"),
                                                          new Pair("Cur", "Current"),
                                                          new Pair("Db", "Database"),
                                                          new Pair("Ddl", "DropDownList"),
                                                          new Pair("Decl", "Declaration"),
                                                          new Pair("Decr", "Decrypt"),
                                                          new Pair("Def", "Definition"),
                                                          new Pair("Defs", "Definitions"),
                                                          new Pair("Dep", "Dependency"),
                                                          new Pair("Deps", "Dependencies"),
                                                          new Pair("Desc", "Description"),
                                                          new Pair("Dest", "Destination"),
                                                          new Pair("Diag", "Diagnostic"),
                                                          new Pair("Diags", "Diagnostics"),
                                                          new Pair("Dic", "Dictionary"),
                                                          new Pair("Dict", "Dictionary"),
                                                          new Pair("Diff", "Difference"),
                                                          new Pair("Diffs", "Differences"),
                                                          new Pair("Dir", "Directory"),
                                                          new Pair("Dirs", "Directories"),
                                                          new Pair("Div", "Division"),
                                                          new Pair("Dlg", "Dialog"),
                                                          new Pair("Dlgt", "Delegate"),
                                                          new Pair("Dm", string.Empty), // 'Dm' means 'Domain Model'
                                                          new Pair("DM", string.Empty), // 'DM' means 'Domain Model'
                                                          new Pair("Doc", "Document"),
                                                          new Pair("Docs", "Documents"),
                                                          new Pair("Docu", "Documentation"),
                                                          new Pair("Docus", "Documentations"),
                                                          new Pair("Dst", "Destination"),
                                                          new Pair("Dto", string.Empty),
                                                          new Pair("DTO", string.Empty),
                                                          new Pair("Ed", "Edit"),
                                                          new Pair("Ef", "EntityFramework"),
                                                          new Pair("EF", "EntityFramework"),
                                                          new Pair("El", "Element"),
                                                          new Pair("Ele", "Element"),
                                                          new Pair("Elem", "Element"),
                                                          new Pair("Encr", "Encrypt"),
                                                          new Pair("Env", "Environment"),
                                                          new Pair("Environ", "Environment"),
                                                          new Pair("Eq", "Equal"),
                                                          new Pair("Err", "Error"),
                                                          new Pair("Exec", "Execute"),
                                                          new Pair("Ext", "Extension"),
                                                          new Pair("Fnc", "Function"),
                                                          new Pair("Frm", "Form"),
                                                          new Pair("Fwd", "Forwarded"),
                                                          new Pair("Hdls", "Headless"),
                                                          new Pair("Hlp", "Help"),
                                                          new Pair("Horz", "Horizontal"),
                                                          new Pair("Ident", "Identification"),
                                                          new Pair("Idents", "Identifications"),
                                                          new Pair("Idx", "Index"),
                                                          new Pair("Imp", "Implementation"),
                                                          new Pair("Impl", "Implementation"),
                                                          new Pair("Init", "Initialize"),
                                                          new Pair("Itf", "Interface"),
                                                          new Pair("Lang", "Language"),
                                                          new Pair("Lbl", "Label"),
                                                          new Pair("Len", "Length"),
                                                          new Pair("Lib", "Library"),
                                                          new Pair("Libs", "Libraries"),
                                                          new Pair("Lv", "ListView"),
                                                          new Pair("Man", "Manager"),
                                                          new Pair("Max", "Maximum"),
                                                          new Pair("Meth", "Method"),
                                                          new Pair("Mgmt", "Management"),
                                                          new Pair("Mgr", "Manager"),
                                                          new Pair("Mgrs", "Managers"),
                                                          new Pair("Min", "Minimum"),
                                                          new Pair("Mngr", "Manager"),
                                                          new Pair("Mngrs", "Managers"),
                                                          new Pair("Mnu", "MenuItem"),
                                                          new Pair("Msg", "Message"),
                                                          new Pair("Nav", "Navigation"),
                                                          new Pair("Ns", "Namespace"),
                                                          new Pair("Num", "Number"),
                                                          new Pair("Obj", "Object"),
                                                          new Pair("Objs", "Objects"),
                                                          new Pair("Op", "Operation"),
                                                          new Pair("Ops", "Operations"),
                                                          new Pair("Opt", "Option"),
                                                          new Pair("Opts", "Options"),
                                                          new Pair("Para", "Parameter"),
                                                          new Pair("Param", "Parameter"),
                                                          new Pair("Params", "Parameters"),
                                                          new Pair("Perc", "Percentage"),
                                                          new Pair("Perf", "Performance"),
                                                          new Pair("Phys", "Physical"),
                                                          new Pair("Plausi", "Plausibility"),
                                                          new Pair("Pos", "Position"),
                                                          new Pair("Pow", "Power"),
                                                          new Pair("Prev", "Previous"),
                                                          new Pair("Proc", "Process"),
                                                          new Pair("Procs", "Processes"),
                                                          new Pair("Prop", "Property"),
                                                          new Pair("Props", "Properties"),
                                                          new Pair("Pt", "Point"),
                                                          new Pair("Pts", "Points"),
                                                          new Pair("Pw", "Password"),
                                                          new Pair("Pwd", "Password"),
                                                          new Pair("Pswd", "Password"),
                                                          new Pair("Passwd", "Password"),
                                                          new Pair("Qty", "Quantity"),
                                                          new Pair("Rec", "Record"),
                                                          new Pair("Rect", "Rectangle"),
                                                          new Pair("Ref", "Reference"),
                                                          new Pair("Refs", "References"),
                                                          new Pair("Rel", "Relative"),
                                                          new Pair("Reloc", "Relocation"),
                                                          new Pair("Repo", "Repository"),
                                                          new Pair("Repos", "Repositories"),
                                                          new Pair("Req", "Request"),
                                                          new Pair("Res", "Result"),
                                                          new Pair("Resp", "Response"),
                                                          new Pair("Rest", "Restore"),
                                                          new Pair("Rgn", "Region"),
                                                          new Pair("Sem", "Semantic"),
                                                          new Pair("Seq", "Sequential"),
                                                          new Pair("Sess", "Session"),
                                                          new Pair("Spec", "Specification"),
                                                          new Pair("Src", "Source"),
                                                          new Pair("Srcs", "Sources"),
                                                          new Pair("Srv", "Service"),
                                                          new Pair("Std", "Standard"),
                                                          new Pair("Str", "String"),
                                                          new Pair("Svc", "Service"),
                                                          new Pair("Svr", "Server"),
                                                          new Pair("Syn", "Syntax"),
                                                          new Pair("Sync", "Synchronization"),
                                                          new Pair("Synchron", "Synchronous"),
                                                          new Pair("Sys", "System"),
                                                          new Pair("Tgt", "Target"),
                                                          new Pair("Tgts", "Targets"),
                                                          new Pair("Tm", "Time"),
                                                          new Pair("Tmp", "Temp"),
                                                          new Pair("Txt", "Text"),
                                                          new Pair("Util", "Utility"),
                                                          new Pair("Utils", "Utilities"),
                                                          new Pair("Val", "Value"),
                                                          new Pair("Var", "Variable"),
                                                          new Pair("Ver", "Version"),
                                                          new Pair("Vert", "Vertical"),
                                                          new Pair("Vm", "ViewModel"),
                                                          new Pair("VM", "ViewModel"),
                                                          new Pair("Vms", "ViewModels"),
                                                          new Pair("VMs", "ViewModels"),
                                                          new Pair("Vol", "Volume"),
                                                      };

        private static readonly Pair[] MidTerms = OnlyMidTerms.Concat(Prefixes).ToArray();

        private static readonly Pair[] OnlyPostFixes =
                                                       {
                                                           new Pair("Seq", "Sequence"),
                                                       };

        private static readonly Pair[] Postfixes = OnlyMidTerms.Except(OnlyPostFixes, IdenticalKeyComparer.Instance).Concat(OnlyPostFixes).OrderBy(_ => _.Key).ToArray();

        private static readonly string[] AllowedPostFixTerms =
                                                               {
                                                                   "cept", // accept
                                                                   "cepts", // accepts
                                                                   "crypt", // decrypt/encrypt
                                                                   "dopt", // adopt
                                                                   "dopts", // adopts
                                                                   "enum",
                                                                   "ires",
                                                                   "idst", // midst
                                                                   "ixtures",
                                                                   "kept",
                                                                   "Kept",
                                                                   "mpt", // prompt
                                                                   "mpts", // attempts
                                                                   "cript", // script
                                                                   "cripts", // scripts
                                                                   "rupt",
                                                                   "rupts",
                                                                   "ures",
                                                                   "wares",
                                                               };

        private static readonly string[] AllowedParts =
                                                        {
                                                            // TODO RKN: Remove me "Async",
                                                            "Enumerable",
                                                            "Enumeration",
                                                            "Enum", // must be after the others so that those get properly replaced
                                                            nameof(EventArgs),
                                                            "ever",
                                                            "Ever",
                                                            "Identifiable",
                                                            "Identification",
                                                            "Identifier",
                                                            "Identity",
                                                            "Identities",
                                                            "next",
                                                            "Next",
                                                            "oAuth",
                                                            "OAuth",
                                                            "Over", // 'ver'
                                                            "salt",
                                                            "Salt",
                                                            "text",
                                                            "Text",
                                                            "topLevel",
                                                            "toplevel",
                                                            "topMost",
                                                            "topmost",
                                                            "TopLevel",
                                                            "Toplevel",
                                                            "TopMost",
                                                            "Topmost",
                                                            "MEF",

                                                            // languages
                                                            "lvLV",
                                                            "LvLV",
                                                            "ptBR",
                                                            "PtBR",
                                                            "ptPT",
                                                            "PtPT",
                                                            "ABLE", // BL
                                                            "IBLE", // BL
                                                            "BLUE", // BL
                                                            "CLIC", // CLI
                                                            "LEFT", // EF
                                                            "DOUBLE", // BL
                                                            "REFRESH", // EF
                                                            "REFER", // EF
                                                            "REFACTOR", // EF
                                                            "REFRIGERATOR", // EF
                                                            "BLOCK", // BL
                                                            "DEFAULT", // EF
                                                            "USEFUL", // EF
                                                        };

        private static readonly string[] AllowedNames =
                                                        {
                                                            "args",
                                                            "obj",
                                                            "next",
                                                            "cref",
                                                            "href",
                                                            nameof(EventArgs),
                                                        };

        private static readonly ConcurrentDictionary<string, Pair[]> AlreadyFoundAbbreviationsCache = new ConcurrentDictionary<string, Pair[]>(StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> AlreadyReplacedAbbreviationsCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private static readonly Pair[] Cleanups =
                                                  {
                                                      new Pair("agerag", "ag"), // 'man' within 'manager' / 'manage' / 'managing'
                                                      new Pair("alculateulat", "alculat"), // 'calc' within 'calculate' / 'calculation'
                                                      new Pair("alueue", "alue"), // 'val' within 'value'
                                                      new Pair("arametereter", "arameter"), // 'param' within 'parameter'
                                                      new Pair("arametermeter", "arameter"), // 'para' within 'parameter'
                                                      new Pair("ariableiable", "ariable"), // 'var' within 'variable'
                                                      new Pair("asynchronization", "async"), // 'sync' within 'asynchronization'
                                                      new Pair("ationate", "ate"), // 'reloc' within 'relocate'
                                                      new Pair("ationati", "ati"), // 'reloc' within 'relocation' / 'relocating'
                                                      new Pair("aximumi", "axi"), // 'max' within 'maximum'
                                                      new Pair("aximumimum", "aximum"), // 'max' within 'maximum'
                                                      new Pair("dentificationi", "denti"), // 'ident' within 'identification' / 'identifier' / 'identify' / identity'
                                                      new Pair("dopoint", "dopt"), // 'pt' within 'adopt'
                                                      new Pair("eclarationar", "eclar"), // 'decl' within 'declaration' / 'declare' / 'declaring'
                                                      new Pair("ecordord", "ecord"), // 'rec' within 'record'
                                                      new Pair("ecordtangl", "ectangl"), // 'rec' within 'rectangle'
                                                      new Pair("ecryptement", "ecrement"), // 'decr' within 'decrement'
                                                      new Pair("ecryptypt", "ecrypt"), // 'decr' within 'decrypt'
                                                      new Pair("ectangleangl", "ectangl"), // 'rect' within 'rectangle'
                                                      new Pair("ectect", "ect"), // 'obj' within 'object'
                                                      new Pair("eferencea", "efa"), // 'ref' within 'refactor'
                                                      new Pair("eferencee", "efe"), // 'ref' within 'refers' / 'reference'
                                                      new Pair("eferencer", "efr"), // 'ref' within 'refresh'
                                                      new Pair("efinitionin", "efin"), // 'def' within 'define' / 'definition'
                                                      new Pair("elativeat", "elat"), // 'rel' within 'relate' / 'relating'
                                                      new Pair("elativeativ", "elativ"), // 'rel' within 'relative'
                                                      new Pair("emanticantic", "emantic"), // 'sem' within 'semantic'
                                                      new Pair("ependentend", "epend"), // 'dep' within 'dependent' / 'dependency'
                                                      new Pair("epositoriesitor", "epositor"), // 'repos' within 'repositories'
                                                      new Pair("epositorysitor", "epositor"), // 'repo' within 'repository'
                                                      new Pair("equaluenc", "equenc"), // 'eq' within 'sequence'
                                                      new Pair("equaluir", "equir"), // 'eq' within 'require'
                                                      new Pair("equentialuen", "equen"), // 'seq' within 'sequence' / 'sequential'
                                                      new Pair("erationeration", "eration"), // 'op' within 'operation'
                                                      new Pair("ercentageent", "ercent"), // 'perc' within 'percent' / 'percentae'
                                                      new Pair("erformanceorm", "erform"), // 'perf' within 'perform' / 'performance'
                                                      new Pair("ersionsion", "ersion"), // 'ver' within 'version'
                                                      new Pair("erticalical", "ertical"), // 'vert' within 'vertical'
                                                      new Pair("ertificateificate", "ertificate"), // 'cert' within 'certificate'
                                                      new Pair("escriptionription", "escription"), // 'desc' within 'description'
                                                      new Pair("esponseon", "espon"), // 'resp' within 'response' / 'responding'
                                                      new Pair("estinationination", "estination"), // 'dest' within 'destination'
                                                      new Pair("estoreor", "estor"), // 'rest' within 'restore' / 'restoring' / 'restoration'
                                                      new Pair("esultpon", "espon"), // 'res' within 'response' / 'responding' / 'respond' / 'responsible'
                                                      new Pair("esulttor", "estor"), // 'res' within 'restore' / 'restoring' / 'restoration'
                                                      new Pair("gthgth", "gth"), // 'len' within 'length'
                                                      new Pair("iagnosticnos", "iagnos"), // 'diag' within 'diagnosis' / 'diagnostics'
                                                      new Pair("iagnosticram", "iagram"), // 'diag' within 'diagram'
                                                      new Pair("ibraryrar", "ibrar"), // 'lib' within 'library' / 'libraries'
                                                      new Pair("icalical", "ical"), // 'phys' within 'physical'
                                                      new Pair("ictionaryionar", "ictionar"), // 'dict' within 'dictionary' / 'dictionaries'
                                                      new Pair("ictionarytionar", "ictionar"), // 'dic' within 'dictionary' / 'dictionaries'
                                                      new Pair("itedit", "ited"), // 'ed' within 'edited'
                                                      new Pair("ifferencee", "iffe"), // 'diff' within 'differ' / 'difference' / 'differences' / 'differencing'
                                                      new Pair("ifferencei", "iffi"), // 'diff' within 'diffing'
                                                      new Pair("igationigation", "igation"), // 'nav' within 'navigation'
                                                      new Pair("inimumi", "ini"), // 'min' within 'minimum'
                                                      new Pair("inimumimum", "inimum"), // 'min' within 'minimum'
                                                      new Pair("ionion", "ion"), // 'sess' within 'session'
                                                      new Pair("irecordt", "irect"), // 'rec' within 'direct' / 'directory' / 'directories'
                                                      new Pair("irectangle", "irect"), // 'rect' within 'direct' / 'directory' / 'directories'
                                                      new Pair("irectoryector", "irector"), // 'dir' within 'directory' / 'directories'
                                                      new Pair("iresult", "ires"), // 'res' within 'fires' / 'hires'
                                                      new Pair("itionition", "ition"), // 'pos' within 'position'
                                                      new Pair("itit", "it"), // 'ed' within 'edit'
                                                      new Pair("ivisionid", "ivid"), // 'div' within 'divide' / 'dividing'
                                                      new Pair("ivisionision", "ivision"), // 'div' within 'division'
                                                      new Pair("lausibilitybilit", "lausibilit"), // 'plausi' within 'plausibility' / 'plausibilities'
                                                      new Pair("lternativeernative", "lternative"), // 'alt' within 'alternative'
                                                      new Pair("mentationment", "ment"), // 'docu' within 'document'
                                                      new Pair("mentationmentation", "mentation"), // 'doc' within 'documentation'
                                                      new Pair("mentct", "ct"), // 'ele' within 'select'
                                                      new Pair("mentect", "ct"), // 'el' within 'select'
                                                      new Pair("mentement", "ment"), // 'el' within 'element'
                                                      new Pair("mentent", "ment"), // 'elem' within 'element'
                                                      new Pair("mentment", "ment"), // 'ele' within 'element'
                                                      new Pair("mplementationement", "mplement"), // 'impl' within 'implementation'
                                                      new Pair("mplementationlement", "mplement"), // 'imp' within 'implement'
                                                      new Pair("mplementationr", "mpr"), // 'imp' within 'impress'
                                                      new Pair("ncryptypt", "ncrypt"), // 'encr' within 'encrypt'
                                                      new Pair("nitializeialize", "nitialize"), // 'init' within 'initialize'
                                                      new Pair("ntaxtax", "ntax"), // 'syn' within 'syntax'
                                                      new Pair("nvironmentironment", "nvironment"), // 'env' within 'environment'
                                                      new Pair("nvironmentment", "nvironment"), // 'environ' within 'environment'
                                                      new Pair("olumeum", "olum"), // 'vol' within 'volume'
                                                      new Pair("ompatibleibilit", "ompatibilit"), // 'comp' within 'compatibility' / 'compatibilities'
                                                      new Pair("ompatibleible", "ompatible"), // 'comp' within 'compatible'
                                                      new Pair("ompileatible", "ompatible"), // 'comp' within 'compatible'
                                                      new Pair("ompileile", "ompile"), // 'comp' within 'compile'
                                                      new Pair("onfigurationigur", "onfigur"), // 'conf' within 'configuration' / 'configure'
                                                      new Pair("onfigurationur", "onfigur"), // 'config' within 'configuration' / 'configure'
                                                      new Pair("onnectionect", "onnect"), // 'conn' within 'connection' / 'connect'
                                                      new Pair("ousous", "ous"), // 'sync' within 'asynchronous'
                                                      new Pair("oversion", "over"), // 'ver' within 'hover'
                                                      new Pair("Oversion", "Over"), // 'ver' within 'Over'
                                                      new Pair("pecificationif", "pecif"), // 'spec' within 'specific' / 'specification' / 'specific'
                                                      new Pair("pplicationlication", "pplication"), // 'app' within 'application'
                                                      new Pair("umentument", "ument"), // 'doc' within 'document'
                                                      new Pair("qualual", "qual"), // 'eq' within 'equal'
                                                      new Pair("questuest", "quest"), // 'req' within 'request'
                                                      new Pair("reviousious", "revious"), // 'prev' within 'previous'
                                                      new Pair("rgumentument", "rgument"), // 'arg' within 'argument' / 'arguments'
                                                      new Pair("rocessedur", "rocedur"), // 'proc' within 'procedure'
                                                      new Pair("rocessess", "rocess"), // 'proc' within 'process' / 'processes'
                                                      new Pair("ropertyert", "ropert"), // 'prop' within 'property' / 'properties'
                                                      new Pair("rrayay", "rray"), // 'arr' within 'array'
                                                      new Pair("rroror", "rror"), // 'err' within 'error'
                                                      new Pair("specificationt", "spect"), // 'spec' within 'aspect'
                                                      new Pair("ssemanticb", "ssemb"), // 'sem' within 'assembly'
                                                      new Pair("ssociationiation", "ssociation"), // 'assoc' within 'association'
                                                      new Pair("stemtem", "stem"), // 'sys' within 'system'
                                                      new Pair("synchronizationhronous", "synchronous"), // 'sync' within 'asynchronous'
                                                      new Pair("thodod", "thod"), // 'meth' within 'method'
                                                      new Pair("tilityit", "tilit"), // 'util' within 'utility' / 'utilities'
                                                      new Pair("ttributeibute", "ttribute"), // 'attr' within 'attribute'
                                                      new Pair("uageuage", "uage"), // 'lang' within 'language'
                                                      new Pair("ultult", "ult"), // 'res' within 'result'
                                                      new Pair("umberber", "umber"), // 'num' within 'number'
                                                      new Pair("urrentrency", "urrency"), // 'cur' within 'currency' / 'concurrency'
                                                      new Pair("urrentrent", "urrent"), // 'cur' within 'current'
                                                      new Pair("uthorizationent", "uthent"), // 'auth' within 'authenticate' / 'authentication'
                                                      new Pair("uthorizationori", "uthori"), // 'auth' within 'authorization' / 'authorize'
                                                      new Pair("xecuteu", "xecu"), // 'exec' within 'execute' / 'executing' / executable' / 'execution'
                                                      new Pair("xtensionen", "xten"), // 'ext' within 'extension' / 'extensions'
                                                      new Pair("xtensioner", "xter"), // 'ext' within 'exterior' / 'extern'
                                                      new Pair("ynchronizationhroniz", "ynchroniz"), // 'sync' within 'synchronize'
                                                      new Pair("yntaxc", "ync"), // 'syn' within 'sync' / 'async'
                                                  };

        /// <summary>
        /// Finds all abbreviations contained in the specified text.
        /// </summary>
        /// <param name="value">
        /// The text to inspect for abbreviations.
        /// </param>
        /// <returns>
        /// A span containing all found abbreviations as pairs of abbreviated and full terms, or an empty span if no abbreviations are found or the text is <see langword="null"/>.
        /// </returns>
        internal static ReadOnlySpan<Pair> Find(string value)
        {
            if (value is null)
            {
                return ReadOnlySpan<Pair>.Empty;
            }

            if (value.EqualsAny(AllowedNames, StringComparison.OrdinalIgnoreCase))
            {
                return ReadOnlySpan<Pair>.Empty;
            }

            // cache findings as they will not change
            return AlreadyFoundAbbreviationsCache.GetOrAdd(value, FindCore);
        }

        /// <summary>
        /// Replaces all abbreviations in the specified text with their full terms.
        /// </summary>
        /// <param name="value">
        /// The text in which abbreviations shall be replaced.
        /// </param>
        /// <param name="findings">
        /// The abbreviations to replace.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with all abbreviations replaced by their full terms.
        /// </returns>
        internal static string ReplaceAllAbbreviations(string value, in ReadOnlySpan<Pair> findings)
        {
            if (findings.Length is 0)
            {
                return value;
            }

            // let's see if we have cached the result and use that (as the findings should not differ here anymore)
            // note that we do not need a synchronization here between 'TryGetValue' and 'AddOrUpdate' as the replaced value should be the same (even when the replacement is done a second time)
            if (AlreadyReplacedAbbreviationsCache.TryGetValue(value, out var replacedValue))
            {
                return replacedValue;
            }

            var builder = value.AsCachedBuilder();

            ReplaceAllAbbreviations(builder, findings);

            return AlreadyReplacedAbbreviationsCache.AddOrUpdate(value, builder.ToStringAndRelease(), (key, presentValue) => presentValue);
        }

        /// <summary>
        /// Replaces all abbreviations in the specified text with their full terms.
        /// </summary>
        /// <param name="value">
        /// The text in which abbreviations shall be replaced.
        /// </param>
        /// <param name="findings">
        /// The abbreviations to replace.
        /// </param>
        /// <returns>
        /// The text builder with all abbreviations replaced by their full terms.
        /// </returns>
        internal static StringBuilder ReplaceAllAbbreviations(StringBuilder value, in ReadOnlySpan<Pair> findings)
        {
            if (findings.Length > 0)
            {
                value.ReplaceAllWithProbe(findings);
                value.ReplaceAllWithProbe(Cleanups);
            }

            return value;
        }

        /// <summary>
        /// Finds and replaces all abbreviations in the specified text with their full terms.
        /// </summary>
        /// <param name="value">
        /// The text to inspect for abbreviations and in which abbreviations shall be replaced.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with all abbreviations replaced by their full terms.
        /// </returns>
        internal static string FindAndReplaceAllAbbreviations(string value)
        {
            return AlreadyReplacedAbbreviationsCache.GetOrAdd(value, _ => ReplaceAllAbbreviations(_, Find(_)));
        }

        /// <summary>
        /// Finds and replaces all abbreviations in the specified text with their full terms.
        /// </summary>
        /// <param name="value">
        /// The text to inspect for abbreviations and in which abbreviations shall be replaced.
        /// </param>
        /// <returns>
        /// The text builder with all abbreviations replaced by their full terms.
        /// </returns>
        internal static StringBuilder FindAndReplaceAllAbbreviations(StringBuilder value)
        {
            var findings = Find(value.ToString());

            return ReplaceAllAbbreviations(value, findings);
        }

//// ncrunch: rdi off

        /// <summary>
        /// Finds all abbreviations contained in the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to inspect for abbreviations.
        /// </param>
        /// <returns>
        /// An array of all found abbreviations as pairs of abbreviated and full terms, or an empty array if no abbreviations are found.
        /// </returns>
        private static Pair[] FindCore(string text)
        {
            var prepared = Prepare(text);

            return FindCore(prepared.AsSpan());

            string Prepare(string s)
            {
                const string Async = "Async";

                var sb = s.AsCachedBuilder();

                if (s.Length > Async.Length)
                {
                    var index = s.IndexOf(Async, StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                    {
                        var afterIndex = index + Async.Length;

                        if (afterIndex < s.Length && s[afterIndex].IsUpperCase())
                        {
                            sb.Remove(index, Async.Length);
                        }
                    }
                }

                sb.Without(AllowedParts)
                  .Trimmed();

                return sb.ToStringAndRelease();
            }
        }

        /// <summary>
        /// Finds all abbreviations contained in the specified text span.
        /// </summary>
        /// <param name="textSpan">
        /// The text span to inspect for abbreviations.
        /// </param>
        /// <returns>
        /// An array of all found abbreviations as pairs of abbreviated and full terms, or an empty array if no abbreviations are found.
        /// </returns>
        private static Pair[] FindCore(in ReadOnlySpan<char> textSpan)
        {
            var results = new HashSet<Pair>(KeyComparer.Instance);

            for (int index = 0, prefixesLength = Prefixes.Length; index < prefixesLength; index++)
            {
                var pair = Prefixes[index];

                var keySpan = pair.Key.AsSpan();

                if (PrefixHasIssue(keySpan, textSpan))
                {
                    results.Add(pair);
                }

                if (CompleteTermHasIssue(keySpan, textSpan))
                {
                    results.Add(pair);
                }
            }

            //// TODO RKN: replace prefixes to not find them again as middle terms or whatever?

            for (int index = 0, postfixesLength = Postfixes.Length; index < postfixesLength; index++)
            {
                var pair = Postfixes[index];

                if (PostFixHasIssue(pair.Key.AsSpan(), textSpan))
                {
                    results.Add(pair);
                }
            }

            for (int index = 0, midTermsLength = MidTerms.Length; index < midTermsLength; index++)
            {
                var pair = MidTerms[index];

                if (MidTermHasIssue(pair.Key.AsSpan(), textSpan))
                {
                    results.Add(pair);
                }
            }

            return results.Count is 0 ? Array.Empty<Pair>() : results.ToArray();
        }
//// ncrunch: rdi default

        /// <summary>
        /// Determines whether the specified character indicates the start of a new word.
        /// </summary>
        /// <param name="c">
        /// The character to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character is uppercase or an underscore; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IndicatesNewWord(in char c) => c.IsUpperCase() || c is Constants.Underscore;

        /// <summary>
        /// Determines whether the specified key represents the complete text.
        /// </summary>
        /// <param name="key">
        /// The abbreviation key to check.
        /// </param>
        /// <param name="value">
        /// The text to compare against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the key exactly matches the text; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CompleteTermHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value) => key.SequenceEqual(value);

        /// <summary>
        /// Determines whether the specified key represents an abbreviation at the beginning of the text.
        /// </summary>
        /// <param name="key">
        /// The abbreviation key to check.
        /// </param>
        /// <param name="value">
        /// The text to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text starts with the key followed by a character indicating a new word; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PrefixHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
        {
            var keyLength = key.Length;

            if (value.Length > keyLength)
            {
                if (IndicatesNewWord(value[keyLength]) && value.StartsWith(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified key represents an abbreviation at the end of the text.
        /// </summary>
        /// <param name="key">
        /// The abbreviation key to check.
        /// </param>
        /// <param name="value">
        /// The text to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text ends with the key and is not part of an allowed postfix term; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PostFixHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value) => value.EndsWith(key, StringComparison.Ordinal) && value.EndsWithAny(AllowedPostFixTerms) is false;

        /// <summary>
        /// Determines whether the specified key represents an abbreviation in the middle of the text.
        /// </summary>
        /// <param name="key">
        /// The abbreviation key to check.
        /// </param>
        /// <param name="value">
        /// The text to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the text contains the key as a standalone term surrounded by word boundaries; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool MidTermHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
        {
            // do a quick check on the hot path, as most times (~99.8%) there is no issue
            if (value.Contains(key, StringComparison.Ordinal) is false)
            {
                return false;
            }

            var keyLength = key.Length;
            var valueLength = value.Length;
            var keyStartsUpperCase = key[0].IsUpperCase();

            var index = 0;

            do
            {
                var newIndex = value.Slice(index).IndexOf(key); // performs ordinal comparison

                if (newIndex <= -1)
                {
                    return false;
                }

                index += newIndex;

                var positionAfterCharacter = index + keyLength;

                if (positionAfterCharacter < valueLength)
                {
                    if (IndicatesNewWord(value[positionAfterCharacter]))
                    {
                        if (keyStartsUpperCase)
                        {
                            return true;
                        }

                        if (index > 0)
                        {
                            var positionBeforeText = index - 1;

                            if (IndicatesNewWord(value[positionBeforeText]))
                            {
                                return true;
                            }
                        }
                    }
                }

                index = positionAfterCharacter;
            }
            while (true);
        }

        /// <summary>
        /// Provides equality comparison for <see cref="Pair"/> instances where keys are considered equal if they match exactly (when same length) or if one key contains the other as a substring (when different lengths).
        /// </summary>
        /// <remarks>
        /// This comparer allows grouping abbreviations that overlap, such as "app" and "application".
        /// <para>
        /// <note type="information">
        /// <see cref="GetHashCode"/> returns a constant value to force hash collisions, ensuring <see cref="Equals"/> is invoked for all comparisons. This trades hash table performance for the ability to use substring-based equality logic.
        /// </note>
        /// </para>
        /// </remarks>
        private sealed class KeyComparer : IEqualityComparer<Pair>
        {
            /// <summary>
            /// The only instance.
            /// </summary>
            internal static readonly KeyComparer Instance = new KeyComparer();

            /// <summary>
            /// Prevents a default instance of the <see cref="KeyComparer"/> class from being created.
            /// </summary>
            private KeyComparer()
            {
            }

            /// <inheritdoc />
            public bool Equals(Pair x, Pair y)
            {
                var spanX = x.Key.AsSpan();
                var spanY = y.Key.AsSpan();

                if (spanX.Length == spanY.Length)
                {
                    return spanX.SequenceEqual(spanY);
                }

                if (spanX.Length > spanY.Length)
                {
                    return spanX.Contains(spanY);
                }

                return spanY.Contains(spanX);
            }

            /// <inheritdoc />
            public int GetHashCode(Pair obj) => 42; // we have to rely on 'Equals', so we have to provide the same hash to cause 'Equals' to be invoked
        }

        /// <summary>
        /// Provides strict equality comparison for <see cref="Pair"/> instances where keys must be character-by-character identical. Unlike <see cref="KeyComparer"/>, this comparer does not consider substring matches as equal.
        /// </summary>
        /// <remarks>
        /// This comparer allows to filter exact duplicates from abbreviation collections.
        /// <para>
        /// <note type="information">
        /// <see cref="GetHashCode"/> returns a constant value to force hash collisions, ensuring <see cref="Equals"/> is invoked for all comparisons.
        /// </note>
        /// </para>
        /// </remarks>
        private sealed class IdenticalKeyComparer : IEqualityComparer<Pair>
        {
            /// <summary>
            /// The only instance.
            /// </summary>
            internal static readonly IdenticalKeyComparer Instance = new IdenticalKeyComparer();

            /// <summary>
            /// Prevents a default instance of the <see cref="IdenticalKeyComparer"/> class from being created.
            /// </summary>
            private IdenticalKeyComparer()
            {
            }

            /// <inheritdoc />
            public bool Equals(Pair x, Pair y)
            {
                var spanX = x.Key.AsSpan();
                var spanY = y.Key.AsSpan();

                return spanX.Length == spanY.Length && spanX.SequenceEqual(spanY);
            }

            /// <inheritdoc />
            public int GetHashCode(Pair obj) => 42; // we have to rely on 'Equals', so we have to provide the same hash to cause 'Equals' to be invoked
        }
    }
}