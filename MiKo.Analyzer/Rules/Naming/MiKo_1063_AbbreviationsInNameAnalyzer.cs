using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1063_AbbreviationsInNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1063";

        private static readonly IReadOnlyDictionary<string, string> Prefixes = new Dictionary<string, string>
                                                                                   {
                                                                                       { "btn", "button" },
                                                                                       { "cb", "checkBox" },
                                                                                       { "cmd", "command" },
                                                                                       { "ddl", "dropDownList" },
                                                                                       { "lbl", "label" },
                                                                                       { "mgr", "manager" },
                                                                                       { "mngr", "manager" },
                                                                                       { "msg", "message" },
                                                                                       { "propName", "propertyName" },
                                                                                       { "tmp", "temp" },
                                                                                       { "txt", "text" },

                                                                                       { "Btn", "Button" },
                                                                                       { "Cb", "CheckBox" },
                                                                                       { "Cmd", "Command" },
                                                                                       { "Ddl", "DropDownList" },
                                                                                       { "Lbl", "Label" },
                                                                                       { "Mgr", "Manager" },
                                                                                       { "Mngr", "Manager" },
                                                                                       { "Msg", "Message" },
                                                                                       { "PropName", "propertyName" },
                                                                                       { "Tmp", "temp" },
                                                                                       { "Txt", "Text" },
                                                                                   };

        private static readonly IReadOnlyDictionary<string, string> Postfixes = new Dictionary<string, string>((IDictionary<string, string>)Prefixes)
                                                                                    {
                                                                                        { "VM", "ViewModel" },
                                                                                        { "Vm", "ViewModel" },
                                                                                        { "BL", "BusinessLogic" },
                                                                                        { "Bl", "BusinessLogic" },
                                                                                        { "Prop", "Property" },
                                                                                        { "PropNames", "PropertyNames" },
                                                                                    };

        public MiKo_1063_AbbreviationsInNameAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => identifiers.Select(_ => _.GetSymbol(semanticModel)).SelectMany(AnalyzeName);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            var prefixesWithIssues = Prefixes.Where(_ => symbolName.StartsWith(_.Key, StringComparison.Ordinal));
            var postFixesWithIssues = Postfixes.Where(_ => symbolName.EndsWith(_.Key, StringComparison.Ordinal));

            return prefixesWithIssues.Concat(postFixesWithIssues).Select(_ => ReportIssue(symbol, _.Key, _.Value));
        }
    }
}