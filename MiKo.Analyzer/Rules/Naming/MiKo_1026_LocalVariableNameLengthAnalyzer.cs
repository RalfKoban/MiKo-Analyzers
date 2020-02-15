using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    /// <seealso cref="MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1026_LocalVariableNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1026";

        public MiKo_1026_LocalVariableNameLengthAnalyzer() : base(Id, (SymbolKind)(-1), Constants.MaxNamingLengths.LocalVariables)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            // local variables in 'for' and 'foreach' statements are covered by MiKo_1027 rule
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers) => from identifier in identifiers
                                                                                                                                 let exceeding = GetExceedingCharacters(identifier.ValueText)
                                                                                                                                 where exceeding > 0
                                                                                                                                 let symbol = identifier.GetSymbol(semanticModel)
                                                                                                                                 select Issue(symbol, exceeding);
    }
}