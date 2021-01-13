using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    //// <seealso cref="MiKo_1026_LocalVariableNameLengthAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1027";

        public MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer() : base(Id, (SymbolKind)(-1), Constants.MaxNamingLengths.LocalVariablesInLoops)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            // normal local variables are covered in MiKo_1026
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers) => from identifier in identifiers
                                                                                                                                 let exceeding = GetExceedingCharacters(identifier.ValueText)
                                                                                                                                 where exceeding > 0
                                                                                                                                 let symbol = identifier.GetSymbol(semanticModel)
                                                                                                                                 select Issue(symbol, exceeding);
    }
}