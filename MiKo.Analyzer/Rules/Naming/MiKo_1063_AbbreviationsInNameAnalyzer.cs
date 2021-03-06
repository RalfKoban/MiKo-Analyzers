﻿using System;
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
                                                                                       { "btn", "button" },
                                                                                       { "cb", "checkBox" },
                                                                                       { "cmd", "command" },
                                                                                       { "ddl", "dropDownList" },
                                                                                       { "decl", "declaration" },
                                                                                       { "desc", "description" },
                                                                                       { "dict", "dictionary" },
                                                                                       { "dir", "directory" },
                                                                                       { "doc", "document" },
                                                                                       { "idx", "index" },
                                                                                       { "itf", "interface" },
                                                                                       { "lbl", "label" },
                                                                                       { "max", "maximum" },
                                                                                       { "mgr", "manager" },
                                                                                       { "min", "minimum" },
                                                                                       { "mngr", "manager" },
                                                                                       { "msg", "message" },
                                                                                       { "num", "number" },
                                                                                       { "param", "parameter" },
                                                                                       { "params", "parameters" },
                                                                                       { "proc", "process" },
                                                                                       { "procs", "processes" },
                                                                                       { "prop", "property" },
                                                                                       { "pos", "position" },
                                                                                       { "pt", "point" },
                                                                                       { "pts", "points" },
                                                                                       { "res", "result" },
                                                                                       { "std", "standard" },
                                                                                       { "str", "string" },
                                                                                       { "tmp", "temp" },
                                                                                       { "txt", "text" },
                                                                                   };

        private static readonly IReadOnlyDictionary<string, string> MidTerms = new Dictionary<string, string>((IDictionary<string, string>)Prefixes)
                                                                                   {
                                                                                       { "Btn", "Button" },
                                                                                       { "Cb", "CheckBox" },
                                                                                       { "Cmd", "Command" },
                                                                                       { "Ddl", "DropDownList" },
                                                                                       { "Decl", "Declaration" },
                                                                                       { "Desc", "Description" },
                                                                                       { "Dict", "Dictionary" },
                                                                                       { "Dir", "Directory" },
                                                                                       { "Doc", "Document" },
                                                                                       { "Idx", "Index" },
                                                                                       { "Itf", "Interface" },
                                                                                       { "Lbl", "Label" },
                                                                                       { "Max", "Maximum" },
                                                                                       { "Mgr", "Manager" },
                                                                                       { "Min", "Minimum" },
                                                                                       { "Mngr", "Manager" },
                                                                                       { "Msg", "Message" },
                                                                                       { "Num", "Number" },
                                                                                       { "Param", "Parameter" },
                                                                                       { "Params", "Parameters" },
                                                                                       { "Proc", "Process" },
                                                                                       { "Procs", "Processes" },
                                                                                       { "Prop", "Property" },
                                                                                       { "Props", "Properties" },
                                                                                       { "Pos", "Position" },
                                                                                       { "Pt", "Point" },
                                                                                       { "Pts", "Points" },
                                                                                       { "Res", "Result" },
                                                                                       { "Std", "Standard" },
                                                                                       { "Str", "String" },
                                                                                       { "Tmp", "Temp" },
                                                                                       { "Txt", "Text" },
                                                                                   };

        private static readonly IReadOnlyDictionary<string, string> Postfixes = new Dictionary<string, string>((IDictionary<string, string>)MidTerms)
                                                                                    {
                                                                                        { "VM", "ViewModel" },
                                                                                        { "Vm", "ViewModel" },
                                                                                        { "BL", "BusinessLogic" },
                                                                                        { "Bl", "BusinessLogic" },
                                                                                        { "Err", "Error" },
                                                                                    };

        private static readonly string[] AllowedPostFixTerms =
            {
                "cept",
                "dopt",
                "ires",
                "ixtures",
                "rupt",
                "ures",
                "wares",
            };

        private static readonly string[] AllowedNames =
            {
                "Enumerable",
                "Enumeration",
                "Enum", // must be after the others so that those get properly replaced
            };

        public MiKo_1063_AbbreviationsInNameAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);

            base.InitializeCore(context);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => identifiers.Select(_ => _.GetSymbol(semanticModel)).SelectMany(AnalyzeName);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => symbol.Name == "paramName"
                                                                                               ? Enumerable.Empty<Diagnostic>()
                                                                                               : AnalyzeName(symbol);

        private static bool PrefixHasIssue(string key, string symbolName) => symbolName.StartsWith(key, StringComparison.Ordinal) && symbolName.Length > key.Length && symbolName[key.Length].IsUpperCase();

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
                    break;
                }

                var positionBeforeText = index - 1;
                var positionAfterCharacter = index + keyLength;
                if (positionAfterCharacter < symbolNameLength && symbolName[positionAfterCharacter].IsUpperCase())
                {
                    if (keyStartsUpperCase)
                    {
                        return true;
                    }

                    if (positionBeforeText >= 0 && symbolName[positionBeforeText].IsUpperCase())
                    {
                        return true;
                    }
                }

                index = positionAfterCharacter;
            }

            return false;
        }

        private static bool CompleteTermHasIssue(string key, string symbolName) => string.Equals(symbolName, key, StringComparison.Ordinal);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name.Without(AllowedNames);

            var prefixesWithIssues = Prefixes.Where(_ => PrefixHasIssue(_.Key, symbolName));
            var postFixesWithIssues = Postfixes.Where(_ => PostFixHasIssue(_.Key, symbolName));
            var midTermsWithIssues = MidTerms.Where(_ => MidTermHasIssue(_.Key, symbolName));
            var completeTermsWithIssues = Prefixes.Where(_ => CompleteTermHasIssue(_.Key, symbolName));

            return prefixesWithIssues.Concat(postFixesWithIssues).Concat(midTermsWithIssues).Concat(completeTermsWithIssues).Distinct(KeyComparer.Instance).Select(_ => Issue(symbol, _.Key, _.Value));
        }

        private sealed class KeyComparer : IEqualityComparer<KeyValuePair<string, string>>
        {
            internal static readonly KeyComparer Instance = new KeyComparer();

            public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y) => string.Equals(x.Key, y.Key, StringComparison.Ordinal);

            public int GetHashCode(KeyValuePair<string, string> obj) => obj.Key.GetHashCode();
        }
    }
}