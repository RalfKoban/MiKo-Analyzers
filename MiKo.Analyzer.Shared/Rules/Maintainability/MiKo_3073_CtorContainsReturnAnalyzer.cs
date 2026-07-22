using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3073_CtorContainsReturnAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3073";

        private static readonly Func<SyntaxNode, bool> IsNoNestedCall = IsNoNestedCallCore;

        public MiKo_3073_CtorContainsReturnAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor()
                                                                   && symbol.IsPrimaryConstructor() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var syntaxNode = symbol.GetSyntax();

            var returnStatements = syntaxNode.DescendantNodes(IsNoNestedCall).OfType<ReturnStatementSyntax>().ToList();

            if (returnStatements.Count > 0)
            {
                // only report if we have no local function statement, as it is likely that the return is used to separate them from the other code
                if (syntaxNode.DescendantNodes<LocalFunctionStatementSyntax>(SyntaxKind.LocalFunctionStatement).None())
                {
                    var symbolName = symbol.Name;

                    return returnStatements.ToArray(_ => Issue(symbolName, _));
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsNoNestedCallCore(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.LocalFunctionStatement:
                case (int)SyntaxKind.ParenthesizedLambdaExpression:
                case (int)SyntaxKind.SimpleLambdaExpression:
                    return false;

                default:
                    return true;
            }
        }
    }
}