using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer"/>
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

            context.RegisterSyntaxNodeAction(AnalyzeTupleExpression, SyntaxKind.TupleExpression); // also covers tuple in ForEachVariableStatement
            context.RegisterSyntaxNodeAction(AnalyzeVariableDesignation, SyntaxKind.ParenthesizedVariableDesignation); // also covers variable designation in ForEachVariableStatement
            context.RegisterSyntaxNodeAction(AnalyzeVariableDesignation, SyntaxKind.SingleVariableDesignation); // also covers variable designation in ForEachVariableStatement
        }

        protected override void AnalyzeTupleExpression(in SyntaxNodeAnalysisContext context, TupleExpressionSyntax node)
        {
            if (node.FirstAncestorWithinMethod<ForEachVariableStatementSyntax>() is ForEachVariableStatementSyntax loop && loop.Variable.Span.Contains(node.Span))
            {
                return; // covered by MiKo_1027
            }

            base.AnalyzeTupleExpression(context, node);
        }

        protected override void AnalyzeVariableDesignation(in SyntaxNodeAnalysisContext context, VariableDesignationSyntax node)
        {
            if (node.FirstAncestorWithinMethod<ForEachVariableStatementSyntax>() is ForEachVariableStatementSyntax loop && loop.Variable.Span.Contains(node.Span))
            {
                return; // covered by MiKo_1027
            }

            base.AnalyzeVariableDesignation(context, node);
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