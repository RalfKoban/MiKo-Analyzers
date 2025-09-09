using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class AbbreviationFinder
    {
        private static readonly Pair[] Prefixes =
                                                  {
                                                      new Pair("alt", "alternative"),
                                                      new Pair("app", "application"),
                                                      new Pair("apps", "applications"),
                                                      new Pair("assoc", "association"),
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
                                                      new Pair("dm", string.Empty), // 'dm' means 'Domain Model'
                                                      new Pair("doc", "document"),
                                                      new Pair("docs", "documents"),
                                                      new Pair("docs", "documents"),
                                                      new Pair("dst", "destination"),
                                                      new Pair("dto", string.Empty),
                                                      new Pair("encr", "encrypt"),
                                                      new Pair("env", "environment"),
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
                                                      new Pair("len", "length"),
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
                                                      new Pair("opt", "option"),
                                                      new Pair("opts", "options"),
                                                      new Pair("op", "operation"),
                                                      new Pair("ops", "operations"),
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
                                                      new Pair("qty", "quantity"),
                                                      new Pair("ref", "reference"),
                                                      new Pair("refs", "references"),
                                                      new Pair("repo", "repository"),
                                                      new Pair("repos", "repositories"),
                                                      new Pair("req", "request"),
                                                      new Pair("res", "result"),
                                                      new Pair("resp", "response"),
                                                      new Pair("sem", "semantic"),
                                                      new Pair("seq", "sequential"),
                                                      new Pair("sess", "session"),
                                                      new Pair("spec", "specification"),
                                                      new Pair("specs", "specifications"),
                                                      new Pair("src", "source"),
                                                      new Pair("std", "standard"),
                                                      new Pair("str", "string"),
                                                      new Pair("svc", "service"),
                                                      new Pair("sync", "synchronization"),
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
                                                          new Pair("Dm", string.Empty), // 'Dm' means 'Domain Model'
                                                          new Pair("DM", string.Empty), // 'DM' means 'Domain Model'
                                                          new Pair("Doc", "Document"),
                                                          new Pair("Docs", "Documents"),
                                                          new Pair("Dst", "Destination"),
                                                          new Pair("Dto", string.Empty),
                                                          new Pair("DTO", string.Empty),
                                                          new Pair("Ef", "EntityFramework"),
                                                          new Pair("EF", "EntityFramework"),
                                                          new Pair("El", "Element"),
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
                                                          new Pair("Len", "Length"),
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
                                                          new Pair("Qty", "Quantity"),
                                                          new Pair("Ref", "Reference"),
                                                          new Pair("Refs", "References"),
                                                          new Pair("Repo", "Repository"),
                                                          new Pair("Repos", "Repositories"),
                                                          new Pair("Req", "Request"),
                                                          new Pair("Res", "Result"),
                                                          new Pair("Resp", "Response"),
                                                          new Pair("Sem", "Semantic"),
                                                          new Pair("Seq", "Sequential"),
                                                          new Pair("Sess", "Session"),
                                                          new Pair("Spec", "Specification"),
                                                          new Pair("Src", "Source"),
                                                          new Pair("Std", "Standard"),
                                                          new Pair("Str", "String"),
                                                          new Pair("Svc", "Service"),
                                                          new Pair("Sync", "Synchronization"),
                                                          new Pair("Tm", "Time"),
                                                          new Pair("Tmp", "Temp"),
                                                          new Pair("Txt", "Text"),
                                                          new Pair("Var", "Variable"),
                                                          new Pair("Ver", "Version"),
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

        private static readonly ConcurrentDictionary<string, string> AlreadyInspectedNames = new ConcurrentDictionary<string, string>();

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

            var name = AlreadyInspectedNames.GetOrAdd(value, _ => _.Without(AllowedParts));

            return FindCore(name.AsSpan());
        }

        internal static string ReplaceAllAbbreviations(string value, in ReadOnlySpan<Pair> findings)
        {
            if (findings.Length > 0)
            {
                return ReplaceAllAbbreviations(value.AsCachedBuilder(), findings).ToStringAndRelease();
            }

            return value;
        }

        internal static StringBuilder ReplaceAllAbbreviations(StringBuilder value, in ReadOnlySpan<Pair> findings)
        {
            if (findings.Length > 0)
            {
                return value.ReplaceAllWithProbe(findings);
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
        private static ReadOnlySpan<Pair> FindCore(in ReadOnlySpan<char> valueSpan)
        {
            var results = new HashSet<Pair>(KeyComparer.Instance);

            for (int index = 0, prefixesLength = Prefixes.Length; index < prefixesLength; index++)
            {
                var pair = Prefixes[index];

                var keySpan = pair.Key.AsSpan();

                if (PrefixHasIssue(keySpan, valueSpan))
                {
                    results.Add(pair);
                }

                if (CompleteTermHasIssue(keySpan, valueSpan))
                {
                    results.Add(pair);
                }
            }

            //// TODO RKN: replace prefixes to not find them again as middle terms or whatever?

            for (int index = 0, postfixesLength = Postfixes.Length; index < postfixesLength; index++)
            {
                var pair = Postfixes[index];

                if (PostFixHasIssue(pair.Key.AsSpan(), valueSpan))
                {
                    results.Add(pair);
                }
            }

            for (int index = 0, midTermsLength = MidTerms.Length; index < midTermsLength; index++)
            {
                var pair = MidTerms[index];

                if (MidTermHasIssue(pair.Key.AsSpan(), valueSpan))
                {
                    results.Add(pair);
                }
            }

            return results.Count is 0 ? ReadOnlySpan<Pair>.Empty : results.ToArray();
        }
//// ncrunch: rdi default

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IndicatesNewWord(in char c) => c.IsUpperCase() || c is Constants.Underscore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CompleteTermHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value) => key.SequenceEqual(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PrefixHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
        {
            var keyLength = key.Length;

            if (value.Length > keyLength)
            {
                if (IndicatesNewWord(value[keyLength]) && value.StartsWith(key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PostFixHasIssue(in ReadOnlySpan<char> key, in ReadOnlySpan<char> value) => value.EndsWith(key, StringComparison.Ordinal) && value.EndsWithAny(AllowedPostFixTerms) is false;

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

        private sealed class KeyComparer : IEqualityComparer<Pair>
        {
            internal static readonly KeyComparer Instance = new KeyComparer();

            private KeyComparer()
            {
            }

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

            public int GetHashCode(Pair obj) => 42; // we have to rely on 'Equals', so we have to provide the same hash to cause 'Equals' to be invoked
        }

        private sealed class IdenticalKeyComparer : IEqualityComparer<Pair>
        {
            internal static readonly IdenticalKeyComparer Instance = new IdenticalKeyComparer();

            private IdenticalKeyComparer()
            {
            }

            public bool Equals(Pair x, Pair y)
            {
                var spanX = x.Key.AsSpan();
                var spanY = y.Key.AsSpan();

                return spanX.Length == spanY.Length && spanX.SequenceEqual(spanY);
            }

            public int GetHashCode(Pair obj) => 42; // we have to rely on 'Equals', so we have to provide the same hash to cause 'Equals' to be invoked
        }
    }
}