using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class LocalVariableNamingAnalyzer : NamingAnalyzer
    {
        protected LocalVariableNamingAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);

            // TODO RKN: Renaming tuples currently does not work, see https://github.com/dotnet/roslyn/issues/14115 and https://github.com/dotnet/roslyn/issues/20115
            // context.RegisterSyntaxNodeAction(AnalyzeTupleElement, SyntaxKind.TupleElement);
            context.RegisterSyntaxNodeAction(AnalyzeTupleExpression, SyntaxKind.TupleExpression); // also covers tuple in ForEachVariableStatement
            context.RegisterSyntaxNodeAction(AnalyzeVariableDesignation, SyntaxKind.SingleVariableDesignation); // also covers variable designation in ForEachVariableStatement
            context.RegisterSyntaxNodeAction(AnalyzeVariableDesignation, SyntaxKind.ParenthesizedVariableDesignation); // also covers variable designation in ForEachVariableStatement

            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol, Compilation compilation) => base.ShallAnalyze(symbol, compilation) || symbol?.TypeKind is TypeKind.Array; // accept analysis of arrays
    }
}