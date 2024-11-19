using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    //// <seealso cref="MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1026_LocalVariableNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1026";

        public MiKo_1026_LocalVariableNameLengthAnalyzer() : base(Id, (SymbolKind)(-1), Constants.MaxNamingLengths.LocalVariables)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            // local variables in 'for' and 'foreach' statements are covered by MiKo_1027 rule
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
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