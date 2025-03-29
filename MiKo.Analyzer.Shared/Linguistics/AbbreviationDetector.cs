using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class AbbreviationDetector
    {
        private static readonly Pair[] Prefixes =
                                                  {
                                                      new Pair("alt", "alternative"),
                                                      new Pair("app", "application"),
                                                      new Pair("apps", "applications"),
                                                      new Pair("assoc", "association"),
                                                      new Pair("auth", "authorization"),
                                                      new Pair("btn", "button"),
                                                      new Pair("calc", "calculate"),
                                                      new Pair("cb", "checkBox"),
                                                      new Pair("cfg", "configuration"),
                                                      new Pair("cert", "certificate"),
                                                      new Pair("chk", "checkBox"),
                                                      new Pair("cls", "class"),
                                                      new Pair("cmb", "comboBox"),
                                                      new Pair("cmd", "command"),
                                                      new Pair("conf", "configuration"),
                                                      new Pair("config", "configuration"),
                                                      new Pair("configs", "configurations"),
                                                      new Pair("conn", "connection"),
                                                      new Pair("ctg", "category"),
                                                      new Pair("ctl", "control"),
                                                      new Pair("ctlg", "catalog"),
                                                      new Pair("ctrl", "control"),
                                                      new Pair("ctx", "context"),
                                                      new Pair("db", "database"),
                                                      new Pair("ddl", "dropDownList"),
                                                      new Pair("decl", "declaration"),
                                                      new Pair("decr", "decrypt"),
                                                      new Pair("def", "definition"),
                                                      new Pair("defs", "definitions"),
                                                      new Pair("desc", "description"),
                                                      new Pair("dest", "destination"),
                                                      new Pair("diag", "diagnostic"),
                                                      new Pair("diags", "diagnostics"),
                                                      new Pair("dict", "dictionary"),
                                                      new Pair("diff", "difference"),
                                                      new Pair("diffs", "differences"),
                                                      new Pair("dir", "directory"),
                                                      new Pair("dirs", "directories"),
                                                      new Pair("dlg", "dialog"),
                                                      new Pair("doc", "document"),
                                                      new Pair("docs", "documents"),
                                                      new Pair("docs", "documents"),
                                                      new Pair("dst", "destination"),
                                                      new Pair("dto", string.Empty),
                                                      new Pair("env", "environment"),
                                                      new Pair("encr", "encrypt"),
                                                      new Pair("environ", "environment"),
                                                      new Pair("err", "error"),
                                                      new Pair("exec", "execute"),
                                                      new Pair("ext", "extension"),
                                                      new Pair("frm", "form"),
                                                      new Pair("hdls", "headless"),
                                                      new Pair("ident", "identification"),
                                                      new Pair("idents", "identifications"),
                                                      new Pair("idx", "index"),
                                                      new Pair("init", "initialize"),
                                                      new Pair("itf", "interface"),
                                                      new Pair("lang", "language"),
                                                      new Pair("lbl", "label"),
                                                      new Pair("lib", "library"),
                                                      new Pair("libs", "libraries"),
                                                      new Pair("lv", "listView"),
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
                                                      new Pair("num", "number"),
                                                      new Pair("nums", "numbers"),
                                                      new Pair("obj", "object"),
                                                      new Pair("para", "parameter"),
                                                      new Pair("param", "parameter"),
                                                      new Pair("params", "parameters"),
                                                      new Pair("perc", "percentage"),
                                                      new Pair("perf", "performance"),
                                                      new Pair("phys", "physical"),
                                                      new Pair("plausi", "plausibility"),
                                                      new Pair("pos", "position"),
                                                      new Pair("pow", "power"),
                                                      new Pair("proc", "process"),
                                                      new Pair("procs", "processes"),
                                                      new Pair("prop", "property"),
                                                      new Pair("props", "properties"),
                                                      new Pair("pt", "point"),
                                                      new Pair("pts", "points"),
                                                      new Pair("qty", "quantity"),
                                                      new Pair("ref", "reference"),
                                                      new Pair("refs", "references"),
                                                      new Pair("repo", "repository"),
                                                      new Pair("repos", "repositories"),
                                                      new Pair("req", "request"),
                                                      new Pair("res", "result"),
                                                      new Pair("resp", "response"),
                                                      new Pair("sem", "semantic"),
                                                      new Pair("spec", "specification"),
                                                      new Pair("specs", "specifications"),
                                                      new Pair("src", "source"),
                                                      new Pair("std", "standard"),
                                                      new Pair("str", "string"),
                                                      new Pair("sync", "synchronization"),
                                                      new Pair("svc", "service"),
                                                      new Pair("tm", "time"),
                                                      new Pair("tmp", "temp"),
                                                      new Pair("txt", "text"),
                                                      new Pair("txts", "texts"),
                                                      new Pair("var", "variable"),
                                                      new Pair("ver", "version"),
                                                      new Pair("vol", "volume"),
                                                  };

        private static readonly Pair[] OnlyMidTerms =
                                                      {
                                                          new Pair("Alt", "Alternative"),
                                                          new Pair("App", "Application"),
                                                          new Pair("Apps", "Applications"),
                                                          new Pair("Assoc", "Association"),
                                                          new Pair("Auth", "Authorization"),
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
                                                          new Pair("Db", "Database"),
                                                          new Pair("Ddl", "DropDownList"),
                                                          new Pair("Decl", "Declaration"),
                                                          new Pair("Decr", "Decrypt"),
                                                          new Pair("Def", "Definition"),
                                                          new Pair("Defs", "Definitions"),
                                                          new Pair("Defs", "Definitions"),
                                                          new Pair("Desc", "Description"),
                                                          new Pair("Dest", "Destination"),
                                                          new Pair("Diag", "Diagnostic"),
                                                          new Pair("Diags", "Diagnostics"),
                                                          new Pair("Dict", "Dictionary"),
                                                          new Pair("Diff", "Difference"),
                                                          new Pair("Diffs", "Differences"),
                                                          new Pair("Dir", "Directory"),
                                                          new Pair("Dirs", "Directories"),
                                                          new Pair("Dlg", "Dialog"),
                                                          new Pair("Doc", "Document"),
                                                          new Pair("Docs", "Documents"),
                                                          new Pair("Dst", "Destination"),
                                                          new Pair("Dto", string.Empty),
                                                          new Pair("DTO", string.Empty),
                                                          new Pair("Ef", "EntityFramework"),
                                                          new Pair("EF", "EntityFramework"),
                                                          new Pair("Encr", "Encrypt"),
                                                          new Pair("Env", "Environment"),
                                                          new Pair("Environ", "Environment"),
                                                          new Pair("Err", "Error"),
                                                          new Pair("Exec", "Execute"),
                                                          new Pair("Ext", "Extension"),
                                                          new Pair("Frm", "Form"),
                                                          new Pair("Hdls", "Headless"),
                                                          new Pair("Ident", "Identification"),
                                                          new Pair("Idents", "Identifications"),
                                                          new Pair("Idx", "Index"),
                                                          new Pair("Init", "Initialize"),
                                                          new Pair("Itf", "Interface"),
                                                          new Pair("Lang", "Language"),
                                                          new Pair("Lbl", "Label"),
                                                          new Pair("Lib", "Library"),
                                                          new Pair("Libs", "Libraries"),
                                                          new Pair("Lv", "ListView"),
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
                                                          new Pair("Num", "Number"),
                                                          new Pair("Obj", "Object"),
                                                          new Pair("Objs", "Objects"),
                                                          new Pair("Op", "Operation"),
                                                          new Pair("Ops", "Operations"),
                                                          new Pair("Para", "Parameter"),
                                                          new Pair("Param", "Parameter"),
                                                          new Pair("Params", "Parameters"),
                                                          new Pair("Perc", "Percentage"),
                                                          new Pair("Perf", "Performance"),
                                                          new Pair("Phys", "Physical"),
                                                          new Pair("Plausi", "Plausibility"),
                                                          new Pair("Pos", "Position"),
                                                          new Pair("Pow", "Power"),
                                                          new Pair("Proc", "Process"),
                                                          new Pair("Procs", "Processes"),
                                                          new Pair("Prop", "Property"),
                                                          new Pair("Props", "Properties"),
                                                          new Pair("Pt", "Point"),
                                                          new Pair("Pts", "Points"),
                                                          new Pair("Qty", "Quantity"),
                                                          new Pair("Ref", "Reference"),
                                                          new Pair("Refs", "References"),
                                                          new Pair("Repo", "Repository"),
                                                          new Pair("Repos", "Repositories"),
                                                          new Pair("Req", "Request"),
                                                          new Pair("Res", "Result"),
                                                          new Pair("Resp", "Response"),
                                                          new Pair("Sem", "Semantic"),
                                                          new Pair("Spec", "Specification"),
                                                          new Pair("Src", "Source"),
                                                          new Pair("Std", "Standard"),
                                                          new Pair("Str", "String"),
                                                          new Pair("Sync", "Synchronization"),
                                                          new Pair("Svc", "Service"),
                                                          new Pair("Tm", "Time"),
                                                          new Pair("Tmp", "Temp"),
                                                          new Pair("Txt", "Text"),
                                                          new Pair("Var", "Variable"),
                                                          new Pair("Ver", "Version"),
                                                          new Pair("Vol", "Volume"),
                                                          new Pair("BL", "BusinessLogic"),
                                                          new Pair("Bl", "BusinessLogic"),
                                                          new Pair("VM", "ViewModel"),
                                                          new Pair("VMs", "ViewModels"),
                                                          new Pair("Vm", "ViewModel"),
                                                          new Pair("Vms", "ViewModels"),
                                                      };

        private static readonly Pair[] MidTerms = Prefixes.Concat(OnlyMidTerms).ToArray();

        private static readonly Pair[] Postfixes = OnlyMidTerms;

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
                                                            "Async",
                                                            "Enumerable",
                                                            "Enumeration",
                                                            "Enum", // must be after the others so that those get properly replaced
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
                                                            "obj",
                                                            "next",
                                                            "cref",
                                                            "href",
                                                        };

        internal static ReadOnlySpan<Pair> Find(string value)
        {
            if (value is null)
            {
                return ReadOnlySpan<Pair>.Empty;
            }

            if (value.EqualsAny(AllowedNames))
            {
                return ReadOnlySpan<Pair>.Empty;
            }

            return FindCore(value.Without(AllowedParts).AsSpan());
        }

        internal static string ReplaceAllAbbreviations(string value, ReadOnlySpan<Pair> findings)
        {
            if (findings.Length > 0)
            {
                return ReplaceAllAbbreviations(value.AsCachedBuilder(), findings).ToStringAndRelease();
            }

            return value;
        }

        internal static StringBuilder ReplaceAllAbbreviations(StringBuilder value, ReadOnlySpan<Pair> findings)
        {
            if (findings.Length > 0)
            {
                return value.ReplaceAllWithCheck(findings);
            }

            return value;
        }

        internal static string FindAndReplaceAllAbbreviations(string value)
        {
            var findings = Find(value);

            return ReplaceAllAbbreviations(value, findings);
        }

        internal static StringBuilder FindAndReplaceAllAbbreviations(StringBuilder value)
        {
            var findings = Find(value.ToString());

            return ReplaceAllAbbreviations(value, findings);
        }

        //// ncrunch: rdi off
        private static ReadOnlySpan<Pair> FindCore(ReadOnlySpan<char> valueSpan)
        {
            var result = new HashSet<Pair>(KeyEqualityComparer.Instance);

            var prefixesLength = Prefixes.Length;

            for (var index = 0; index < prefixesLength; index++)
            {
                var pair = Prefixes[index];

                var keySpan = pair.Key.AsSpan();

                if (PrefixHasIssue(keySpan, valueSpan))
                {
                    result.Add(pair);
                }

                if (CompleteTermHasIssue(keySpan, valueSpan))
                {
                    result.Add(pair);
                }
            }

            //// TODO RKN: replace prefixes to not find them again as middle terms or whatever?

            var postfixesLength = Postfixes.Length;

            for (var index = 0; index < postfixesLength; index++)
            {
                var pair = Postfixes[index];

                if (PostFixHasIssue(pair.Key.AsSpan(), valueSpan))
                {
                    result.Add(pair);
                }
            }

            var midTermsLength = MidTerms.Length;

            for (var index = 0; index < midTermsLength; index++)
            {
                var pair = MidTerms[index];

                if (MidTermHasIssue(pair.Key.AsSpan(), valueSpan))
                {
                    result.Add(pair);
                }
            }

            return result.Count > 0
                   ? result.ToArray()
                   : ReadOnlySpan<Pair>.Empty;
        }
//// ncrunch: rdi default

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IndicatesNewWord(ref char c) => c.IsUpperCase() || c == Constants.Underscore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CompleteTermHasIssue(ReadOnlySpan<char> key, ReadOnlySpan<char> value) => key.SequenceEqual(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PrefixHasIssue(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            if (value.Length > key.Length)
            {
                var last = value[key.Length];

                if (IndicatesNewWord(ref last) && value.StartsWith(key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PostFixHasIssue(ReadOnlySpan<char> key, ReadOnlySpan<char> value) => value.EndsWith(key, StringComparison.Ordinal) && value.EndsWithAny(AllowedPostFixTerms, StringComparison.Ordinal) is false;

        private static bool MidTermHasIssue(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
        {
            var index = 0;

            // do not cache 'key.Length' or 'value.Length' or 'keys[0].IsUpperCase' as most times (~99.8%) they are not used
            do
            {
                var newIndex = value.Slice(index).IndexOf(key, StringComparison.Ordinal);

                if (newIndex <= -1)
                {
                    return false;
                }

                index += newIndex;

                var positionAfterCharacter = index + key.Length;

                if (positionAfterCharacter < value.Length)
                {
                    var charAfter = value[positionAfterCharacter];

                    if (IndicatesNewWord(ref charAfter))
                    {
                        if (key[0].IsUpperCase())
                        {
                            return true;
                        }

                        var positionBeforeText = index - 1;

                        if (positionBeforeText >= 0)
                        {
                            var charBefore = value[positionBeforeText];

                            if (IndicatesNewWord(ref charBefore))
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

        private sealed class KeyEqualityComparer : IEqualityComparer<Pair>
        {
            internal static readonly KeyEqualityComparer Instance = new KeyEqualityComparer();

            private KeyEqualityComparer()
            {
            }

            public bool Equals(Pair x, Pair y) => x.Key.AsSpan().SequenceEqual(y.Key.AsSpan());

            public int GetHashCode(Pair obj) => obj.Key.GetHashCode();
        }
    }
}