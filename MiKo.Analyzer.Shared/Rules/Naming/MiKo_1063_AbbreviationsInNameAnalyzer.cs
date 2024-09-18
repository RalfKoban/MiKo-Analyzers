using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1063_AbbreviationsInNameAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1063";

        private static readonly Pair[] Prefixes =
                                                  {
                                                      new Pair("app", "application"),
                                                      new Pair("apps", "applications"),
                                                      new Pair("assoc", "association"),
                                                      new Pair("auth", "authorization"),
                                                      new Pair("btn", "button"),
                                                      new Pair("cb", "checkBox"),
                                                      new Pair("cert", "certificate"),
                                                      new Pair("chk", "checkBox"),
                                                      new Pair("cls", "class"),
                                                      new Pair("cmb", "comboBox"),
                                                      new Pair("cmd", "command"),
                                                      new Pair("conf", "configuration"),
                                                      new Pair("config", "configuration"),
                                                      new Pair("configs", "configurations"),
                                                      new Pair("conn", "connection"),
                                                      new Pair("ctl", "control"),
                                                      new Pair("ctrl", "control"),
                                                      new Pair("ctx", "context"),
                                                      new Pair("db", "database"),
                                                      new Pair("ddl", "dropDownList"),
                                                      new Pair("decl", "declaration"),
                                                      new Pair("decr", "decrypt"),
                                                      new Pair("desc", "description"),
                                                      new Pair("dest", "destination"),
                                                      new Pair("diag", "diagnostic"),
                                                      new Pair("diags", "diagnostics"),
                                                      new Pair("dict", "dictionary"),
                                                      new Pair("diff", "difference"),
                                                      new Pair("diffs", "differences"),
                                                      new Pair("dir", "directory"),
                                                      new Pair("dlg", "dialog"),
                                                      new Pair("doc", "document"),
                                                      new Pair("dst", "destination"),
                                                      new Pair("dto", string.Empty),
                                                      new Pair("env", "environment"),
                                                      new Pair("encr", "encrypt"),
                                                      new Pair("environ", "environment"),
                                                      new Pair("err", "error"),
                                                      new Pair("ext", "extension"),
                                                      new Pair("frm", "form"),
                                                      new Pair("hdls", "headless"),
                                                      new Pair("ident", "identification"),
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
                                                      new Pair("min", "minimum"),
                                                      new Pair("mngr", "manager"),
                                                      new Pair("mnu", "menuItem"),
                                                      new Pair("msg", "message"),
                                                      new Pair("num", "number"),
                                                      new Pair("obj", "object"),
                                                      new Pair("param", "parameter"),
                                                      new Pair("params", "parameters"),
                                                      new Pair("perc", "percentage"),
                                                      new Pair("perf", "performance"),
                                                      new Pair("phys", "physical"),
                                                      new Pair("pos", "position"),
                                                      new Pair("pow", "power"),
                                                      new Pair("proc", "process"),
                                                      new Pair("procs", "processes"),
                                                      new Pair("prop", "property"),
                                                      new Pair("pt", "point"),
                                                      new Pair("pts", "points"),
                                                      new Pair("ref", "reference"),
                                                      new Pair("repo", "repository"),
                                                      new Pair("req", "request"),
                                                      new Pair("res", "result"),
                                                      new Pair("resp", "response"),
                                                      new Pair("sem", "semantic"),
                                                      new Pair("spec", "specification"),
                                                      new Pair("src", "source"),
                                                      new Pair("std", "standard"),
                                                      new Pair("str", "string"),
                                                      new Pair("sync", "synchronization"),
                                                      new Pair("svc", "service"),
                                                      new Pair("tm", "time"),
                                                      new Pair("tmp", "temp"),
                                                      new Pair("txt", "text"),
                                                      new Pair("ver", "version"),
                                                      new Pair("vol", "volume"),
                                                  };

        private static readonly Pair[] MidTerms = Prefixes.Concat(new[]
                                                                      {
                                                                          new Pair("App", "Application"),
                                                                          new Pair("Apps", "Applications"),
                                                                          new Pair("Assoc", "Association"),
                                                                          new Pair("Auth", "Authorization"),
                                                                          new Pair("Btn", "Button"),
                                                                          new Pair("Cb", "CheckBox"),
                                                                          new Pair("Cert", "Certificate"),
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
                                                                          new Pair("Ctl", "Control"),
                                                                          new Pair("Ctrl", "Control"),
                                                                          new Pair("Ctx", "Context"),
                                                                          new Pair("Db", "Database"),
                                                                          new Pair("Ddl", "DropDownList"),
                                                                          new Pair("Decl", "Declaration"),
                                                                          new Pair("Decr", "Decrypt"),
                                                                          new Pair("Desc", "Description"),
                                                                          new Pair("Dest", "Destination"),
                                                                          new Pair("Diag", "Diagnostic"),
                                                                          new Pair("Diags", "Diagnostics"),
                                                                          new Pair("Dict", "Dictionary"),
                                                                          new Pair("Diff", "Difference"),
                                                                          new Pair("Diffs", "Differences"),
                                                                          new Pair("Dir", "Directory"),
                                                                          new Pair("Dlg", "Dialog"),
                                                                          new Pair("Doc", "Document"),
                                                                          new Pair("Dst", "Destination"),
                                                                          new Pair("Dto", string.Empty),
                                                                          new Pair("DTO", string.Empty),
                                                                          new Pair("Ef", "EntityFramework"),
                                                                          new Pair("EF", "EntityFramework"),
                                                                          new Pair("Encr", "Encrypt"),
                                                                          new Pair("Env", "Environment"),
                                                                          new Pair("Environ", "Environment"),
                                                                          new Pair("Err", "Error"),
                                                                          new Pair("Ext", "Extension"),
                                                                          new Pair("Frm", "Form"),
                                                                          new Pair("Hdls", "Headless"),
                                                                          new Pair("Ident", "Identification"),
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
                                                                          new Pair("Min", "Minimum"),
                                                                          new Pair("Mngr", "Manager"),
                                                                          new Pair("Mnu", "MenuItem"),
                                                                          new Pair("Msg", "Message"),
                                                                          new Pair("Num", "Number"),
                                                                          new Pair("Obj", "Object"),
                                                                          new Pair("Op", "Operation"),
                                                                          new Pair("Param", "Parameter"),
                                                                          new Pair("Params", "Parameters"),
                                                                          new Pair("Perc", "Percentage"),
                                                                          new Pair("Perf", "Performance"),
                                                                          new Pair("Phys", "Physical"),
                                                                          new Pair("Pos", "Position"),
                                                                          new Pair("Pow", "Power"),
                                                                          new Pair("Proc", "Process"),
                                                                          new Pair("Procs", "Processes"),
                                                                          new Pair("Prop", "Property"),
                                                                          new Pair("Props", "Properties"),
                                                                          new Pair("Pt", "Point"),
                                                                          new Pair("Pts", "Points"),
                                                                          new Pair("Ref", "Reference"),
                                                                          new Pair("Repo", "Repository"),
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
                                                                          new Pair("Ver", "Version"),
                                                                          new Pair("Vol", "Volume"),
                                                                      })
                                                          .ToArray();

        private static readonly Pair[] Postfixes = MidTerms.Concat(new[]
                                                                       {
                                                                           new Pair("BL", "BusinessLogic"),
                                                                           new Pair("Bl", "BusinessLogic"),
                                                                           new Pair("VM", "ViewModel"),
                                                                           new Pair("Vm", "ViewModel"),
                                                                       })
                                                           .ToArray();

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
                                                            "text",
                                                            "Text",
                                                            "MEF",
                                                        };

        private static readonly string[] AllowedNames =
                                                        {
                                                            "obj",
                                                            "next",
                                                        };

        public MiKo_1063_AbbreviationsInNameAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);

            base.InitializeCore(context);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => identifiers.Select(_ => _.GetSymbol(semanticModel)).SelectMany(AnalyzeName);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsExtern is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol method && method.IsExtern)
            {
                return false;
            }

            return base.ShallAnalyze(symbol);
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            switch (symbol.Name)
            {
                case "paramName": // used in exceptions
                case "lParam": // used by Windows C++ API
                case "wParam": // used by Windows C++ API
                    return Enumerable.Empty<Diagnostic>();

                default:
                    return AnalyzeName(symbol);
            }
        }

        private static bool IndicatesNewWord(char c) => c == '_' || c.IsUpperCase();

        private static bool PrefixHasIssue(string key, string symbolName) => symbolName.Length > key.Length && IndicatesNewWord(symbolName[key.Length]) && symbolName.StartsWith(key, StringComparison.Ordinal);

        private static bool PostFixHasIssue(string key, string symbolName) => symbolName.EndsWith(key, StringComparison.Ordinal) && symbolName.EndsWithAny(AllowedPostFixTerms, StringComparison.Ordinal) is false;

        private static bool MidTermHasIssue(string key, string symbolName)
        {
            var index = 0;
            var keyLength = key.Length;
            var symbolNameLength = symbolName.Length;

            var keyStartsUpperCase = key[0].IsUpperCase();

            while (true)
            {
                index = symbolName.IndexOf(key, index, StringComparison.Ordinal);

                if (index <= -1)
                {
                    return false;
                }

                var positionAfterCharacter = index + keyLength;

                if (positionAfterCharacter < symbolNameLength && IndicatesNewWord(symbolName[positionAfterCharacter]))
                {
                    if (keyStartsUpperCase)
                    {
                        return true;
                    }

                    var positionBeforeText = index - 1;

                    if (positionBeforeText >= 0 && IndicatesNewWord(symbolName[positionBeforeText]))
                    {
                        return true;
                    }
                }

                index = positionAfterCharacter;
            }
        }

        private static bool CompleteTermHasIssue(string key, string symbolName) => string.Equals(symbolName, key, StringComparison.Ordinal);

        private static IEnumerable<Pair> AnalyzeName(string symbolName)
        {
            if (symbolName.EqualsAny(AllowedNames))
            {
                return Enumerable.Empty<Pair>();
            }

            symbolName = symbolName.Without(AllowedParts);

//// ncrunch: rdi off
            var prefixesWithIssues = Prefixes.Where(_ => PrefixHasIssue(_.Key, symbolName));
            var postFixesWithIssues = Postfixes.Where(_ => PostFixHasIssue(_.Key, symbolName));
            var midTermsWithIssues = MidTerms.Where(_ => MidTermHasIssue(_.Key, symbolName));
            var completeTermsWithIssues = Prefixes.Where(_ => CompleteTermHasIssue(_.Key, symbolName));
//// ncrunch: rdi default

            return Enumerable.Empty<Pair>()
                             .Union(prefixesWithIssues, KeyComparer.Instance)
                             .Union(postFixesWithIssues, KeyComparer.Instance)
                             .Union(midTermsWithIssues, KeyComparer.Instance)
                             .Union(completeTermsWithIssues, KeyComparer.Instance);
        }

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            foreach (var pair in AnalyzeName(symbol.Name))
            {
                yield return Issue(symbol, pair.Key, pair.Value);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            var symbolName = GetFieldNameWithoutPrefix(symbol.Name);

            foreach (var pair in AnalyzeName(symbolName))
            {
                yield return Issue(symbol, pair.Key, pair.Value);
            }

            string GetFieldNameWithoutPrefix(string fieldName)
            {
                // remove any field marker
                foreach (var fieldMarker in Constants.Markers.FieldPrefixes)
                {
                    if (fieldMarker.Length > 0 && fieldName.StartsWith(fieldMarker, StringComparison.Ordinal))
                    {
                        return fieldName.Substring(fieldMarker.Length);
                    }
                }

                return fieldName;
            }
        }

        private sealed class KeyComparer : IEqualityComparer<Pair>
        {
            internal static readonly KeyComparer Instance = new KeyComparer();

            public bool Equals(Pair x, Pair y) => string.Equals(x.Key, y.Key, StringComparison.Ordinal);

            public int GetHashCode(Pair obj) => obj.Key.GetHashCode();
        }
    }
}