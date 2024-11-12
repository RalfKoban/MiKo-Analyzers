using System.Collections.Generic;

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

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            // normal local variables are covered in MiKo_1026
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var exceeding = GetExceedingCharacters(identifier.ValueText);

                    if (exceeding > 0)
                    {
                        var symbol = identifier.GetSymbol(semanticModel);

                        yield return Issue(symbol, exceeding);
                    }
                }
            }
        }
    }
}