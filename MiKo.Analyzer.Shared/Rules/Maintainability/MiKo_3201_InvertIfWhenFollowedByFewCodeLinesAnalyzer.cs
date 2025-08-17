using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer : InvertNegativeIfAnalyzer
    {
        public const string Id = "MiKo_3201";

        private const int MaximumAllowedFollowUpStatements = 3;

        private static readonly ISet<SyntaxKind> ForbiddenFollowUps = new HashSet<SyntaxKind>
                                                                          {
                                                                              SyntaxKind.DoStatement,
                                                                              SyntaxKind.WhileStatement,
                                                                              SyntaxKind.ForStatement,
                                                                              SyntaxKind.ForEachStatement,
                                                                              SyntaxKind.SwitchStatement,
                                                                              SyntaxKind.IfStatement,
                                                                              SyntaxKind.UsingStatement,
                                                                              SyntaxKind.TryStatement,
                                                                              SyntaxKind.LocalFunctionStatement,
                                                                          };

        private static readonly ISet<SyntaxKind> ForbiddenInsides = new HashSet<SyntaxKind>
                                                                        {
                                                                            SyntaxKind.DoStatement,
                                                                            SyntaxKind.WhileStatement,
                                                                            SyntaxKind.ForStatement,
                                                                            SyntaxKind.ForEachStatement,
                                                                            SyntaxKind.SwitchStatement,
                                                                            SyntaxKind.LockStatement,
                                                                        };

        public MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIfStatement(IfStatementSyntax node, in SyntaxNodeAnalysisContext context)
        {
            if (IsApplicable(node))
            {
                if (node.Parent is BlockSyntax block)
                {
                    if (block.Parent is IfStatementSyntax)
                    {
                        // do not invert nested ones
                        return Array.Empty<Diagnostic>();
                    }

                    var otherStatements = block.Statements.ToList();
                    otherStatements.Remove(node); // get rid of the 'if' statement itself

                    if (otherStatements.Count > 0 && otherStatements.Count <= MaximumAllowedFollowUpStatements)
                    {
                        if (otherStatements.Exists(_ => _.IsAnyKind(ForbiddenFollowUps)))
                        {
                            // we assume that those follow-ups also contain code, so inverting would make the code less readable
                            return Array.Empty<Diagnostic>();
                        }

                        if (node.IsInside(ForbiddenInsides))
                        {
                            // inverting would alter behavior
                            return Array.Empty<Diagnostic>();
                        }

                        if (node.FirstDescendant<ReturnStatementSyntax>().HasComment())
                        {
                            // the developer documented a reason, in that case we keep the if statement
                            return Array.Empty<Diagnostic>();
                        }

                        // report only in case we have something to invert
                        var method = node.GetEnclosingMethod(context.SemanticModel);

                        if (method != null && method.ReturnsVoid)
                        {
                            return new[] { Issue(node) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsApplicable(IfStatementSyntax node) => node.Else is null // do not invert in case of an else block
                                                                    && node.ReturnsImmediately();
    }
}