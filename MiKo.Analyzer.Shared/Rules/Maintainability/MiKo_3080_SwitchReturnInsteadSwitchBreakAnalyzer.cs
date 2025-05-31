﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3080_SwitchReturnInsteadSwitchBreakAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3080";

        private static readonly Func<SyntaxNode, bool> IsNoDeclaration = IsNoDeclarationCore;

        public MiKo_3080_SwitchReturnInsteadSwitchBreakAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);

        private static bool IsNoDeclarationCore(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.MethodDeclaration:
                case (int)SyntaxKind.ConstructorDeclaration:
                case (int)SyntaxKind.IndexerDeclaration:
                    return false;

                default:
                    return true;
            }
        }

        private static bool HasIssue(SwitchStatementSyntax switchStatement)
        {
            var variableNames = GetVariableNamesUntilHere(switchStatement).ToHashSet();

            var usedVariables = switchStatement.Sections.Select(_ => _.Statements.SelectMany(GetAssignmentIdentifierCandidates).ToHashSet()).ToList();

            if (usedVariables.Count is 0)
            {
                // for whatever reason we do not have any variables
                return false;
            }

            // keep those that are used on multiple cases
            var variableUsages = usedVariables[0];

            if (usedVariables.Count > 1)
            {
                // we have to collect all other variables together as otherwise, an intersect for each single block would ignore (get rid of) the variables in the other blocks
                var allOtherVariables = usedVariables.Skip(1).SelectMany(_ => _).ToHashSet();
                variableUsages.IntersectWith(allOtherVariables);
            }

            // now only keep those that are part of the original collection
            variableUsages.IntersectWith(variableNames);

            var foundReturn = switchStatement.DescendantNodes<ReturnStatementSyntax>().Any();

            return variableUsages.Count != 0 && foundReturn is false;
        }

        private static IEnumerable<string> GetVariableNamesUntilHere(SwitchStatementSyntax switchStatement)
        {
            return switchStatement.Ancestors()
                                  .TakeWhile(IsNoDeclaration)
                                  .SelectMany(_ => GetVariableNames(_, switchStatement));

            IEnumerable<string> GetVariableNames(SyntaxNode node, SyntaxNode stopNode) => node.ChildNodes()
                                                                                              .TakeWhile(_ => _ != stopNode)
                                                                                              .OfType<LocalDeclarationStatementSyntax>()
                                                                                              .SelectMany(_ => _.Declaration.Variables)
                                                                                              .Select(_ => _.GetName());
        }

        private static IEnumerable<string> GetAssignmentIdentifierCandidates(SyntaxNode node)
        {
            var candidates = node.DescendantNodes<AssignmentExpressionSyntax>(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression))
                                 .Select(_ => _.FirstChild<IdentifierNameSyntax>())
                                 .WhereNotNull()
                                 .Select(_ => _.GetName());

            return candidates;
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            if (HasIssue(switchStatement))
            {
                // keep in mind that 'while(true) { switch ... }' is a performance optimization to avoid recursive calls
                if (switchStatement.AncestorsWithinMethods<WhileStatementSyntax>().None())
                {
                    ReportDiagnostics(context, Issue(switchStatement));
                }
            }
        }
    }
}
