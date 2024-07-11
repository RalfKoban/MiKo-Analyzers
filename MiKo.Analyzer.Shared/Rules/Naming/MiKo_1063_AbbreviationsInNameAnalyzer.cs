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

        private static readonly IReadOnlyDictionary<string, string> Prefixes = new Dictionary<string, string>
                                                                                   {
                                                                                       { "app", "application" },
                                                                                       { "apps", "applications" },
                                                                                       { "assoc", "association" },
                                                                                       { "auth", "authorization" },
                                                                                       { "btn", "button" },
                                                                                       { "cb", "checkBox" },
                                                                                       { "cert", "certificate" },
                                                                                       { "chk", "checkBox" },
                                                                                       { "cls", "class" },
                                                                                       { "cmb", "comboBox" },
                                                                                       { "cmd", "command" },
                                                                                       { "conf", "configuration" },
                                                                                       { "config", "configuration" },
                                                                                       { "configs", "configurations" },
                                                                                       { "conn", "connection" },
                                                                                       { "ctl", "control" },
                                                                                       { "ctrl", "control" },
                                                                                       { "ctx", "context" },
                                                                                       { "db", "database" },
                                                                                       { "ddl", "dropDownList" },
                                                                                       { "decl", "declaration" },
                                                                                       { "desc", "description" },
                                                                                       { "dest", "destination" },
                                                                                       { "diag", "diagnostic" },
                                                                                       { "diags", "diagnostics" },
                                                                                       { "dict", "dictionary" },
                                                                                       { "diff", "difference" },
                                                                                       { "diffs", "differences" },
                                                                                       { "dir", "directory" },
                                                                                       { "dlg", "dialog" },
                                                                                       { "doc", "document" },
                                                                                       { "dst", "destination" },
                                                                                       { "dto", string.Empty },
                                                                                       { "ef", "entityFramework" },
                                                                                       { "env", "environment" },
                                                                                       { "environ", "environment" },
                                                                                       { "err", "error" },
                                                                                       { "ext", "extension" },
                                                                                       { "frm", "form" },
                                                                                       { "hdls", "headless" },
                                                                                       { "ident", "identification" },
                                                                                       { "idx", "index" },
                                                                                       { "init", "initialize" },
                                                                                       { "itf", "interface" },
                                                                                       { "lbl", "label" },
                                                                                       { "lib", "library" },
                                                                                       { "libs", "libraries" },
                                                                                       { "lv", "listView" },
                                                                                       { "max", "maximum" },
                                                                                       { "meth", "method" },
                                                                                       { "mgr", "manager" },
                                                                                       { "min", "minimum" },
                                                                                       { "mngr", "manager" },
                                                                                       { "mnu", "menuItem" },
                                                                                       { "msg", "message" },
                                                                                       { "num", "number" },
                                                                                       { "param", "parameter" },
                                                                                       { "params", "parameters" },
                                                                                       { "perc", "percentage" },
                                                                                       { "perf", "performance" },
                                                                                       { "pos", "position" },
                                                                                       { "proc", "process" },
                                                                                       { "procs", "processes" },
                                                                                       { "prop", "property" },
                                                                                       { "pt", "point" },
                                                                                       { "pts", "points" },
                                                                                       { "repo", "repository" },
                                                                                       { "req", "request" },
                                                                                       { "res", "result" },
                                                                                       { "resp", "response" },
                                                                                       { "spec", "specification" },
                                                                                       { "src", "source" },
                                                                                       { "std", "standard" },
                                                                                       { "str", "string" },
                                                                                       { "sync", "synchronization" },
                                                                                       { "svc", "service" },
                                                                                       { "tm", "time" },
                                                                                       { "tmp", "temp" },
                                                                                       { "txt", "text" },
                                                                                       { "vol", "volume" },
                                                                                   };

        private static readonly IReadOnlyDictionary<string, string> MidTerms = new Dictionary<string, string>((IDictionary<string, string>)Prefixes)
                                                                                   {
                                                                                       { "App", "Application" },
                                                                                       { "Apps", "Applications" },
                                                                                       { "Assoc", "Association" },
                                                                                       { "Auth", "Authorization" },
                                                                                       { "Btn", "Button" },
                                                                                       { "Cb", "CheckBox" },
                                                                                       { "Cert", "Certificate" },
                                                                                       { "Chk", "CheckBox" },
                                                                                       { "Cli", "CommandLineInterface" },
                                                                                       { "CLI", "CommandLineInterface" },
                                                                                       { "Cls", "Class" },
                                                                                       { "Cmb", "ComboBox" },
                                                                                       { "Cmd", "Command" },
                                                                                       { "Conf", "Configuration" },
                                                                                       { "Config", "Configuration" },
                                                                                       { "Configs", "Configurations" },
                                                                                       { "Conn", "Connection" },
                                                                                       { "Ctl", "Control" },
                                                                                       { "Ctrl", "Control" },
                                                                                       { "Ctx", "Context" },
                                                                                       { "Db", "Database" },
                                                                                       { "Ddl", "DropDownList" },
                                                                                       { "Decl", "Declaration" },
                                                                                       { "Desc", "Description" },
                                                                                       { "Dest", "Destination" },
                                                                                       { "Diag", "Diagnostic" },
                                                                                       { "Diags", "Diagnostics" },
                                                                                       { "Dict", "Dictionary" },
                                                                                       { "Diff", "Difference" },
                                                                                       { "Diffs", "Differences" },
                                                                                       { "Dir", "Directory" },
                                                                                       { "Dlg", "Dialog" },
                                                                                       { "Doc", "Document" },
                                                                                       { "Dst", "Destination" },
                                                                                       { "Dto", string.Empty },
                                                                                       { "DTO", string.Empty },
                                                                                       { "Ef", "EntityFramework" },
                                                                                       { "EF", "EntityFramework" },
                                                                                       { "Env", "Environment" },
                                                                                       { "Environ", "Environment" },
                                                                                       { "Err", "Error" },
                                                                                       { "Ext", "Extension" },
                                                                                       { "Frm", "Form" },
                                                                                       { "Hdls", "Headless" },
                                                                                       { "Ident", "Identification" },
                                                                                       { "Idx", "Index" },
                                                                                       { "Init", "Initialize" },
                                                                                       { "Itf", "Interface" },
                                                                                       { "Lbl", "Label" },
                                                                                       { "Lib", "Library" },
                                                                                       { "Libs", "Libraries" },
                                                                                       { "Lv", "ListView" },
                                                                                       { "Max", "Maximum" },
                                                                                       { "Meth", "Method" },
                                                                                       { "Mgr", "Manager" },
                                                                                       { "Min", "Minimum" },
                                                                                       { "Mngr", "Manager" },
                                                                                       { "Mnu", "MenuItem" },
                                                                                       { "Msg", "Message" },
                                                                                       { "Num", "Number" },
                                                                                       { "Op", "Operation" },
                                                                                       { "Param", "Parameter" },
                                                                                       { "Params", "Parameters" },
                                                                                       { "Perc", "Percentage" },
                                                                                       { "Perf", "Performance" },
                                                                                       { "Pos", "Position" },
                                                                                       { "Proc", "Process" },
                                                                                       { "Procs", "Processes" },
                                                                                       { "Prop", "Property" },
                                                                                       { "Props", "Properties" },
                                                                                       { "Pt", "Point" },
                                                                                       { "Pts", "Points" },
                                                                                       { "Repo", "Repository" },
                                                                                       { "Req", "Request" },
                                                                                       { "Res", "Result" },
                                                                                       { "Resp", "Response" },
                                                                                       { "Spec", "Specification" },
                                                                                       { "Src", "Source" },
                                                                                       { "Std", "Standard" },
                                                                                       { "Str", "String" },
                                                                                       { "Sync", "Synchronization" },
                                                                                       { "Svc", "Service" },
                                                                                       { "Tm", "Time" },
                                                                                       { "Tmp", "Temp" },
                                                                                       { "Txt", "Text" },
                                                                                       { "Vol", "Volume" },
                                                                                   };

        private static readonly IReadOnlyDictionary<string, string> Postfixes = new Dictionary<string, string>((IDictionary<string, string>)MidTerms)
                                                                                    {
                                                                                        { "BL", "BusinessLogic" },
                                                                                        { "Bl", "BusinessLogic" },
                                                                                        { "VM", "ViewModel" },
                                                                                        { "Vm", "ViewModel" },
                                                                                    };

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

        private static readonly string[] AllowedNames =
                                                        {
                                                            "Async",
                                                            "Enumerable",
                                                            "Enumeration",
                                                            "Enum", // must be after the others so that those get properly replaced
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

        private static IEnumerable<KeyValuePair<string, string>> AnalyzeName(string symbolName)
        {
            symbolName = symbolName.Without(AllowedNames);

//// ncrunch: rdi off
            var prefixesWithIssues = Prefixes.Where(_ => PrefixHasIssue(_.Key, symbolName));
            var postFixesWithIssues = Postfixes.Where(_ => PostFixHasIssue(_.Key, symbolName));
            var midTermsWithIssues = MidTerms.Where(_ => MidTermHasIssue(_.Key, symbolName));
            var completeTermsWithIssues = Prefixes.Where(_ => CompleteTermHasIssue(_.Key, symbolName));
//// ncrunch: rdi default

            return Enumerable.Empty<KeyValuePair<string, string>>()
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
                foreach (var fieldMarker in Constants.Markers.FieldPrefixes.Where(_ => _.Length > 0 && fieldName.StartsWith(_, StringComparison.Ordinal)))
                {
                    return fieldName.Substring(fieldMarker.Length);
                }

                return fieldName;
            }
        }

        private sealed class KeyComparer : IEqualityComparer<KeyValuePair<string, string>>
        {
            internal static readonly KeyComparer Instance = new KeyComparer();

            public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y) => string.Equals(x.Key, y.Key, StringComparison.Ordinal);

            public int GetHashCode(KeyValuePair<string, string> obj) => obj.Key.GetHashCode();
        }
    }
}