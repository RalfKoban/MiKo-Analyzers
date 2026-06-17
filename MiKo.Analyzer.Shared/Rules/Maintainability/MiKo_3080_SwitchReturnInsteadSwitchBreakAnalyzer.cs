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
            if (switchStatement.DescendantNodes<ReturnStatementSyntax>().Any())
            {
                return false;
            }

            var usedIdentifiers = switchStatement.Sections.Select(_ => _.Statements.SelectMany(GetAssignmentIdentifierCandidates).ToHashSet()).ToList();

            if (usedIdentifiers.Count is 0)
            {
                // for whatever reason we do not have any identifiers, so we do not have an issue here
                return false;
            }

            return HasIssueWithVariable(switchStatement, usedIdentifiers)
                || HasIssueWithProperty(switchStatement, usedIdentifiers)
                || HasIssueWithField(switchStatement, usedIdentifiers)
                || HasIssueWithParameter(switchStatement, usedIdentifiers);
        }

        private static bool HasIssue(IReadOnlyList<HashSet<string>> usedIdentifiers, HashSet<string> identifiers)
        {
            if (identifiers.None())
            {
                return false;
            }

            // keep those that are used on multiple cases
            var identifierUsages = usedIdentifiers[0].ToHashSet();

            if (usedIdentifiers.Count > 1)
            {
                // we have to collect all other together as otherwise, an intersect for each single block would ignore (get rid of) them in the other blocks
                var allOtherIdentifiers = usedIdentifiers.Skip(1).SelectMany(_ => _).ToHashSet();
                identifierUsages.IntersectWith(allOtherIdentifiers);
            }

            // now only keep those that are part of the original collection
            identifierUsages.IntersectWith(identifiers);

            return identifierUsages.Count != 0;
        }

        private static bool HasIssueWithParameter(SwitchStatementSyntax switchStatement, IReadOnlyList<HashSet<string>> usedIdentifiers)
        {
            var method = switchStatement.FirstAncestor<BaseMethodDeclarationSyntax>();

            if (method is null)
            {
                // may be used in global context
                return false;
            }

            var parameterNames = method.ParameterList.Parameters.ToHashSet(_ => _.GetName());

            return HasIssue(usedIdentifiers, parameterNames);
        }

        private static bool HasIssueWithField(SwitchStatementSyntax switchStatement, IReadOnlyList<HashSet<string>> usedIdentifiers)
        {
            var type = switchStatement.FirstAncestor<BaseTypeDeclarationSyntax>();

            if (type is null)
            {
                // may be used in global context
                return false;
            }

            var fieldNames = type.ChildNodes<BaseFieldDeclarationSyntax>().SelectMany(_ => _.Declaration.Variables).ToHashSet(_ => _.GetName());

            return HasIssue(usedIdentifiers, fieldNames);
        }

        private static bool HasIssueWithProperty(SwitchStatementSyntax switchStatement, IReadOnlyList<HashSet<string>> usedIdentifiers)
        {
            var type = switchStatement.FirstAncestor<BaseTypeDeclarationSyntax>();

            if (type is null)
            {
                // may be used in global context
                return false;
            }

            var fieldNames = type.ChildNodes<BasePropertyDeclarationSyntax>().ToHashSet(_ => _.GetName());

            return HasIssue(usedIdentifiers, fieldNames);
        }

        private static bool HasIssueWithVariable(SwitchStatementSyntax switchStatement, IReadOnlyList<HashSet<string>> usedIdentifiers)
        {
            var variableNames = GetVariableNamesUntilHere(switchStatement).ToHashSet();

            return HasIssue(usedIdentifiers, variableNames);
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
            var candidates = node.DescendantNodes<AssignmentExpressionSyntax>(SyntaxKind.SimpleAssignmentExpression)
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
