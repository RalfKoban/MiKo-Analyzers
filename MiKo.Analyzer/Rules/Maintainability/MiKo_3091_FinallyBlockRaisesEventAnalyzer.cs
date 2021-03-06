﻿using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3091_FinallyBlockRaisesEventAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3091";

        public MiKo_3091_FinallyBlockRaisesEventAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeFinallyClause, SyntaxKind.FinallyClause);

        private void AnalyzeFinallyClause(SyntaxNodeAnalysisContext context)
        {
            var finallyBlock = ((FinallyClauseSyntax)context.Node).Block;
            if (finallyBlock is null)
            {
                return;
            }

            var method = context.GetEnclosingMethod();
            var events = method.ContainingType.GetMembersIncludingInherited<IEventSymbol>().Select(_ => _.Name).ToHashSet();

            foreach (var token in finallyBlock.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.IdentifierToken)))
            {
                var eventName = token.ValueText;

                if (events.Contains(eventName) && token.GetSymbol(context.SemanticModel) is IEventSymbol)
                {
                    var issue = Issue(method.Name, token, eventName);
                    context.ReportDiagnostic(issue);
                }
            }
        }
    }
}