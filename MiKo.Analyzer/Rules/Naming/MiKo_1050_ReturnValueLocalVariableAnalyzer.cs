using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1050_ReturnValueLocalVariableAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1050";

        private static readonly string[] WrongNames = CreateWrongNames("ret", "retVal", "retVals", "returnVal", "returnVals", "returnValue", "returnValues");

        public MiKo_1050_ReturnValueLocalVariableAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private static string[] CreateWrongNames(params string[] values)
        {
            var results = new HashSet<string>();

            foreach (var value in values)
            {
                results.Add(value);
                foreach (var i in Enumerable.Range(0, 10))
                {
                    results.Add(value + i);
                    results.Add(value + "_" + i);
                }
            }

            return results.ToArray();
        }

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                if (name.EqualsAny(WrongNames, StringComparison.OrdinalIgnoreCase))
                {
                    var symbol = identifier.GetSymbol(semanticModel);

                    if (results == null) results = new List<Diagnostic>();
                    results.Add(Issue(symbol, name));
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}