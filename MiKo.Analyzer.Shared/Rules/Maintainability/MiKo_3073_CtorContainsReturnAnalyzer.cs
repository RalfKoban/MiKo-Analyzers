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

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.GetSyntax()
                                                                                                                   .DescendantNodes(IsNoNestedCall)
                                                                                                                   .OfType<ReturnStatementSyntax>()
                                                                                                                   .Select(_ => Issue(symbol.Name, _));

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